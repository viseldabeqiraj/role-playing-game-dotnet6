using AutoMapper;
using dotnetcore6_rpg.Data;
using dotnetcore6_rpg.Dtos.Character;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace dotnetcore6_rpg.Service.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _contextAccessor;

        public CharacterService(IMapper mapper, DataContext context, IHttpContextAccessor contextAccessor)
        {
            _mapper = mapper;
            _context = context;
            _contextAccessor = contextAccessor;
        }

        private int GetUserId() => int.Parse(_contextAccessor.HttpContext.User
            .FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<GetCharacterDto>>> AddCharacter(AddCharacterDto newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                Character character = _mapper.Map<Character>(newCharacter);
                character.User = await _context.Users.FirstOrDefaultAsync(u => u.Id == GetUserId());
                _context.Characters.Add(character);
                await _context.SaveChangesAsync();

                serviceResponse.Data = await _context.Characters
                    .Where(c => c.User.Id == GetUserId())
                    .Select(c => _mapper.Map<GetCharacterDto>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message.ToString();
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDto>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                Character character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());

                if (character != null)
                {
                    _context.Remove(character);
                    _context.SaveChangesAsync();

                    serviceResponse.Data = _context.Characters
                        .Where(u => u.User.Id == GetUserId())
                        .Select(c => _mapper.Map<GetCharacterDto>(c)).ToList();
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found!";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message.ToString();
            }
            return serviceResponse;
        }


        public async Task<ServiceResponse<List<GetCharacterDto>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDto>>();

            try
            {
                var dbCharacters = await _context.Characters
                    .Where(c => c.User.Id == GetUserId())
                    .ToListAsync();
                serviceResponse.Data = dbCharacters.Select
                    (c => _mapper.Map<GetCharacterDto>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message.ToString();
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {
                var character = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == id && c.User.Id == GetUserId());
                serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message.ToString();
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDto>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDto>();

            try
            {
                Character character = await _context.Characters
                    .Include(c => c.User) //include related objects in case we want to use them
                    .FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if (character.User.Id == GetUserId())
                {
                    character.Name = updatedCharacter.Name;
                    character.Strength = updatedCharacter.Strength;
                    character.Defense = updatedCharacter.Defense;
                    character.Intelligence = updatedCharacter.Intelligence;
                    character.RpgClass = updatedCharacter.RpgClass;

                    await _context.SaveChangesAsync();
                    //_mapper.Map(updatedCharacter, character);

                    serviceResponse.Data = _mapper.Map<GetCharacterDto>(character);
                }
                else
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Character not found";
                }
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message.ToString();
            }
            return serviceResponse;
        }
    }
}
