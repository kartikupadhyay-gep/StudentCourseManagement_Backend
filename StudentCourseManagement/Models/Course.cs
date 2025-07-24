using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace StudentCourseManagement.Models
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonRequired]
        public string CourseId { get; set; }
        [BsonRequired]
        public string Title { get; set; }
        public string Description { get; set; }
        public int Credits { get; set; }

    }
}
