namespace Library.PoS.Model
{
    public class User
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; } // equivalent to FSUID

        public override string ToString()
        {
            return $"{Id}. {Name} ({Code})";
        }
    }

    public class Student : User
    {
        public string? Classification { get; set; } // Freshman, Sophomore, etc.
    }

    public class Instructor : User
    {
        public int YearsOfExperience { get; set; }
    }
}