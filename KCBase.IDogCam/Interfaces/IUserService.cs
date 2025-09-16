using KCBase.IDogCam.Models;
using System.Collections.Generic;


namespace KCBase.IDogCam.Interfaces
{
    public interface IUserService
    {
        UserData LoadUserData();
        void SaveUserData(UserData userData);
        User CreateUser(CreateUserViewModel model);
        User UpdateUser(EditUserViewModel model);
        bool DeleteUser(int userId);
        User GetUserById(int userId);
        User GetUserByUsername(string username);
        User GetUserByEmail(string email);
        User AuthenticateUser(string usernameOrEmail, string password);
        List<User> GetAllUsers();
        List<User> SearchUsers(string searchTerm, UserRole? role, bool? isActive);
        bool ChangePassword(int userId, string newPassword);
        void UpdateLastLogin(int userId);
    }
}
