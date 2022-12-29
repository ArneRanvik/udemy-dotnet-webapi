using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using udemy_net_webapi.DTOs.Fight;

namespace udemy_net_webapi.Services.FightService
{
    public interface IFightService
    {
        Task<ServiceResponse<AttackResultDTO>> WeaponAttack(WeaponAttackDTO request);
    }
}