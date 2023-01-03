using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using udemy_net_webapi.Data;
using udemy_net_webapi.DTOs.Weapon;

namespace udemy_net_webapi.Services.Weapon
{
    public class WeaponService : IWeaponService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContextAccessor _contextAccess;
        public WeaponService(IContextAccessor contextAccess, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _contextAccess = contextAccess;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
        public async Task<ServiceResponse<GetCharacterDTO>> AddWeapon(AddWeaponDTO newWeapon)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                // Get character that weapon will be linked to
                var character = await _contextAccess.GetUserCharacter(newWeapon.CharacterId, GetUserId());

                // Define the weapon
                var weapon = _mapper.Map<Models.Weapon>(newWeapon);
                weapon.Character = character;

                // Add weapon to the database, database will be saved here
                await _contextAccess.AddWeapon(weapon);

                response.Data = _mapper.Map<GetCharacterDTO>(await _contextAccess.GetCharacter(newWeapon.CharacterId));
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