using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using udemy_net_webapi.Data;

namespace udemy_net_webapi.Services.CharacterService
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly DataContext _context;

        public CharacterService(IMapper mapper, DataContext context)
        {
            _context = context;
            _mapper = mapper;
        }
        
        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var character = _mapper.Map<Character>(newCharacter);

            _context.Characters.Add(character);
            await _context.SaveChangesAsync();

            serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();
            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
                if (character is null)
                    throw new Exception($"Character with Id '{id}' not found.");

                _context.Characters.Remove(character);

                await _context.SaveChangesAsync();
                
                serviceResponse.Data = await _context.Characters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToListAsync();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> GetAllCharacters()
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var dbCharacters = await _context.Characters.ToListAsync();
            serviceResponse.Data = dbCharacters.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();
            var dbCharacter = await _context.Characters.FirstOrDefaultAsync(c => c.Id == id);
            serviceResponse.Data = _mapper.Map<GetCharacterDTO>(dbCharacter);
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            try
            {
                var character = await _context.Characters.FirstOrDefaultAsync(c => c.Id == updatedCharacter.Id);
                if (character is null)
                    throw new Exception($"Character with Id '{updatedCharacter.Id}' not found.");

                _mapper.Map(updatedCharacter, character);
                
                await _context.SaveChangesAsync();
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }
    }
}