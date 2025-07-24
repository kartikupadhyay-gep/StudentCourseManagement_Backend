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
    public class CourseController : ControllerBase
    {
        private readonly CourseService courseService;
        private readonly JwtHelper _jwtHelper;

        public CourseController(CourseService cs, JwtHelper jwtHelper)
        {
            courseService = cs;
            _jwtHelper = jwtHelper;
        }

        [HttpGet("courses")]
        public IActionResult Get()
        {
            List<Course> courses = courseService.Get();
            return Ok(courses);
        }

        [HttpGet("courses/{id:length(5)}")]
        public IActionResult Get(string id)
        {
            if (id == null)
            {
                return BadRequest("Id cannot be null!");
            }
            Course course = courseService.Get(id);
            if (course == null)
            {
                return BadRequest("Course does not exist with given Id");
            }
            return Ok(course);
        }

        [HttpPost("courses")]
        public IActionResult Post([FromBody] Course course)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            try
            {
                var res = courseService.Add(course);
                return Ok(res);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("courses/{id:length(5)}")]
        public IActionResult Put(string id, [FromBody] Course course)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            try
            {
                var res = courseService.Update(id, course);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("courses/{id:length(5)}")]
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
                var res = courseService.Delete(id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
