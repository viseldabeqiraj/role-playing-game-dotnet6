using dotnetcore6_rpg.Dtos.Character;
using dotnetcore6_rpg.Service.CharacterService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace dotnetcore6_rpg.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ICharacterService _characterService;

        public CharacterController(ICharacterService characterService)
        {
            _characterService = characterService;
        }
        //[AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> Get()
        {
            //int UserId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value);
            return Ok(_characterService.GetAllCharacters());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> GetSingle(int id)
        {
            return Ok(_characterService.GetCharacterById(id));
        }

        [HttpPost("CreateCharacter")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacter(AddCharacterDto newCharacter)
        {          
            return Ok(_characterService.AddCharacter(newCharacter));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> UpdateCharacter(UpdateCharacterDto updatedCharacter)
        {
            var response = await _characterService.UpdateCharacter(updatedCharacter);
            if(response.Data != null)
            {
                return Ok(response);

            }
            else return NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> DeleteCharacter(int id)
        {
            var response = await _characterService.DeleteCharacter(id);
            if (response.Data != null)
            {
                return Ok(response);

            }
            else return NotFound();
        }

        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<List<GetCharacterDto>>>> AddCharacterSkill(AddCharacterSkillDto newCharacterSkill)
        {
            try
            {
                return Ok(_characterService.AddCharacterSkill(newCharacterSkill));

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

     
    }
}
