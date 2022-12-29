using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.DTOs.Weapon
{
    public class AddWeaponDTO
    {
        public string Name { get; set; } = "";
        public int Damage { get; set; }
        public int CharacterId { get; set; }
    }
}