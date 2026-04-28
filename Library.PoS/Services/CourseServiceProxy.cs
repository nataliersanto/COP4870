using Library.PoS.Model;

namespace Library.PoS.Services
{
    public class CourseServiceProxy
    {
        private List<Course> courses;

        private CourseServiceProxy()
        {
            courses = new List<Course>
            {
                new Course { Id = 1, Code = "COP4870", Name = "Mobile Development", Description = "Learn to build mobile apps", Semester = "Fall 2024", Section = "1" },
                new Course { Id = 2, Code = "COP3330", Name = "Object Oriented Programming", Description = "Learn OOP concepts", Semester = "Fall 2024", Section = "2" }
            };
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

        public List<Course> Courses => courses;

        public int NextKey => courses.Any() ? courses.Select(c => c.Id).Max() + 1 : 1;

        public Course? AddOrUpdate(Course? course)
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
            return course;
        }

        public Course? Delete(int id)
        {
            var course = courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
                courses.Remove(course);
            return course;
        }

        public Course? GetById(int id) => courses.FirstOrDefault(c => c.Id == id);
    }
}