using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [Display(Name = "Username")]
        [RegularExpression("^(?=.{8,20}$)(?![_.])(?!.*[_.]{2})[a-zA-Z0-9._]+(?<![_.])$")]
        [StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 8)]
        public string UserName { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
