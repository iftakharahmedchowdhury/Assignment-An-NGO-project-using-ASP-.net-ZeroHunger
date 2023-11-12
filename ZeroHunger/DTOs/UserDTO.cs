using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZeroHunger.DTOs
{
    public class UserDTO
    {
        public int UserID { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ContactInfo { get; set; }

        [Required]
        public string Location { get; set; }
    }
}
