using Microsoft.VisualBasic;
using MongoDB.Driver;
using StudentCourseManagement.Models;
using System.Security.Claims;

namespace StudentCourseManagement.Services
{
    public class AuthService
    {
        private readonly IMongoCollection<User> _users;
        public AuthService(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            var mongoClient = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = mongoClient.GetDatabase(config["MongoDB:DatabaseName"]);
            _users = database.GetCollection<User>(config["MongoDB:CollectionNames:2"]);
        }

        public User Get(string username)
        {
            return _users.Find(u => u.Username == username).FirstOrDefault();
        }
        public userData GetUserData(string username)
        {
            User user = _users.Find(u => u.Username == username || u.UserId == username).FirstOrDefault();
            if (user == null)
            {
                return new userData { };
            }
            userData userInfo = new userData
            {
                Username = user.Username,
                UserId = user.UserId,
                Identity = user.Role,
                Id = user.Id
            };
            return userInfo;
        }

        public bool Validate(string username, string password)
        {
            User u = _users.Find(u => u.Username == username).FirstOrDefault();
            if (u == null)
                return false;
            
            var res = BCrypt.Net.BCrypt.Verify(password, u.Password);

            return res;
        }

        public void UpdateTokens(string username, string token)
        {
            User user = _users.Find(u => u.Username == username).FirstOrDefault();
            user.Token = token;

            _users.ReplaceOne(u => u.Username == user.Username, user);
        }

        public string Add(User newUser)
        {
            if (_users.Find(user => user.Username == newUser.Username || user.UserId == newUser.UserId).FirstOrDefault() != null)
            {
                return "User already Exists";
            }

            string password = newUser.Password;
            newUser.Password = BCrypt.Net.BCrypt.HashPassword(password);

            _users.InsertOne(newUser);
            return "User added successfully!";
        }

        public string Update(string userId, User updatedUser)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(updatedUser.Password);
            updatedUser.Password = hashedPassword;
            User user = _users.Find(u => u.UserId == userId).FirstOrDefault();
            if (user == null)
            {
                return "User does not exists. Provide valid username.";
            }

            foreach (var prop in typeof(User).GetProperties())
            {
                var data = prop.GetValue(updatedUser);
                if (data == null)
                {
                    var orgData = prop.GetValue(user);
                    prop.SetValue(updatedUser, orgData);
                }
            }

            _users.ReplaceOne(u => u.UserId == userId, updatedUser);

            return "User updated successfully.";
        }

        public string Delete(string username)
        {
            try
            {
                var res = _users.DeleteOne(u => u.Username == username || u.UserId == username);
                if (res.DeletedCount == 0)
                {
                    return "User does not exist.";
                }
                return "User deleted successfully.";
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
