using KCBase.IDogCam.Interfaces;
using KCBase.IDogCam.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace KCBase.IDogCam.Services 
{
    public class UserService : IUserService
    {
        private readonly string _userDataPath;

        public UserService()
        {
            _userDataPath = HttpContext.Current?.Server.MapPath("~/App_Data/user_data.xml")
                           ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "user_data.xml");

            // Ensure directory exists
            var directory = Path.GetDirectoryName(_userDataPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Initialize with default admin user if file doesn't exist
            if (!File.Exists(_userDataPath))
            {
                InitializeDefaultUsers();
            }
        }
        private string HashPassword(string password)
        {
            using (var sha256 = new System.Security.Cryptography.SHA256Managed())
            {
                var salt = "iDogCam_Salt_2024";
                var inputBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                var hashedBytes = sha256.ComputeHash(inputBytes);

                // Convert to hex string
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashedBytes.Length; i++)
                {
                    sb.Append(hashedBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
        public User AuthenticateUser(string usernameOrEmail, string password)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            throw new NotImplementedException();
        }

        public User CreateUser(CreateUserViewModel model)
        {
            throw new NotImplementedException();
        }

        public bool DeleteUser(int userId)
        {
            throw new NotImplementedException();
        }

        public List<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public User GetUserByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public User GetUserById(int userId)
        {
            throw new NotImplementedException();
        }

        public User GetUserByUsername(string username)
        {
            throw new NotImplementedException();
        }

        public UserData LoadUserData()
        {
            try
            {
                if (!File.Exists(_userDataPath))
                {
                    return new UserData();
                }

                var serializer = new XmlSerializer(typeof(UserData));
                using (var reader = new FileStream(_userDataPath, FileMode.Open))
                {
                    return (UserData)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading user data: {ex.Message}");
                return new UserData();
            }
        }

        public void SaveUserData(UserData userData)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(UserData));
                using (var writer = new FileStream(_userDataPath, FileMode.Create))
                {
                    serializer.Serialize(writer, userData);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving user data: {ex.Message}");
                throw;
            }
        }

        public List<User> SearchUsers(string searchTerm, UserRole? role, bool? isActive)
        {
            throw new NotImplementedException();
        }

        public void UpdateLastLogin(int userId)
        {
            throw new NotImplementedException();
        }

        public User UpdateUser(EditUserViewModel model)
        {
            throw new NotImplementedException();
        }

        private void InitializeDefaultUsers()
        {
            var userData = new UserData();

            // Create default admin user
            var adminUser = new User
            {
                Id = 1,
                Username = "admin",
                Email = "admin@idogcam.com",
                PasswordHash = HashPassword("admin123"),
                FirstName = "System",
                LastName = "Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            // Create sample customer users tied to test pets
            var customer1 = new User
            {
                Id = 2,
                Username = "smith",
                Email = "smith@example.com",
                PasswordHash = HashPassword("password123"),
                FirstName = "John",
                LastName = "Smith",
                Role = UserRole.Customer,
                IsActive = true,
                CreatedDate = DateTime.Now,
                PetIds = new List<int> { 1 } // Rex
            };

            var customer2 = new User
            {
                Id = 3,
                Username = "johnson",
                Email = "johnson@example.com",
                PasswordHash = HashPassword("password123"),
                FirstName = "Mary",
                LastName = "Johnson",
                Role = UserRole.Customer,
                IsActive = true,
                CreatedDate = DateTime.Now,
                PetIds = new List<int> { 2 } // Bella
            };

            var employee1 = new User
            {
                Id = 4,
                Username = "employee",
                Email = "employee@idogcam.com",
                PasswordHash = HashPassword("employee123"),
                FirstName = "Staff",
                LastName = "Member",
                Role = UserRole.Employee,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            userData.Users.AddRange(new[] { adminUser, customer1, customer2, employee1 });
        }
    }
    }

