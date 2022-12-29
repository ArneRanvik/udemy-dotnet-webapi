using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace udemy_net_webapi.Services.FightService
{
    public class FightService : IFightService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public FightService(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
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

                var attackerResponse = await _characterService.GetCharacterById(request.AttackerId);
                var attacker = attackerResponse.Data;
                if (attacker!.Weapon is null)
                {
                    response.Success = false;
                    response.Message = "Attacker does not have a weapon.";
                    return response;
                }

                var opponentResponse = await _characterService.GetCharacterById(request.OpponentId);
                var opponent = opponentResponse.Data;
                
                int damage = attacker.Weapon.Damage + (new Random().Next(attacker.Strength));
                damage -= new Random().Next(opponent!.Defense);

                if (damage > 0)
                    opponent.HitPoints -= damage;

                if (opponent.HitPoints <= 0)
                    response.Message = $"{opponent.Name} has been defeated!";

                // Update characters after the fight
                var updatedOpponent = _mapper.Map<UpdateCharacterDTO>(opponent);
                await _characterService.UpdateCharacter(updatedOpponent);
                
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