using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.DTOs.Skill
{
    public class GetSkillDTO
    {
        public string Name { get; set; } = "";
        public int Damage { get; set; }
    }
}