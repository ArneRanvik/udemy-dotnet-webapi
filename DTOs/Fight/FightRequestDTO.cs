using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.DTOs.Fight
{
    public class FightRequestDTO
    {
        public List<int> CharacterIds { get; set; } = new List<int>();
    }
}