using Library.PoS.Model;
using Newtonsoft.Json;

namespace Library.PoS.Services
{
    public class CourseServiceProxy
    {
        private List<Course> courses;
        private static string filePath = "courses.json";

        private CourseServiceProxy()
        {
            if (File.Exists(filePath))
                //if a saved file exists...
            {
                var json = File.ReadAllText(filePath); //load the data
                courses = JsonConvert.DeserializeObject<List<Course>>(json) ?? new List<Course>();
            }
            else
            {
                courses = new List<Course>
                {
                    new Course { Id = 1, Code = "COP4870", Name = "Full Stack Application Development", Description = "Building Canvas in C#", Semester = "Spring 2026", Section = "1" },
                    new Course { Id = 2, Code = "COP3330", Name = "Object Oriented Programming", Description = "Learn OOP concepts", Semester = "Fall 2024", Section = "2" }
                };
                Save();
            }
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(courses, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        private static object _lock = new object();
        private static CourseServiceProxy? instance;
        public static CourseServiceProxy Current
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                        instance = new CourseServiceProxy();
                }
                return instance;
            }
        }

        public List<Course> Courses => courses; //show all courses 

        public int NextKey => courses.Any() ? courses.Select(c => c.Id).Max() + 1 : 1;

        public Course? AddOrUpdate(Course? course) //edit/add new or check id 
        {
            if (course == null) return null;

            var existing = courses.FirstOrDefault(c => c.Id == course.Id);
            if (existing != null)
            {
                var index = courses.IndexOf(existing);
                courses[index] = course;
            }
            else
            {
                course.Id = NextKey;
                courses.Add(course);
            }
            Save();
            return course;
        }

        public Course? Delete(int id)
        {
            var course = courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
            {
                courses.Remove(course);
                Save();
            }
            return course;
        }

        public Course? GetById(int id) => courses.FirstOrDefault(c => c.Id == id);
    }
}