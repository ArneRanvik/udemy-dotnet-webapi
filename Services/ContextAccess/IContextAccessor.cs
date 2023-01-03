using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace udemy_net_webapi.Services.ContextAccess
{
    public interface IContextAccessor
    {
        Task AddCharacter(Models.Character character);
        Task<List<Models.Character>> GetAllCharacters();
        Task<List<Models.Character>> GetUserCharacters(int userId);
        Task<User> GetUser(int id);
        Task<Models.Character> GetCharacter(int id);
        Task<Models.Character> GetUserCharacter(int id, int userId);
        Task RemoveCharacter(int id);
        Task<Skill> GetSkill(int id);
        Task<bool> CharacterIsOwnedByUser(int characterId, int userId);
        Task<List<Models.Character>> GetCharactersWithIds(List<int> characterIds);
        Task<int> SaveChanges();
        Task AddWeapon(Models.Weapon weapon);
        Task<List<Models.Character>> GetHighscoreList();
    }
}