namespace Library.PoS.Model
{
    public class Course
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Semester { get; set; }
        public string? Section { get; set; }
        public List<Student> Roster { get; set; } = new List<Student>();
        public List<Module> Modules { get; set; } = new List<Module>();
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();
        public List<AssignmentGroup> AssignmentGroups { get; set; } = new List<AssignmentGroup>();
        public Dictionary<string, double> GradeRanges { get; set; } = new Dictionary<string, double>
        {
            { "A", 90 },
            { "B", 80 },
            { "C", 70 },
            { "D", 60 },
            { "F", 0 }
        };
        public string GetLetterGrade(double percentage)
        {
            foreach (var range in GradeRanges.OrderByDescending(r => r.Value))
            {
                if (percentage >= range.Value)
                    return range.Key;
            }
            return "F";
        }
        public override string ToString()
        {
            return $"{Id}. [{Code}] {Name} | {Semester} Section {Section} - {Description}";
        }
    }

    public class AssignmentGroup
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double Weight { get; set; } // e.g. 0.4 = 40%
        public List<Assignment> Assignments { get; set; } = new List<Assignment>();

        public override string ToString()
        {
            return $"{Id}. {Name} (Weight: {Weight:P0})";
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
        public bool IsQuiz { get; set; }
        public List<string> Questions { get; set; } = new List<string>();
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
        public string? FilePath { get; set; }
        public DateTime SubmissionDate { get; set; }
        public int? Grade { get; set; }
        public string? Feedback { get; set; }

        public override string ToString()
        {
            return $"Submission {Id} by Student {StudentId} on {SubmissionDate:MM/dd/yyyy}";
        }
    }
}