using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using udemy_net_webapi.DTOs.Weapon;

namespace udemy_net_webapi.Services.WeaponService
{
    public interface IWeaponService
    {
        Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon);
    }
}