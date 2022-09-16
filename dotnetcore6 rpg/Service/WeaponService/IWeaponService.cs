using dotnetcore6_rpg.Dtos.Character;
using dotnetcore6_rpg.Dtos.Weapon;

namespace dotnetcore6_rpg.Service.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDto>> AddWeapon(AddWeaponDto weapon);
    }
}
