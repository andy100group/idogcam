using KCBase.IDogCam.Models;
using System.Collections.Generic;

namespace KCBase.IDogCam.Models
{
    public class UserData
    {
        public List<User> Users { get; set; } = new List<User>();
        public List<UserCameraAccess> CameraAccess { get; set; } = new List<UserCameraAccess>();
    }

}