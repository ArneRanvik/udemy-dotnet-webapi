using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.DTOs.Fight
{
    public class WeaponAttackDTO
    {
        public int AttackerId { get; set; }
        public int OpponentId { get; set; }
    }
}