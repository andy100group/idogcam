using KCBase.IDogCam.Models;
using System;
using System.Collections.Generic;

namespace IDogCamIntegration.Web.ViewModels
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public List<int> PetIds { get; set; } = new List<int>();
    }
}