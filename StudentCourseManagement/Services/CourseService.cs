using MongoDB.Driver;
using StudentCourseManagement.Models;
using StudentCourseManagement.Helpers;

namespace StudentCourseManagement.Services
{
    public class CourseService
    {
        private readonly IMongoCollection<Course> _courses;

        public CourseService(IConfiguration config)
        {
            var mongoClient = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = mongoClient.GetDatabase(config["MongoDB:DatabaseName"]);
            _courses = database.GetCollection<Course>(config["MongoDB:CollectionNames:0"]);
        }

        /* METHODS -> 
            1. All courses
            2. Course By ID
            3. Create new Course
            4. Update existing Course
            5. Delete Course
        */

        public List<Course> Get()
        {
            return _courses.Find(c => true).ToList();
        }

        public Course Get(string id)
        {
            return _courses.Find(c => c.CourseId == id).FirstOrDefault();
        }

        public string Add(Course newCourse)
        {
            try
            {
                var res = _courses.Find(c => c.CourseId == newCourse.CourseId).FirstOrDefault();
                if (res != null)
                {
                    return "Course already exist!";
                }
                _courses.InsertOne(newCourse);
                return "Course Added Successfully!";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(string id, Course updatedCourse)
        {
            Course course = _courses.Find(c => c.CourseId == id).FirstOrDefault();
            if (course == null)
            {
                return "No courses found with the given ID";
            }

            foreach (var prop in typeof(Course).GetProperties())
            {
                var data = prop.GetValue(updatedCourse);
                if (data == null)
                {
                    var orgData = prop.GetValue(course);
                    prop.SetValue(updatedCourse, orgData);
                }

            }

            _courses.ReplaceOne(c => c.CourseId == id, updatedCourse);
            return "Course record Updated successfully!";
        }

        public string Delete(string id)
        {
            var res = _courses.DeleteOne(c => c.CourseId == id);
            if (res.DeletedCount == 0)
            {
                return "No courses found with the given ID";
            }
            return "Record of the Course deleted successfully!";
        }
    }
}
