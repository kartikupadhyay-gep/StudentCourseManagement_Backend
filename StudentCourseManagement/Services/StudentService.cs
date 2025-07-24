using MongoDB.Driver;
using StudentCourseManagement.Models;

namespace StudentCourseManagement.Services
{
    public class StudentService
    {
        private readonly IMongoCollection<Student> _students;

        public StudentService(IConfiguration config)
        {
            var mongoClient = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = mongoClient.GetDatabase(config["MongoDB:DatabaseName"]);
            _students = database.GetCollection<Student>(config["MongoDB:CollectionNames:1"]);
        }

        /* METHODS -> 
            1. All students
            2. Student By ID
            3. Create new Student
            4. Update existing Student
            5. Delete Student
        */

        public List<Student> Get()
        {
            return _students.Find(s => true).ToList();
        }

        public Student Get(string id)
        {
            return _students.Find(s => s.StudentId == id).FirstOrDefault();
        }

        public string Add(Student newStudent)
        {
            try
            {
                var res = _students.Find(s => s.StudentId == newStudent.StudentId).FirstOrDefault();
                if (res != null)
                {
                    return "Student already exist!";
                }
                _students.InsertOne(newStudent);
                return "Student Added Successfully!";
            } catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string Update(string id, Student updatedStudent)
        {
            Student student = _students.Find(s => s.StudentId == id).FirstOrDefault();
            if (student == null)
            {
                return "No students found with the given ID";
            }

            foreach (var prop in typeof(Student).GetProperties())
            {
                var data = prop.GetValue(updatedStudent);
                if (data == null)
                {
                    var orgData = prop.GetValue(student);
                    prop.SetValue(updatedStudent, orgData);
                }

            }

            _students.ReplaceOne(s => s.StudentId == id, updatedStudent);
            return "Student record Updated successfully!";
        }

        public string Delete(string id)
        {
            var res = _students.DeleteOne(s => s.StudentId == id);
            if (res.DeletedCount == 0)
            {
                return "No students found with the given ID";
            }
            return "Record of the student deleted successfully!";
        }
    }
}
