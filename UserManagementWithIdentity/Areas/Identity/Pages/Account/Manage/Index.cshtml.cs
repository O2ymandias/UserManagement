// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using UserManagementWithIdentity.Models;

namespace UserManagementWithIdentity.Areas.Identity.Pages.Account.Manage
{
	public class IndexModel : PageModel
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public IndexModel(
			UserManager<ApplicationUser> userManager,
			SignInManager<ApplicationUser> signInManager)
		{
			_userManager = userManager;
			_signInManager = signInManager;
		}

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public string Username { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[TempData]
		public string StatusMessage { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		[BindProperty]
		public InputModel Input { get; set; }

		/// <summary>
		///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
		///     directly from your code. This API may change or be removed in future releases.
		/// </summary>
		public class InputModel
		{
			/// <summary>
			///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
			///     directly from your code. This API may change or be removed in future releases.
			/// </summary>
			[Phone]
			[Display(Name = "Phone number")]
			public string PhoneNumber { get; set; }

			[Required]
			[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
			[Display(Name = "First Name")]
			public string FirstName { get; set; }

			[Required]
			[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
			[Display(Name = "Last Name")]
			public string LastName { get; set; }

			[Display(Name = "Profile Picture")]
			public byte[] ProfilePicture { get; set; }
		}

		private async Task LoadAsync(ApplicationUser user)
		{
			var userName = await _userManager.GetUserNameAsync(user);
			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

			Username = userName;

			Input = new InputModel
			{
				FirstName = user.FirstName,
				LastName = user.LastName,
				PhoneNumber = phoneNumber,
				ProfilePicture = user.ProfilePicture
			};
		}

		public async Task<IActionResult> OnGetAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			await LoadAsync(user);
			return Page();
		}

		public async Task<IActionResult> OnPostAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			if (user == null)
			{
				return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			if (!ModelState.IsValid)
			{
				await LoadAsync(user);
				return Page();
			}

			if (user.FirstName != Input.FirstName || user.LastName != Input.LastName)
			{
				user.FirstName = Input.FirstName;
				user.LastName = Input.LastName;
				await _userManager.UpdateAsync(user);
			}

			var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
			if (Input.PhoneNumber != phoneNumber)
			{
				var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
				if (!setPhoneResult.Succeeded)
				{
					StatusMessage = "Unexpected error when trying to set phone number.";
					return RedirectToPage();
				}
			}

			if (Request.Form.Files.Count > 0)
			{
				var picture = Request.Form.Files[0];

				var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
				var pictureExtension = Path.GetExtension(picture.FileName).ToLowerInvariant();
				if (string.IsNullOrEmpty(pictureExtension) || !allowedExtensions.Contains(pictureExtension))
				{
					StatusMessage = "Invalid file type. Only image files are allowed.";
					return RedirectToPage();
				}

				long maxFileSize = 5 * 1024 * 1024;
				if (picture.Length > maxFileSize)
				{
					StatusMessage = "File size exceeds the 5MB limit.";
					return RedirectToPage();
				}

				using var pictureStream = new MemoryStream();
				await picture.CopyToAsync(pictureStream);
				user.ProfilePicture = pictureStream.ToArray();

				await _userManager.UpdateAsync(user);
			}


			await _signInManager.RefreshSignInAsync(user);
			StatusMessage = "Your profile has been updated";
			return RedirectToPage();
		}
	}
}
