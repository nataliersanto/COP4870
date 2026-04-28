namespace Library.PoS.Model
{
    public class Course
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<Student> Roster { get; set; } = new List<Student>();
        public List<Module> Modules { get; set; } = new List<Module>();
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();

        public override string ToString()
        {
            return $"{Id}. [{Code}] {Name} - {Description}";
        }
    }

    public class Module
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public List<string> Content { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"{Id}. {Name}";
        }
    }

    public class Assignment
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int AvailablePoints { get; set; }
        public DateTime DueDate { get; set; }
        public List<Submission> Submissions { get; set; } = new List<Submission>();

        public override string ToString()
        {
            return $"{Id}. {Name} (Due: {DueDate:MM/dd/yyyy}) [{AvailablePoints} pts]";
        }
    }

    public class Submission
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int AssignmentId { get; set; }
        public string? Content { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int? Grade { get; set; }
        public string? Feedback { get; set; }

        public override string ToString()
        {
            return $"Submission {Id} by Student {StudentId} on {SubmissionDate:MM/dd/yyyy}";
        }
    }
}