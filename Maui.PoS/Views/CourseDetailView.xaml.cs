using Library.PoS.Model;
using Library.PoS.Services;

namespace Maui.PoS.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class CourseDetailView : ContentPage
{
    private Student? _student;
    private Course? _course;

    public int StudentId { get; set; }

    public CourseDetailView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        _student = StudentServiceProxy.Current.GetById(StudentId);

        if (_student != null)
        {
            // Get first enrolled course for now
            _course = CourseServiceProxy.Current.Courses
                .FirstOrDefault(c => c.Roster.Any(s => s.Id == _student.Id));
        }

        if (_course != null)
        {
            CourseNameLabel.Text = $"{_course.Name} ({_course.Code})";
            ModulesListView.ItemsSource = _course.Modules;
            AssignmentsListView.ItemsSource = _course.Assignments;
            RosterListView.ItemsSource = _course.Roster;

            // Calculate and show grade
            var gradedAssignments = _course.Assignments
                .Where(a => a.Submissions.Any(s => s.StudentId == StudentId && s.Grade != null))
                .ToList();

            if (gradedAssignments.Any())
            {
                double avg = gradedAssignments.Average(a =>
                {
                    var sub = a.Submissions.First(s => s.StudentId == StudentId);
                    return (double)sub.Grade! / a.AvailablePoints * 100;
                });

                string letter = avg >= 90 ? "A" : avg >= 80 ? "B" : avg >= 70 ? "C" : avg >= 60 ? "D" : "F";
                GradeLabel.Text = $"Current Grade: {letter} ({avg:F1}%)";
            }
            else
            {
                GradeLabel.Text = "Current Grade: N/A";
            }
        }
    }

    private async void AssignmentSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Assignment assignment && _student != null)
        {
            string content = await DisplayPromptAsync(
                "Submit Assignment",
                $"{assignment.Name}\n{assignment.Description}",
                placeholder: "Enter your submission here");

            if (!string.IsNullOrEmpty(content))
            {
                var submission = new Submission
                {
                    Id = assignment.Submissions.Any() ? assignment.Submissions.Max(s => s.Id) + 1 : 1,
                    StudentId = _student.Id,
                    AssignmentId = assignment.Id,
                    Content = content,
                    SubmissionDate = DateTime.Now
                };
                assignment.Submissions.Add(submission);
                await DisplayAlert("Success", "Submission recorded!", "OK");
            }
        }
    }

    private void BackClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//StudentMenu");
    }
}