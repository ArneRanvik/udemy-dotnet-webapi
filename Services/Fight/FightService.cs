using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace udemy_net_webapi.Services.Fight
{
    public class FightService : IFightService
    {
        private readonly IMapper _mapper;
        private readonly IContextAccessor _contextAccess;
        public FightService(IContextAccessor contextAccess, IMapper mapper)
        {
            _contextAccess = contextAccess;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<AttackResultDTO>> WeaponAttack(WeaponAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();

            try
            {
                if (request.AttackerId == request.OpponentId)
                {
                    response.Success = false;
                    response.Message = "Character can not attack itself.";
                    return response;
                }

                // Get attacker
                var attacker = await _contextAccess.GetCharacter(request.AttackerId);

                // Check that attacker has a weapon
                if (attacker.Weapon is null)
                {
                    response.Success = false;
                    response.Message = "Attacker does not have a weapon.";
                    return response;
                }

                // Get opponent
                var opponent = await _contextAccess.GetCharacter(request.OpponentId);

                // Calculate attack damage and damage the opponent
                int damage = WeaponDamage(attacker, opponent);

                // Check fight outcome
                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

                // Save context
                await _contextAccess.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int WeaponDamage(Models.Character attacker, Models.Character opponent)
        {
            int damage = attacker.Weapon!.Damage + (new Random().Next(attacker.Strength));
            damage -= new Random().Next(opponent.Defense);
            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<AttackResultDTO>> SkillAttack(SkillAttackDTO request)
        {
            var response = new ServiceResponse<AttackResultDTO>();

            try
            {
                if (request.AttackerId == request.OpponentId)
                {
                    response.Success = false;
                    response.Message = "Character can not attack itself.";
                    return response;
                }

                // Get attacker
                var attacker = await _contextAccess.GetCharacter(request.AttackerId);

                // Check that attacker has the requested skill
                var skill = attacker.Skills.FirstOrDefault(s => s.Id == request.SkillId);
                if (skill is null)
                {
                    response.Success = false;
                    response.Message = "Attacker does not have the requested skill.";
                    return response;
                }

                // Get opponent
                var opponent = await _contextAccess.GetCharacter(request.OpponentId);

                // Calculate attack damage
                int damage = SkillDamage(attacker, skill, opponent);

                // Check fight outcome
                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                response.Data = new AttackResultDTO
                {
                    Attacker = attacker.Name,
                    Opponent = opponent.Name,
                    AttackerHP = attacker.HitPoints,
                    OpponentHP = opponent.HitPoints,
                    Damage = damage
                };

                await _contextAccess.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private static int SkillDamage(Models.Character attacker, Skill skill, Models.Character opponent)
        {
            int damage = skill.Damage + (new Random().Next(attacker.Intelligence));
            damage -= new Random().Next(opponent.Defense);
            if (damage > 0)
            {
                opponent.HitPoints -= damage;
            }

            return damage;
        }

        public async Task<ServiceResponse<FightResultDTO>> Fight(FightRequestDTO request)
        {
            var response = new ServiceResponse<FightResultDTO>()
            {
                Data = new FightResultDTO()
            };

            try
            {
                // Get list of the fighting characters
                var characters = await _contextAccess.GetCharactersWithIds(request.CharacterIds);

                bool defeated = false;
                while (!defeated)
                {
                    // Go through each character in the list where the character attacks the other, one by one
                    Random r = new Random();
                    foreach (int i in Enumerable.Range(0, characters.Count).OrderBy(x => r.Next()))
                    {
                        var attacker = characters[i];
                        var opponents = characters.Where(c => c.Id != attacker.Id).ToList();
                        var opponent = opponents[new Random().Next(opponents.Count)];

                        int damage = 0;
                        string attackUsed = "";

                        bool useWeapon = new Random().Next(2) == 0;
                        if (useWeapon && attacker.Weapon is not null)
                        {
                            attackUsed = attacker.Weapon.Name;
                            damage = WeaponDamage(attacker, opponent);
                        }
                        else if (!useWeapon && (attacker.Skills.Count > 0))
                        {
                            var skill = attacker.Skills[new Random().Next(attacker.Skills.Count)];
                            attackUsed = skill.Name;
                            
                            // Calculate attack damage
                            damage = SkillDamage(attacker, skill, opponent);
                        }
                        else
                        {
                            response.Data.Log.Add($"{attacker.Name} was not able to attack.");
                            continue;
                        }

                        response.Data.Log
                            .Add($"{attacker.Name} attacks {opponent.Name} using {attackUsed} with {(damage >= 0 ? damage : 0)} damage");

                        if (opponent.HitPoints <= 0)
                        {
                            defeated = true;
                            attacker.Victories++;
                            opponent.Defeats++;
                            response.Data.Log.Add($"{opponent.Name} has been defeated!");
                            response.Data.Log.Add($"{attacker.Name} wins with {attacker.HitPoints} HP left!");
                            break;
                        }
                    }
                }

                characters.ForEach(c => {
                    c.Fights++;
                    c.HitPoints = 100;
                });
                await _contextAccess.SaveChanges();
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message + ex.StackTrace;
            }

            return response;
        }

        public async Task<ServiceResponse<List<HighscoreDTO>>> GetHighscore()
        {
            // Get all highscore list
            var highscoreCharacters = await _contextAccess.GetHighscoreList();

            // Create service response and map to DTO
            var response = new ServiceResponse<List<HighscoreDTO>>()
            {
                Data = highscoreCharacters.Select(c => _mapper.Map<HighscoreDTO>(c)).ToList()
            };
            
            return response;
        }
    }
}