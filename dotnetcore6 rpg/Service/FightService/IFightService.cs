using dotnetcore6_rpg.Dtos.Fights;

namespace dotnetcore6_rpg.Service.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack);
        Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto weaponAttack);
        Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest);
        Task<ServiceResponse<List<HighScoreDto>>> GetHighScore();
    }
}
