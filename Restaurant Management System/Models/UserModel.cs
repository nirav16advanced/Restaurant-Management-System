using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace Restaurant_Management_System.Models
{
    public class UserModel
    {
        public int UserId { get; set; }

        [Display(Name = "User Name:")] //used for dynamic changes in MVC
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter UserName")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "UserName should be min 4 and max 20 characters long")]
        public string UserName { get; set; }


        [Display(Name = "Mobile:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Mobile")]
        [RegularExpression(("^[0-9]{10}$"), ErrorMessage = "Please enter valid Mobile No")]
        public string Mobile { get; set; }

        [Display(Name = "Email:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Email")]
        [RegularExpression(("^[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$"), ErrorMessage = "Enter Valid Email")]
        public string Email { get; set; }

        [Display(Name = "Password:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Password")]
        [StringLength(14, MinimumLength = 8, ErrorMessage = "Password should be min 8 and max 14 characters long")]

        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Please enter Password")]
        [Compare("Password", ErrorMessage = "Password do not match")]
        public string Confirm { get; set; }

        [Display(Name = "Gender:")]
        [Required(ErrorMessage = "Please enter Gender")]
        public string Gender { get; set; }
    }
}