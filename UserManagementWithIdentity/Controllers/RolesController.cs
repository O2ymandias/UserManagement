using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementWithIdentity.Utilities;
using UserManagementWithIdentity.ViewModels;

namespace UserManagementWithIdentity.Controllers
{
    [Authorize(Roles = RoleConstants.Admin)]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRoleVM model)
        {
            if (ModelState.IsValid)
            {
                bool isRoleExists = await _roleManager.RoleExistsAsync(model.RoleName.Trim());

                if (!isRoleExists)
                    await _roleManager.CreateAsync(new IdentityRole() { Name = model.RoleName.Trim() });
                else
                    ModelState.AddModelError(nameof(CreateRoleVM.RoleName), "Role Already Exists!");

            }
            return View(nameof(Index), await _roleManager.Roles.ToListAsync());
        }
    }
}
