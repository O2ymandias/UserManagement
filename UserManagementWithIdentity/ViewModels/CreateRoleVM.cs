using System.ComponentModel.DataAnnotations;

namespace UserManagementWithIdentity.ViewModels
{
    public class CreateRoleVM
    {

        [Required, StringLength(256), Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
