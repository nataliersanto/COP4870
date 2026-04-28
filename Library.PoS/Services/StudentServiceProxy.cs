using Library.PoS.Model;

namespace Library.PoS.Services
{
    public class StudentServiceProxy
    {
        private List<Student> students;

        private StudentServiceProxy()
        {
            students = new List<Student>
            {
                new Student { Id = 1, Name = "Alice Smith", Code = "asmith01", Classification = "Junior" },
                new Student { Id = 2, Name = "Bob Jones", Code = "bjones02", Classification = "Senior" }
            };
        }

        private static object _lock = new object();
        private static StudentServiceProxy? instance;
        public static StudentServiceProxy Current
        {
            get
            {
                lock (_lock)
                {
                    if (instance == null)
                        instance = new StudentServiceProxy();
                }
                return instance;
            }
        }

        public List<Student> Students => students;

        public int NextKey => students.Any() ? students.Select(s => s.Id).Max() + 1 : 1;

        public Student? AddOrUpdate(Student? student)
        {
            if (student == null) return null;

            var existing = students.FirstOrDefault(s => s.Id == student.Id);
            if (existing != null)
            {
                var index = students.IndexOf(existing);
                students[index] = student;
            }
            else
            {
                student.Id = NextKey;
                students.Add(student);
            }
            return student;
        }

        public Student? Delete(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);
            if (student != null)
                students.Remove(student);
            return student;
        }

        public Student? GetById(int id) => students.FirstOrDefault(s => s.Id == id);
    }
}