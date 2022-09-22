using AutoMapper;
using dotnetcore6_rpg.Data;
using dotnetcore6_rpg.Dtos.Fights;
using Microsoft.EntityFrameworkCore;

namespace dotnetcore6_rpg.Service.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public FightService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AttackResultDto>> WeaponAttack(WeaponAttackDto weaponAttack)
        {
            var serviceResponse = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Weapon)
                    .FirstOrDefaultAsync(c => c.Id == weaponAttack.AttackerId);

                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == weaponAttack.OpponentId);

                int damage = DoWeaponAttack(attacker, opponent);
                if(opponent.HitPoints <= 0)
                {
                    serviceResponse.Message = $"{opponent.Name} has been defeated!";
                }
                await _context.SaveChangesAsync();
                serviceResponse.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent= opponent.Name,
                    AttackerHP= attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage= damage
                };

            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }


        public async Task<ServiceResponse<AttackResultDto>> SkillAttack(SkillAttackDto skillAttack)
        {
            var serviceResponse = new ServiceResponse<AttackResultDto>();
            try
            {
                var attacker = await _context.Characters
                    .Include(c => c.Skills)
                    .FirstOrDefaultAsync(c => c.Id == skillAttack.AttackerId);

                var opponent = await _context.Characters
                    .FirstOrDefaultAsync(c => c.Id == skillAttack.OpponentId);

                var skill = attacker.Skills.FirstOrDefault(s => s.Id == skillAttack.SkillId);

                if (skill == null)
                {
                    serviceResponse.Success=false;
                    serviceResponse.Message = $"{attacker.Name} doesn't know that skill!";
                    return serviceResponse;
                }

                int damage = DoSkillAttack(attacker, opponent, skill);
                if (opponent.HitPoints <= 0)
                {
                    serviceResponse.Message = $"{opponent.Name} has been defeated!";
                }
                await _context.SaveChangesAsync();
                serviceResponse.Data = new AttackResultDto
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public async Task<ServiceResponse<FightResultDto>> Fight(FightRequestDto fightRequest)
        {
            var serviceResponse = new ServiceResponse<FightResultDto>
            {
                Data = new FightResultDto()
            };

            try
            {
                var characters = await _context.Characters
                    .Include(c=>c.Weapon)
                    .Include(c=>c.Skills)
                    .Where(c=> fightRequest.CharacterIds.Contains(c.Id)).ToListAsync();

                bool defated = false;
                while (!defated)
                {
                    foreach(Character attacker in characters)
                    {
                        var opponents = characters.Where(c=>c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = string.Empty;

                        bool useWeapon = new Random().Next(2) == 0;

                        if (useWeapon)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = DoWeaponAttack(attacker,opponent);
                        }
                        else
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            damage = DoSkillAttack(attacker, opponent, skill);
                        }
                        serviceResponse.Data.Log.Add
                            ($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with damage {(damage >= 0 ? damage : 0)} damage");

                        if (opponent.HitPoints <=0)
                        {
                            defated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            serviceResponse.Data.Log.Add($"{opponent.Name} has been defeated!");
                            serviceResponse.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }

                }
                ///reset characters
                characters.ForEach(c =>
                {
                    c.Fights++;
                    c.HitPoints = 100;
                });
                _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }

        public static int DoWeaponAttack(Character? attacker, Character? opponent)
        {
            int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }
            return damage;
        }

        public static int DoSkillAttack(Character? attacker, Character? opponent, Skill? skill)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);

            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }
            return damage;
        }

        public async Task<ServiceResponse<List<HighScoreDto>>> GetHighScore()
        {
            var serviceResponse = new ServiceResponse<List<HighScoreDto>>();

            try
            {
                var characters = await _context.Characters
                    .Where(c => c.Fights > 0)
                    .OrderByDescending(c => c.Victories)
                    .ThenBy(c => c.Defeats)
                    .ToListAsync();
                serviceResponse.Data = characters.Select(c => _mapper.Map<HighScoreDto>(c)).ToList();

                return serviceResponse;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            return serviceResponse;
        }
    }
}
