using AutoMapper;
using dotnetcore6_rpg.Dtos.Character;
using dotnetcore6_rpg.Dtos.Skill;
using dotnetcore6_rpg.Dtos.Weapon;

namespace dotnetcore6_rpg.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Character, GetCharacterDto>();
            CreateMap<AddCharacterDto, Character>();
            CreateMap<UpdateCharacterDto, Character>();
            CreateMap<GetWeaponDto, Weapon>();
            CreateMap<Weapon, GetWeaponDto>();
            CreateMap<Skill, GetSkillDto>();
        }
    }
}
