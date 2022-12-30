using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.Services.ContextAccess
{
    public interface IContextAccessor
    {
        Task AddCharacter(Models.Character character);
        Task<List<Models.Character>> GetUserCharacters(int userId);
        Task<User> GetUser(int id);
        Task<Models.Character> GetUserCharacter(int id, int userId);
        Task<Models.Character> GetCharacter(int id);
        Task RemoveCharacter(Models.Character character);
        Task UpdateCharacter(Models.Character updatedCharacter);
        Task<Skill> GetSkill(int id);
        Task AddSkillToCharacterWithId(int skillId, int characterId);
        Task<bool> CharacterIsOwnedByUser(int characterId, int userId);
        Task AddWeaponToCharacter(Models.Weapon weapon);
        Task<Models.Character> DamageCharacter(int characterId, int damage);
    }
}