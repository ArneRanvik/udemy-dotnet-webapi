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
                // Check that user owns the character
                bool isOwner = await _contextAccess.CharacterIsOwnedByUser(id, GetUserId());
                if (!isOwner)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Can not delete, user does not own this character.";
                    return serviceResponse;
                }

                // Remove the character
                await _contextAccess.RemoveCharacter(id);

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
                // Get specific character
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
                await _contextAccess.SaveChanges();
                
                // Return the updated character
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
                // Get current character before update
                var character = await _contextAccess.GetUserCharacter(newCharacterSkill.CharacterId, GetUserId());

                // Get the requested skill from Skills repository
                var skill = await _contextAccess.GetSkill(newCharacterSkill.SkillId);

                // Add skill to character and save tracked changes into context
                character.Skills.Add(skill);
                await _contextAccess.SaveChanges();

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