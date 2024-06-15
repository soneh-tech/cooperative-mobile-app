using Microsoft.AspNetCore.Mvc;

namespace CooperativeAppAPI.Controllers
{
    public class AccountTypeController(IAccountTypeService account_type) : ControllerBase
    {
        [HttpGet]
        [Route("api/getAccountTypes")]
        public async Task<IActionResult> GetAccountTypes()
        {
            var result = await account_type.GetAccountTypeAsync();
            return result is not null ? Ok(result) : BadRequest(result);
        }

        [HttpGet]
        [Route("api/getAccountType{id}")]
        public async Task<IActionResult> GetAccountType(int id)
        {
            var result = await account_type.GetAccountTypeAsync(id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
        [HttpGet]
        [Route("api/getAccountTypeByMember/{member_id}")]
        public async Task<IActionResult> GetAccountTypeMember(int member_id)
        {
            var result = await account_type.GetAccountTypeByMember(member_id);
            return result is not null ? Ok(result) : BadRequest(result);
        }
    }
}
