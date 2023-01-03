using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace udemy_net_webapi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FightController : ControllerBase
    {
        private readonly IFightService _fightService;
        public FightController(IFightService fightService)
        {
            _fightService = fightService;
        }

        [HttpPost("Weapon")]
        public async Task<ActionResult<ServiceResponse<AttackResultDTO>>> WeaponAttack(WeaponAttackDTO request)
        {
            return Ok(await _fightService.WeaponAttack(request));
        }
        [HttpPost("Skill")]
        public async Task<ActionResult<ServiceResponse<AttackResultDTO>>> SkillAttack(SkillAttackDTO request)
        {
            return Ok(await _fightService.SkillAttack(request));
        }
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<FightResultDTO>>> Fight(FightRequestDTO request)
        {
            return Ok(await _fightService.Fight(request));
        }
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<HighscoreDTO>>>> GetHighscore()
        {
            return Ok(await _fightService.GetHighscore());
        }
    }
}