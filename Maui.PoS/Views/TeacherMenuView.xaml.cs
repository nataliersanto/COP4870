namespace Maui.PoS.Views;

public partial class TeacherMenuView : ContentPage
{
    public TeacherMenuView()
    {
        InitializeComponent();
    }

    private void ManageCoursesClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//CourseDetail");
    }

    private void ManageStudentsClicked(object sender, EventArgs e)
    {
        // Coming soon
    }

    private void BackClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
    
    private void ManageCoursesClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//CourseList");
    }
}