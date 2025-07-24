using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StudentCourseManagement.Helpers;
using StudentCourseManagement.Models;
using StudentCourseManagement.Services;
using System.Linq;

namespace StudentCourseManagement.Controllers
{
    [Route("api/students")]
    [ApiController]
    [Authorize]
    public class EnrollmentController : ControllerBase
    {
        private readonly StudentService studentService;
        private readonly CourseService courseService;
        private readonly JwtHelper _jwtHelper;
        public EnrollmentController(StudentService ss, CourseService cs, JwtHelper jwtHelper)
        {
            studentService = ss;
            courseService = cs;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("{id:length(4)}/enroll")]
        public IActionResult Post(string id, [FromBody] List<string> courseIds)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin" && currUser.Identity != "student")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            if (id == null || courseIds == null)
            {
                return BadRequest("Student ID or Course ID cannot be null.");
            }

            if (currUser.Identity == "student" && currUser.UserId != id)
            {
                return BadRequest("User not allowed to enroll for other student. Provide valid Id.");
            }

            Student student = studentService.Get(id);
            if (student == null)
            {
                return BadRequest("Student does not exist with given Id.");
            }

            string courseNotFoundMessage = "Course(s) does not exist with given Id(s): ";
            string userAlreadyEnrolledMessage = "Student already enrolled in the course(s): ";
            string enrolledCourses = "Student enrolled successfully in the following course(s): ";

            foreach (string courseId in courseIds)
            {
                Course course = courseService.Get(courseId);
                if (course == null)
                {
                    courseNotFoundMessage += $"{courseId}, ";
                    continue;
                }

                if (student.Courses == null)
                {
                    student.Courses = new List<String>();
                }

                if (student.Courses.Contains(courseId))
                {
                    userAlreadyEnrolledMessage += $"{courseId}, ";
                    continue;
                }


                student.Courses.Add(courseId);
                enrolledCourses += $"{courseId}, ";
            }

            studentService.Update(id, student);

            return Ok(enrolledCourses + "\n" + courseNotFoundMessage + "\n" + userAlreadyEnrolledMessage);
        }

        [HttpGet("{id:length(4)}/courses")]
        public IActionResult Get(string id)
        {
            if (id == null)
            {
                return BadRequest("Student ID cannot be null.");
            }

            Student student = studentService.Get(id);
            if (student == null)
            {
                return BadRequest("Student does not exist with given Id.");
            }

            if(student.Courses == null)
            {
                return BadRequest("Student is not enrolled in any courses!");
            }

            List<Course> enrolledCourses = new List<Course>();
            foreach (string courseId in student.Courses)
            {
                Course course = courseService.Get(courseId);
                if (course != null)
                {
                    enrolledCourses.Add(course);
                }
            }

            return Ok(enrolledCourses);
        }

        [HttpDelete("{id:length(4)}/unroll")]
        public IActionResult Delete(string id, [FromBody] List<string> courseIds)
        {
            userData currUser = _jwtHelper.GetCurrentUserData();
            if (currUser.Identity != "admin" && currUser.Identity != "student")
            {
                return BadRequest("User not allowed to access this resource.");
            }

            if (id == null || courseIds == null)
            {
                return BadRequest("Student ID or Course ID cannot be null.");
            }

            if (currUser.Identity == "student" && currUser.UserId != id)
            {
                return BadRequest("User not allowed to unroll for other student. Provide valid Id.");
            }

            Student student = studentService.Get(id);
            if (student == null)
            {
                return BadRequest("Student does not exist with given Id.");
            }

            string courseNotFoundMessage = "Course(s) does not exist with given Id(s): ";
            string userAlreadyUnrolledMessage = "Student already unrolled from the course(s): ";
            string unrolledCourses = "Student unrolled successfully in the following course(s): ";

            foreach (string courseId in courseIds)
            {
                Course course = courseService.Get(courseId);
                if (course == null)
                {
                    courseNotFoundMessage += $"{courseId}, ";
                    continue;
                }

                if (!student.Courses.Contains(courseId))
                {
                    userAlreadyUnrolledMessage += $"{courseId}, ";
                    continue;
                }


                student.Courses.Remove(courseId);
                unrolledCourses += $"{courseId}, ";
            }

            studentService.Update(id, student);

            return Ok(unrolledCourses + "\n" + courseNotFoundMessage + "\n" + userAlreadyUnrolledMessage);
        }
    }
}
