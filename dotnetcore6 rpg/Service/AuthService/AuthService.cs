using dotnetcore6_rpg.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace dotnetcore6_rpg.Service.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly DataContext _context;

        private readonly IConfiguration _configuration;

        public AuthService(DataContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration; //needed for the security key in appsettings.json
        }
        public async Task<ServiceResponse<string>> Login(string username, string password)
        {
            ServiceResponse<string> serviceResponse = new ServiceResponse<string>();
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower().Equals(username));

                if (user == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User not found!";
                }
                else if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Wrong password!";
                }
                else
                {
                    serviceResponse.Success = true;
                    serviceResponse.Data = CreateToken(user);
                }
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Exception thrown: " + ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<int>> Register(User user, string password)
        {
            ServiceResponse<int> serviceResponse = new ServiceResponse<int>();

            try
            {
                if (await UserExists(user.Username))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "User already exists!";
                    return serviceResponse;
                }

                CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;

                _context.Add(user);
                await _context.SaveChangesAsync();
                serviceResponse.Data = user.Id;
                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = "Exception thrown: " + ex.Message;
                return serviceResponse;
            }
        }

        public async Task<bool> UserExists(string username)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower()))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username.ToString())
            };

            SymmetricSecurityKey key = new SymmetricSecurityKey
                (System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value)); //get symmetric key from configuration file appsettings.json
            SigningCredentials credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); //create signing creds for the key

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
