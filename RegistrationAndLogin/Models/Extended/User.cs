using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace RegistrationAndLogin.Models
{
    [MetadataType(typeof(UserMetaData))]
    public partial class User
    {
        public string ConfirmPassword { get; set; }
    }
    public class UserMetaData
    {
        [Display(Name ="First Name:")]
        [Required(AllowEmptyStrings = false, ErrorMessage ="Please Enter FirstName.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter LastName.")]
        public string LastName { get; set; }

        [Display(Name = "Email Address:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter EmailID.")]
        [DataType(DataType.EmailAddress)]
        public string EmailID { get; set; }

        [Display(Name = "Date Of Birth:")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Password:")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please Enter Password.")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Password should contain minimum 6 characters.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password:")]
        [Compare("Password",ErrorMessage ="Confirm Password and Password must be same.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}