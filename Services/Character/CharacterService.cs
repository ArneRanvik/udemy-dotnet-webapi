using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using udemy_net_webapi.Data;
using udemy_net_webapi.Services.ContextAccess;

namespace udemy_net_webapi.Services.Character
{
    public class CharacterService : ICharacterService
    {
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IContextAccessor _contextAccess;

        public CharacterService(IMapper mapper, IContextAccessor contextAccess, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _contextAccess = contextAccess;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext!.User
            .FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        public async Task<ServiceResponse<List<GetCharacterDTO>>> AddCharacter(AddCharacterDTO newCharacter)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();
            var character = _mapper.Map<Models.Character>(newCharacter);

            try
            {
                // Get user with id
                character.User = await _contextAccess.GetUser(GetUserId());

                // Add new character
                await _contextAccess.AddCharacter(character);

                // Get list of all characters belonging to user with ID
                var characterList = await _contextAccess.GetUserCharacters(GetUserId());
                serviceResponse.Data = characterList!.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<GetCharacterDTO>>> DeleteCharacter(int id)
        {
            var serviceResponse = new ServiceResponse<List<GetCharacterDTO>>();

            try
            {
                // Get specific character that belongs to user
                var character = await _contextAccess.GetUserCharacter(id, GetUserId());
                if (character is null)
                    throw new Exception($"Character with Id '{id}' not found.");

                // Remove character
                await _contextAccess.RemoveCharacter(character);

                // Get list of all characters belonging to user with ID
                var characterList = await _contextAccess.GetUserCharacters(GetUserId());
                serviceResponse.Data = characterList!.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
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

            try
            {
                // Get list of all characters belonging to user with ID
                var characterList = await _contextAccess.GetUserCharacters(GetUserId());
                serviceResponse.Data = characterList.Select(c => _mapper.Map<GetCharacterDTO>(c)).ToList();
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> GetCharacterById(int id)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            try
            {
                // Get specific character that belongs to user
                var character = await _contextAccess.GetUserCharacter(id, GetUserId());
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }
            
            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> UpdateCharacter(UpdateCharacterDTO updatedCharacter)
        {
            var serviceResponse = new ServiceResponse<GetCharacterDTO>();

            try
            {
                // Get current character before update
                var character = await _contextAccess.GetUserCharacter(updatedCharacter.Id, GetUserId());
                
                // Update the character
                _mapper.Map(updatedCharacter, character);
                await _contextAccess.UpdateCharacter(character);
                
                serviceResponse.Data = _mapper.Map<GetCharacterDTO>(character);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = ex.Message;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<GetCharacterDTO>> AddCharacterSkill(AddCharacterSkillDTO newCharacterSkill)
        {
            var response = new ServiceResponse<GetCharacterDTO>();
            try
            {
                // Check that user owns the character
                bool ownedByUser = await _contextAccess.CharacterIsOwnedByUser(newCharacterSkill.CharacterId, GetUserId());
                if (!ownedByUser)
                {
                    response.Success = false;
                    response.Message = "User does not own the character.";
                    return response;
                }

                // Add skill to the character
                await _contextAccess.AddSkillToCharacterWithId(newCharacterSkill.SkillId, newCharacterSkill.CharacterId);

                // Respond with character with the added skill
                response.Data = _mapper.Map<GetCharacterDTO>(await _contextAccess.GetCharacter(newCharacterSkill.CharacterId));
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