using AutoMapper;
using dotnetcore6_rpg.Data;
using dotnetcore6_rpg.Dtos.Character;
using dotnetcore6_rpg.Dtos.Weapon;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dotnetcore6_rpg.Service.WeaponService
{
    public class WeaponService : IWeaponService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public WeaponService(DataContext context, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User
                    .FindFirstValue(ClaimTypes.NameIdentifier));


        public async Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto weapon)
        {
            ServiceResponse<GetCharacterDto> serviceResponse = new ServiceResponse<GetCharacterDto>();
            try
            {
                Character character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == weapon.CharacterId && c.User.Id == GetUserId());

                if (character == null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                }
                else
                {
                    Weapon newWeapon = new Weapon
                    {
                        Name = weapon.Name,
                        Damage=weapon.Damage,
                        Character = character,
                    };

                    _context.Weapons.Add(newWeapon);
                    await _context.SaveChangesAsync();
                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
                }
            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;

        }
    }
}
