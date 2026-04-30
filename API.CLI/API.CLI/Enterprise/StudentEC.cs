using API.CLI.Database;
using Library.PoS.Model;

namespace API.CLI.Enterprise
{
    public class StudentEC
    {
        public IEnumerable<Student> Students => FakeDatabase.Current.Students;
    
        public Student? GetById(int id) => FakeDatabase.Current.Students.FirstOrDefault(s => s.Id == id);

        public Student? AddOrUpdate(Student student)
        {
            var existing = FakeDatabase.Current.Students.FirstOrDefault(s => s.Id == student.Id);
            if (existing != null)
            {
                var index = FakeDatabase.Current.Students.IndexOf(existing);
                FakeDatabase.Current.Students[index] = student;
            }
            else
            {
                student.Id = NextKey;
                FakeDatabase.Current.Students.Add(student);
            }
            return student;
        }

        public Student? Delete(int id)
        {
            var student = FakeDatabase.Current.Students.FirstOrDefault(s => s.Id == id);
            if (student != null)
                FakeDatabase.Current.Students.Remove(student);
            return student;
        }

        public int NextKey => Students.Any() ? Students.Select(s => s.Id).Max() + 1 : 1;
    }

    public class CourseEC
    {
        public IEnumerable<Course> Courses => FakeDatabase.Current.Courses;

        public Course? GetById(int id) => FakeDatabase.Current.Courses.FirstOrDefault(c => c.Id == id);

        public Course? AddOrUpdate(Course course)
        {
            var existing = FakeDatabase.Current.Courses.FirstOrDefault(c => c.Id == course.Id);
            if (existing != null)
            {
                var index = FakeDatabase.Current.Courses.IndexOf(existing);
                FakeDatabase.Current.Courses[index] = course;
            }
            else
            {
                course.Id = NextKey;
                FakeDatabase.Current.Courses.Add(course);
            }
            return course;
        }

        public Course? Delete(int id)
        {
            var course = FakeDatabase.Current.Courses.FirstOrDefault(c => c.Id == id);
            if (course != null)
                FakeDatabase.Current.Courses.Remove(course);
            return course;
        }

        public int NextKey => Courses.Any() ? Courses.Select(c => c.Id).Max() + 1 : 1;
    }
}