﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Areas.Users.ViewModels.Passwords
{
    public class CreateViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
