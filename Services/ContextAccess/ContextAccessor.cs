using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace udemy_net_webapi.Services.ContextAccess
{
    public class ContextAccessor : IContextAccessor
    {
        private readonly DataContext _context;
        public ContextAccessor(DataContext context)
        {
            _context = context;
        }
        public async Task AddCharacter(Models.Character character)
        {
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Models.Character>> GetUserCharacters(int userId)
        {
            try
            {
                return await _context.Characters
                    .Include(c => c.User)
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills)
                    .Where(c => c.User!.Id == userId).ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Unable to get list of characters, user may not have any.");
            }
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is null)
                throw new Exception($"Requested user with id '{id}' does not exist.");
            return user;
        }

        public async Task<Models.Character> GetCharacter(int id)
        {
            var character = await _context.Characters
                .Include(c => c.User)
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => (c.Id == id));
            if (character is null)
                throw new Exception("Character does not exist.");
            return character;
        }

        public async Task<Models.Character> GetUserCharacter(int id, int userId)
        {
            var character = await _context.Characters
                .Include(c => c.User)
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => (c.Id == id) && (c.User!.Id == userId));
            if (character is null)
                throw new Exception("Character does not exist. Or user does not own the character.");
            return character;
        }

        public async Task RemoveCharacter(int id)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            if (character is not null)
            {
                _context.Characters.Remove(character);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Skill> GetSkill(int id)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);
            if (skill is null)
                throw new Exception("Skill does not exist.");
            return skill;
        }

        public async Task<bool> CharacterIsOwnedByUser(int characterId, int userId)
        {
            var character = await _context.Characters
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == characterId);
            if (character is null || character.User!.Id != userId)
                return false;
            return true;
        }

        public async Task<List<Models.Character>> GetCharactersWithIds(List<int> characterIds)
        {
            var characters = await _context.Characters
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .Where(c => characterIds.Contains(c.Id)).ToListAsync();
            if (characters is null)
                characters = new List<Models.Character>();
            
            return characters;
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task AddWeapon(Models.Weapon weapon)
        {
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Models.Character>> GetHighscoreList()
        {
            var characters = await _context.Characters
                .Where(c => c.Fights > 0)
                .OrderByDescending(c => c.Victories)
                .ThenBy(c => c.Defeats)
                .ToListAsync();
            return characters;
        }

        public async Task<List<Models.Character>> GetAllCharacters()
        {
            try
            {
                return await _context.Characters
                    .Include(c => c.User)
                    .Include(c => c.Weapon)
                    .Include(c => c.Skills).ToListAsync();
            }
            catch (Exception)
            {
                throw new Exception("Not able to get list of all characters.");
            }
        }
    }
}