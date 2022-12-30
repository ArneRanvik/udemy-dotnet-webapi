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

                // Calculate attack damage
                int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                damage -= new Random().Next(opponent.Defense);

                // Perform damage on opponent
                opponent = await _contextAccess.DamageCharacter(opponent.Id, damage);

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
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}