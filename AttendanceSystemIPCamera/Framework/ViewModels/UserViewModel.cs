using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AttendanceSystemIPCamera.Framework.ViewModels
{
    public class UserViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
    }
    public class UserAuthentication
    {
        [Required]
        public string FirebaseToken { get; set; }
    }

    public class AuthorizedUser
    {
        public UserViewModel User { get; set; }
        public string[] Roles { get; set; }
        public string AccessToken { get; set; }
    }

    public class SupervisorInfo
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Image { get; set; }
    }

}
