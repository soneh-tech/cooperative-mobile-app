using Microsoft.AspNetCore.Mvc;

namespace CooperativeAppAPI.Controllers
{
    public class StateController(IStatesService states) : ControllerBase
    {
        [HttpGet]
        [Route("api/getStates")]
        public async Task<IActionResult> GetAccountTypes()
        {
            var result = await states.GetStatesAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }
    }
}
