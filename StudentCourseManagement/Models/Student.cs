using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentCourseManagement.Models
{
    public class Student
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRequired]
        public string StudentId { get; set; }
        [BsonRequired]
        public string Name { get; set; }
        public string Email { get; set; }
        [BsonRequired]
        public DateTime DOB { get; set; }
        public List<String>? Courses { get; set; }


    }
}
