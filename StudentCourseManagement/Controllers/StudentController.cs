using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services;

namespace StudentCourseManagement.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly StudentService studentService;
        private readonly JwtHelper _jwtHelper;
        public StudentController(StudentService ss, JwtHelper jwtHelper)
        {
            studentService = ss;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("students")]
        public IActionResult Get()
        {
            List<Student> students = studentService.Get();
            return Ok(students);
        }

        [HttpGet("students/{id:length(4)}")]
        public IActionResult Get(string id)
        {
            if (id == null)
            {
                return BadRequest("Id cannot be null!");
            }
            Student student = studentService.Get(id);
            if (student == null)
            {
                return BadRequest("Student does not exist with given Id");
            }
            return Ok(student);
        }

        [HttpPost("students")]
        public IActionResult Post([FromBody] Student student)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            try
            {
                var res = studentService.Add(student);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("students/{id:length(4)}")]
        public IActionResult Put(string id, [FromBody] Student student)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            try
            {
                var res = studentService.Update(id, student);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("students/{id:length(4)}")]
        public IActionResult Delete(string id)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            try
            {
                if (id == null)
                {
                    return BadRequest("Id cannot be null!");
                }
                var res = studentService.Delete(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
