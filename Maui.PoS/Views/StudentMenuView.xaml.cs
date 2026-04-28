using Library.PoS.Model;
using Library.PoS.Services;

namespace Maui.PoS.Views;

public partial class StudentMenuView : ContentPage
{
    public StudentMenuView()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        StudentListView.ItemsSource = StudentServiceProxy.Current.Students;
    }

    private async void StudentSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is Student student)
        {
            await Shell.Current.GoToAsync($"//CourseDetail?studentId={student.Id}");
        }
    }

    private void BackClicked(object sender, EventArgs e)
    {
        Shell.Current.GoToAsync("//MainPage");
    }
}