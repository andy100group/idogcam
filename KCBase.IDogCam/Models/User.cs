using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCBase.IDogCam.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        public UserRole Role { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        // Pet associations for customers
        public List<int> PetIds { get; set; } = new List<int>();

        // Direct camera access permissions
        public List<string> CameraIds { get; set; } = new List<string>();

        public string FullName => $"{FirstName} {LastName}";
    }
}