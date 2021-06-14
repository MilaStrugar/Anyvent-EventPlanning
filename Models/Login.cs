using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email:")]
        public string LoginEmail { get; set; }

        [DataType(DataType.Password)]
        [Required]
        [Display(Name = "Password:")]
        public string LoginPassword { get; set; }
    }
}