using Library.PoS.Model;
using Library.PoS.Services;

namespace Maui.PoS.Views;

public partial class StudentManagementView : ContentPage
{
    public StudentManagementView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StudentsListView.ItemsSource = null;
        StudentsListView.ItemsSource = StudentServiceProxy.Current.Students;
    }

    private async void AddStudentClicked(object sender, EventArgs e)
    {
        var name = await DisplayPromptAsync("New Student", "Name:");
        if (string.IsNullOrEmpty(name)) return;
        var code = await DisplayPromptAsync("New Student", "FSUID:");
        var classification = await DisplayPromptAsync("New Student", "Classification:");

        var student = new Student
        {
            Name = name,
            Code = code,
            Classification = classification
        };
        StudentServiceProxy.Current.AddOrUpdate(student);
        OnAppearing();
    }

    private async void StudentSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Student student)
        {
            var action = await DisplayActionSheet(
                $"Edit {student.Name}",
                "Cancel",
                null,
                "Edit Name",
                "Edit Classification");

            if (action == "Edit Name")
            {
                var newName = await DisplayPromptAsync("Edit Student", "New Name:", initialValue: student.Name);
                if (!string.IsNullOrEmpty(newName))
                {
                    student.Name = newName;
                    StudentServiceProxy.Current.AddOrUpdate(student);
                    OnAppearing();
                }
            }
            else if (action == "Edit Classification")
            {
                var newClass = await DisplayPromptAsync("Edit Student", "New Classification:", initialValue: student.Classification);
                if (!string.IsNullOrEmpty(newClass))
                {
                    student.Classification = newClass;
                    StudentServiceProxy.Current.AddOrUpdate(student);
                    OnAppearing();
                }
            }
        }
    }

    private void DeleteClicked(object sender, EventArgs e)
    {
        if (StudentsListView.SelectedItem is Student student)
        {
            StudentServiceProxy.Current.Delete(student.Id);
            // Remove from all course rosters too
            CourseServiceProxy.Current.Courses.ForEach(c =>
                c.Roster.RemoveAll(s => s.Id == student.Id));
            OnAppearing();
        }
    }

    private void BackClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//TeacherMenu");
    }
}