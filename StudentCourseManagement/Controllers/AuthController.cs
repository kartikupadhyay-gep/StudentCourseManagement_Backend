using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services;

namespace StudentCourseManagement.Controllers
{
    [Route("api/auth/users")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly JwtHelper _jwtHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(AuthService authService, JwtHelper jwtHelper, IHttpContextAccessor contextAccessor)
        {
            _authService = authService;
            _jwtHelper = jwtHelper;
            _httpContextAccessor = contextAccessor;
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest req)
        {
            if (req.Username == null || req.Password == null)
            {
                return BadRequest("User cannot be null.");
            }

            var res = _authService.Validate(req.Username, req.Password);
            if (res)
            {
                User user = _authService.Get(req.Username);
                string token = _jwtHelper.GenerateToken(user, new List<int> { 0, 1, 2 });

                _authService.UpdateTokens(user.Username, token);

                var objToken = new
                {
                    token = token
                };

                return Ok(objToken);
            }

            return BadRequest("Username or Password is incorrect!");
        }

        [Authorize]
        [HttpGet("currentUser")]
        public IActionResult Get()
        {
            var res = _jwtHelper.GetCurrentUserData();
            return Ok(res);
        }

        [Authorize]
        [HttpGet("{username}")]
        public IActionResult Get(string username)
        {
            if (username == null)
            {
                return BadRequest("Username cannot be null.");
            }
            var res = _authService.GetUserData(username);
            return Ok(res);
        }

        [Authorize]
        [HttpPost("add")]
        public IActionResult Post([FromBody] User user)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            if (user == null)
            {
                return BadRequest("User cannot be null.");
            }
            var res = _authService.Add(user);
            return Ok(res);
        }

        [Authorize]
        [HttpPut("update/{userId}")]
        public IActionResult Put(string userId, [FromBody] User user)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin" && currUser.UserId != userId)
            {
                return BadRequest("User not allowed to access this resource.");
            }

            if (userId == null || user == null)
            {
                return BadRequest("Username or User cannot be null.");
            }

            var res = _authService.Update(userId, user);
            return Ok(res);
        }

        [Authorize]
        [HttpDelete("delete/{username}")]
        public IActionResult Delete(string username)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            if (username == null)
            {
                return BadRequest("Username cannot be null.");
            }

            var res = _authService.Delete(username);
            return Ok(res);
        }
    }
}
