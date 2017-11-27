using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RegistrationAndLogin.Models
{
    public class UserLogin
    {
        [Required(AllowEmptyStrings = false, ErrorMessage ="Email Id is required.")]
        [Display(Name ="Email Id:")]
        public string EmailId { get; set; }

        [Required(AllowEmptyStrings = false,ErrorMessage ="Password is Required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name ="Remember Me:")]
        public bool RememberMe { get; set; }
    }
}