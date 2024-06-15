namespace CooperativeAppAPI.Controllers
{
    [ApiController]
    public class DashboardController(IDashboardService dashboard) : ControllerBase
    {
        [HttpGet]
        [Route("api/getDailyLoans")]
        public async Task< IActionResult> DailyLoan()
        {
            try
            {
                return Ok(await dashboard.GetDailyLoans());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/getDailyDeposits")]
        public async Task<IActionResult> DailyDeposit()
        {
            try
            {
                return Ok(await dashboard.GetDailyDeposits());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/getDailyMembers")]
        public async Task<IActionResult> DailyMembers()
        {
            try
            {
                return Ok(await dashboard.GetDailyMembers());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/getDailyWithdrawals")]
        public async Task<IActionResult> DailyWithdrawals()
        {
            try
            {
                return Ok(await dashboard.GetDailyWithdrawals());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/getTotalLoans")]
        public async Task<IActionResult> TotalLoans()
        {
            try
            {
                return Ok(await dashboard.GetTotalLoans());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet]
        [Route("api/getTotalDeposits")]
        public async Task<IActionResult> TotalDeposits()
        {
            try
            {
                return Ok(await dashboard.GetTotalDeposits());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/getTotalMembers")]
        public async Task<IActionResult> TotalMembers()
        {
            try
            {
                return Ok(await dashboard.GetTotalMembers());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/getTotalWithrawals")]
        public async Task<IActionResult> TotalWithdrawals()
        {
            try
            {
                return Ok(await dashboard.GetTotalWithdrawals());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
