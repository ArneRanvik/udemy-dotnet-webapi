using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.DTOs.Fight
{
    public class AttackResultDTO
    {
        public string Attacker { get; set; } = "";
        public string Opponent { get; set; } = "";
        public int AttackerHP { get; set; }
        public int OpponentHP { get; set; }
        public int Damage { get; set; }
    }
}