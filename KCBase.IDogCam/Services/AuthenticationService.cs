using KCBase.IDogCam.Interfaces;
using KCBase.IDogCam.Models;
using System;
using System.Collections.Generic;
using System.Web;

namespace KCBase.IDogCam.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;
        private const string USER_SESSION_KEY = "CurrentUser";

        public AuthenticationService(IUserService userService)
        {
            _userService = userService;
        }

        public bool Login(string usernameOrEmail, string password, bool rememberMe)
        {
            var user = _userService.AuthenticateUser(usernameOrEmail, password);
            if (user != null)
            {
                HttpContext.Current.Session[USER_SESSION_KEY] = user;
                _userService.UpdateLastLogin(user.Id);

                // Set authentication cookie if remember me
                if (rememberMe)
                {
                    var cookie = new HttpCookie("IDogCamAuth", user.Id.ToString())
                    {
                        Expires = DateTime.Now.AddDays(30),
                        HttpOnly = true
                    };
                    HttpContext.Current.Response.Cookies.Add(cookie);
                }

                return true;
            }
            return false;
        }

        public void Logout()
        {
            HttpContext.Current.Session.Remove(USER_SESSION_KEY);

            // Remove authentication cookie
            var cookie = new HttpCookie("IDogCamAuth")
            {
                Expires = DateTime.Now.AddDays(-1)
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public User GetCurrentUser()
        {
            // First check session
            var user = HttpContext.Current.Session[USER_SESSION_KEY] as User;
            if (user != null)
            {
                return user;
            }

            // Check remember me cookie
            var authCookie = HttpContext.Current.Request.Cookies["IDogCamAuth"];
            if (authCookie != null && int.TryParse(authCookie.Value, out int userId))
            {
                user = _userService.GetUserById(userId);
                if (user != null && user.IsActive)
                {
                    HttpContext.Current.Session[USER_SESSION_KEY] = user;
                    return user;
                }
            }

            return null;
        }

        public bool IsAuthenticated()
        {
            return GetCurrentUser() != null;
        }

        public bool IsInRole(UserRole role)
        {
            var user = GetCurrentUser();
            return user != null && user.Role >= role;
        }

        public bool CanAccessCamera(string cameraId)
        {
            var user = GetCurrentUser();
            if (user == null)
            {
                return false;
            }

            // Admins and employees can access all cameras
            if (user.Role >= UserRole.Employee)
            {
                return true;
            }

            // Customers can access cameras they're explicitly granted access to
            return user.CameraIds.Contains(cameraId);
        }

        public List<string> GetUserCameraIds()
        {
            var user = GetCurrentUser();
            return user?.CameraIds ?? new List<string>();
        }

        public List<int> GetUserPetIds()
        {
            var user = GetCurrentUser();
            return user?.PetIds ?? new List<int>();
        }
    }
}