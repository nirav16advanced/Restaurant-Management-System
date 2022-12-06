using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Restaurant_Management_System.Models
{
    public class LoginModel
    {
        [Display(Name = "User Name")] //used for dynamic changes in MVC
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter UserName")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "UserName should be min 4 and max 20 characters long")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Password")]
        

        public string Password { get; set; }
    }
}