using KCBase.IDogCam.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KCBase.IDogCam.Models 
{
    public class CreateUserViewModel
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public UserRole Role { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Associated Pets")]
        public List<int> SelectedPetIds { get; set; } = new List<int>();

        [Display(Name = "Camera Access")]
        public List<string> SelectedCameraIds { get; set; } = new List<string>();
    }

}
