using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace StudentCourseManagement.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string? UserId { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        /*
            Multiple Role based Authorisation => 
                1. Viewer: Can view Students and Courses
                2. Student: Can view Students & Courses. Can also Enroll in courses. Can edit only their Details.
                3. Admin: Can edit, view and add Students and Courses. Can also enroll students in courses.
         */
        public string? Role { get; set; }
        public string? Token { get; set; }
    }
}
