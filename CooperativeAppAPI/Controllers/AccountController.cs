namespace CooperativeAppAPI.Controllers
{
    [ApiController]
    public class AccountController(IAccountService account) : ControllerBase
    {
        [HttpPost]
        [Route("api/login")]
        public async Task<IActionResult> Authenticate(UserDto user)
        {
            var result = await account.AuthenticateUser(user.username, user.password);
            return result.Status is 200 ? Ok(result) : result.Status is 404 ? NotFound() : BadRequest(result);
        }
        [HttpPost]
        [Route("api/register")]
        public async Task<IActionResult> Register(Member member, Staff staff)
        {
            var result = await account.Register(member, staff);
            return result.Status is 200 ? Ok(result) : BadRequest(result);
        }
    }
}
