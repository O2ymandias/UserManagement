using System.ComponentModel.DataAnnotations;

namespace UserManagementWithIdentity.ViewModels
{
	public class EditUserVM
	{
		public string UserId { get; set; }

		[Required]
		[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }


		[Required]
		[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required]
		[Display(Name = "User Name")]
		public string UserName { get; set; }


		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string Email { get; set; }

		public List<RoleVM> Roles { get; set; }
	}
}
