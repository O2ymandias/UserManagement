using System.ComponentModel.DataAnnotations;

namespace UserManagementWithIdentity.ViewModels
{
    public class UserVM
    {
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public IList<string> Roles { get; set; }
    }
}
