﻿using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Loo.Models
{
    public class AuthenticatedUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Company { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [Required]
        public string Phone { get; set; }
        [Required]
        public bool NotificationOptIn { get; set; }
    }
}