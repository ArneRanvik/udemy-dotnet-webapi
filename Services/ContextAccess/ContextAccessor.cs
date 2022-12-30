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

        public async Task<Models.Character> GetUserCharacter(int id, int userId)
        {
            var character = await _context.Characters
                .Include(c => c.User)
                .Include(c => c.Weapon)
                .Include(c => c.Skills)
                .FirstOrDefaultAsync(c => (c.Id == id) && (c.User!.Id == userId));
            if (character is null)
                throw new Exception("User does not have this character.");
            return character;
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

        public async Task RemoveCharacter(Models.Character character)
        {
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCharacter(Models.Character updatedCharacter)
        {
            // Get the current character entry
            var character = await _context.Characters.FirstOrDefaultAsync(c => (c.Id == updatedCharacter.Id));
            character = updatedCharacter;
            await _context.SaveChangesAsync();
        }

        public async Task<Skill> GetSkill(int id)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(s => s.Id == id);
            if (skill is null)
                throw new Exception("Skill does not exist.");
            return skill;
        }

        public async Task AddSkillToCharacterWithId(int skillId, int characterId)
        {
            Skill skill = await GetSkill(skillId);
            Models.Character character = await GetCharacter(characterId);
            character.Skills!.Add(skill);
            await _context.SaveChangesAsync();
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

        public async Task AddWeaponToCharacter(Models.Weapon weapon)
        {
            // Get the character
            var character = await GetCharacter(weapon.CharacterId);
            weapon.Character = character;

            // Add the weapon to the database
            _context.Weapons.Add(weapon);
            await _context.SaveChangesAsync();
        }

        public async Task<Models.Character> DamageCharacter(int characterId, int damage)
        {
            var character = await GetCharacter(characterId);
            if (damage > 0)
                character.HitPoints -= damage;
            
            await _context.SaveChangesAsync();
            return character;
        }
    }
}