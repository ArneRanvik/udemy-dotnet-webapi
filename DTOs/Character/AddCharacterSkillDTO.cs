using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.DTOs.Character
{
    public class AddCharacterSkillDTO
    {
        public int CharacterId { get; set; }
        public int SkillId { get; set; }
    }
}