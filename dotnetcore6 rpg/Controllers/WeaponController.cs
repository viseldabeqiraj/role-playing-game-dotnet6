using dotnetcore6_rpg.Dtos.Character;
using dotnetcore6_rpg.Dtos.Weapon;
using dotnetcore6_rpg.Service.WeaponService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnetcore6_rpg.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WeaponController : ControllerBase
    {
        private readonly IWeaponService _weaponService;

        public WeaponController(IWeaponService weaponService)
        {
            _weaponService = weaponService;
        }

        [HttpPost]

        public async Task<ActionResult<ServiceResponse<GetCharacterDto>>> AddWeapon(AddWeaponDto newWeapon)
        {
            try
            {
                return Ok(_weaponService.AddWeapon(newWeapon));
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
