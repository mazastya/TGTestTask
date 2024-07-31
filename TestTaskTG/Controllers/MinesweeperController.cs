using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TestTaskTG.Models;
using TestTaskTG.Services;

namespace TestTaskTG.Controllers
{
    [ApiController]
    public class MinesweeperController(MinesweeperService minesweeperService) : ControllerBase
    {
        [HttpPost("new")]
        public ActionResult<GameInfoResponse> CreateGame(NewGameRequest newGameRequest)
        {
            try
            {
                return Ok(JsonConvert.SerializeObject(minesweeperService.NewGame(newGameRequest)));
            }
            catch (Exception ex)
            {
                return BadRequest(JsonConvert.SerializeObject(new ErrorResponse(ex.Message)));
            }
        }

        [HttpPost("turn")]
        public Task<ActionResult<GameInfoResponse>> Turn(GameTurnRequest gameTurnRequest)
        {
            try
            {
                return Task.FromResult<ActionResult<GameInfoResponse>>(Ok(JsonConvert.SerializeObject(minesweeperService.Turn(gameTurnRequest))));
            }
            catch (Exception ex)
            {
                return Task.FromResult<ActionResult<GameInfoResponse>>(BadRequest(JsonConvert.SerializeObject(new ErrorResponse(ex.Message))));
            }
        }
    }
}