using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StudentCourseManagement.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace StudentCourseManagement.Helpers
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;
        private readonly IMongoCollection<User> _users;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public JwtHelper(IConfiguration config, IHttpContextAccessor contextAccessor)
        {
            _config = config;
            _httpContextAccessor = contextAccessor;

            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _users = database.GetCollection<User>(config["MongoDB:CollectionNames:2"]);

        }

        public string GenerateToken(User user, List<int> audienceKeys)
        {
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_config[$"JwtSettings:Key"]));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            List<string> audiences = new List<string>
            {
                _config["JwtSettings:Audiences:0"],
                _config["JwtSettings:Audiences:1"],
                _config["JwtSettings:Audiences:2"]
            };

            claims.AddRange(audiences.Select(aud => new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Aud, aud)));

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public userData GetCurrentUserData()
        {
            string username = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

            if (username == null)
            {
                return new userData { };
            }

            User user = _users.Find(u => u.Username == username).FirstOrDefault();

            userData currentUserInfo = new userData
            {
                Username = user.Username,
                UserId = user.UserId,
                Identity = user.Role
            };

            return currentUserInfo;
        }

    }
}
