using KCBase.IDogCam.Models;
using System.Collections.Generic;

namespace KCBase.IDogCam.Interfaces 
{
    public interface IAuthenticationService
    {
        bool Login(string usernameOrEmail, string password, bool rememberMe);
        void Logout();
        User GetCurrentUser();
        bool IsAuthenticated();
        bool IsInRole(UserRole role);
        bool CanAccessCamera(string cameraId);
        List<string> GetUserCameraIds();
        List<int> GetUserPetIds();
    }
}
