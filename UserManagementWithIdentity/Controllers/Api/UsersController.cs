using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserManagementWithIdentity.Models;
using UserManagementWithIdentity.Utilities;

namespace UserManagementWithIdentity.Controllers.Api
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize(Roles = RoleConstants.Admin)]
	public class UsersController : ControllerBase
	{
		private readonly UserManager<ApplicationUser> _userManager;

		public UsersController(UserManager<ApplicationUser> userManager)
		{
			_userManager = userManager;
		}

		[HttpDelete("{userId}")]
		public async Task<IActionResult> DeleteUser(string userId)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user is null)
				return NotFound();

			var deleteResult = await _userManager.DeleteAsync(user);
			return deleteResult.Succeeded
				? Ok()
				: StatusCode(StatusCodes.Status500InternalServerError);
		}
	}
}
