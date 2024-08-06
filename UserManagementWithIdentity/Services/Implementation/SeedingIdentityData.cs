
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using UserManagementWithIdentity.Models;
using UserManagementWithIdentity.Services.Contract;
using UserManagementWithIdentity.Utilities;

namespace UserManagementWithIdentity.Services.Implementation
{
	public class SeedingIdentityData : ISeedingIdentityData
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly Admin _admin;

		public SeedingIdentityData(UserManager<ApplicationUser> userManager,
			RoleManager<IdentityRole> roleManager,
			IOptions<Admin> adminOptions)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_admin = adminOptions.Value;
		}
		public async Task SeedDataAsync()
		{
			// Seeding Roles
			if (!_roleManager.Roles.Any())
			{
				await _roleManager.CreateAsync(new IdentityRole() { Name = RoleConstants.Admin });
				await _roleManager.CreateAsync(new IdentityRole() { Name = RoleConstants.User });
			}

			// Seeding Admin
			if (!_userManager.Users.Any())
			{
				var admin = new ApplicationUser()
				{
					FirstName = _admin.FirstName,
					LastName = _admin.LastName,
					Email = _admin.Email,
					UserName = _admin.UserName,
					PhoneNumber = _admin.PhoneNumber
				};
				await _userManager.CreateAsync(admin, _admin.Password);
				await _userManager.AddToRoleAsync(admin, RoleConstants.Admin);
			}
		}
	}
}
