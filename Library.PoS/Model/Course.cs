namespace Library.PoS.Model
{
    public class Course
    {
        public int Id { get; set; }
        public string? Code { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Semester { get; set; }
        public DateTime? SemesterStart { get; set; }
        public DateTime? SemesterEnd { get; set; }
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
        public Dictionary<string, string> GradeColors { get; set; } = new Dictionary<string, string>
        {
            //using standard hex colors : 
            { "A", "#00FF00" }, // green
            { "B", "#0000FF" }, // blue
            { "C", "#FFFF00" }, // yellow
            { "D", "#FFA500" }, // orange
            { "F", "#FF0000" }  // red
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

        public string GetGradeColor(string letterGrade)
        {
            return GradeColors.TryGetValue(letterGrade, out var color) ? color : "#000000";
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
        public List<string> Content { get; set; } = new List<string>(); // keep for backwards compatibility
        public List<ModuleContent> RichContent { get; set; } = new List<ModuleContent>();

        public override string ToString()
        {
            return $"{Id}. {Name}";
        }
    }
    
    public abstract class ModuleContent // base class
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public abstract string Display();
    }
    public class PageContent : ModuleContent
    {
        public string? Body { get; set; }
        public override string Display() => $"[Page] {Title}: {Body}"; //just shows a string of text
    }
    public class FileContent : ModuleContent
    {
        public string? FilePath { get; set; }
        public string? FileName => Path.GetFileName(FilePath);
        public override string Display() => $"[File] {Title}: {FileName}";
    }
    
    public class AssignmentContent : ModuleContent
    {
        public int AssignmentId { get; set; }
        public override string Display() => $"[Assignment] {Title} (Assignment ID: {AssignmentId})"; //embed the assignment in the module 
    }

    public class Assignment
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int AvailablePoints { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsQuiz { get; set; }
        // Each question has a prompt and a list of multiple choice options
        public List<QuizQuestion> Questions { get; set; } = new List<QuizQuestion>();
        public List<Submission> Submissions { get; set; } = new List<Submission>();

        public override string ToString()
        {
            var type = IsQuiz ? "Quiz" : "Assignment";
            return $"{Id}. [{type}] {Name} (Due: {DueDate:MM/dd/yyyy}) [{AvailablePoints} pts]";
        }
    }

    public class QuizQuestion
    {
        public string? Prompt { get; set; }
        public List<string> Choices { get; set; } = new List<string>();
        public bool IsMultipleChoice => Choices.Any();

        public override string ToString()
        {
            return Prompt ?? "";
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