using Library.PoS.Model;
using Newtonsoft.Json;

namespace Library.PoS.Services
{
    public class StudentServiceProxy
    {
        private List<Student> students;
        private static string filePath = "students.json";
        private StudentServiceProxy()
        {
            if (File.Exists(filePath)) //if a saved file exists...
            {
                var json = File.ReadAllText(filePath); //load the data
                students = JsonConvert.DeserializeObject<List<Student>>(json) ?? new List<Student>();
            }
            else
            {
                students = new List<Student> //else use fake data
                {
                    new Student { Id = 1, Name = "Natalie Santo", Code = "nrs23c", Classification = "Junior" },
                    new Student { Id = 2, Name = "Sam Smith", Code = "sls22d", Classification = "Senior" }
                };
                Save();
            }
        }

        private void Save() //save to disk 
        {
            var json = JsonConvert.SerializeObject(students, Formatting.Indented);
            File.WriteAllText(filePath, json);
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
        //add new student or edit one based on id w persistence
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
            Save();
            return student;
        }

        public Student? Delete(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);
            if (student != null)
            {
                students.Remove(student);
                Save();
            }
            return student;
        }

        public Student? GetById(int id) => students.FirstOrDefault(s => s.Id == id);
    }
}