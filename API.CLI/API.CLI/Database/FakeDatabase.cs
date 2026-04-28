using Library.PoS.Model;

namespace API.CLI.Database
{
    public class FakeDatabase
    {
        private List<Student> students;
        private List<Course> courses;

        private FakeDatabase()
        {
            students = new List<Student>
            {
                new Student { Id = 1, Name = "Alice Smith", Code = "asmith01", Classification = "Junior" },
                new Student { Id = 2, Name = "Bob Jones", Code = "bjones02", Classification = "Senior" }
            };

            courses = new List<Course>
            {
                new Course { Id = 1, Code = "COP4870", Name = "Mobile Development", Description = "Learn to build mobile apps", Semester = "Fall 2024", Section = "1" },
                new Course { Id = 2, Code = "COP3330", Name = "Object Oriented Programming", Description = "Learn OOP concepts", Semester = "Fall 2024", Section = "2" }
            };
        }

        private static FakeDatabase? instance;
        public static FakeDatabase Current
        {
            get
            {
                if (instance == null)
                    instance = new FakeDatabase();
                return instance;
            }
        }

        public List<Student> Students => students;
        public List<Course> Courses => courses;
    }
}