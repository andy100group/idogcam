using IDogCamIntegration.Web.ViewModels;
using System.Collections.Generic;
using System.Web.Mvc; // for SelectListItem

namespace KCBase.IDogCam.Models
{
    public class UserListViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<Pet> AllPets { get; set; } = new List<Pet>();
        public List<Camera> AllCameras { get; set; } = new List<Camera>();
        public string SearchTerm { get; set; }
        public UserRole? FilterRole { get; set; }  // Must be nullable
        public bool? FilterActive { get; set; }    // Must be nullable
    }
}
