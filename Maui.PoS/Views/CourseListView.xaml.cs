using Library.PoS.Model;
using Library.PoS.Services;

namespace Maui.PoS.Views;

public partial class CourseListView : ContentPage
{
    public CourseListView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        CoursesListView.ItemsSource = null;
        CoursesListView.ItemsSource = CourseServiceProxy.Current.Courses;
    }

    private async void AddCourseClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("New Course", "Course Name:");
        if (string.IsNullOrEmpty(name)) return;
        var code = await DisplayPromptAsync("New Course", "Course Code:");
        var description = await DisplayPromptAsync("New Course", "Description:");
        var semester = await DisplayPromptAsync("New Course", "Semester (e.g. Fall 2024):");
        var section = await DisplayPromptAsync("New Course", "Section:");

        var course = new Course
        {
            Name = name,
            Code = code,
            Description = description,
            Semester = semester,
            Section = section
        };
        CourseServiceProxy.Current.AddOrUpdate(course);
        OnAppearing();
    }

    private void CourseSelected(object sender, SelectedItemChangedEventArgs e)
    {
        // TODO: navigate to course detail for teacher
    }

    private void DeleteClicked(object sender, EventArgs e)
    {
        if (CoursesListView.SelectedItem is Course course)
        {
            CourseServiceProxy.Current.Delete(course.Id);
            OnAppearing();
        }
    }

    private void BackClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//TeacherMenu");
    }
}