using System;
using Xamarin.Forms;

namespace PDC06_Module08
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            // Set the navigation bar color to transparent
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void ViewStudentRecords_Clicked(object sender, EventArgs e)
        {
            // Navigate to the page where you display student records (you can use your existing code or create a new page)
            await Navigation.PushAsync(new StudentRecordsPage());
        }

        private async void AboutUs_Clicked(object sender, EventArgs e)
        {
            // Handle the "About Us" button click event
            await Navigation.PushAsync(new AboutUsPage());
        }
        
        private async void OnAttendanceButtonClicked(object sender, EventArgs e)
        {
            // You can replace 'AttendancePage' with the actual name of your attendance page
            await Navigation.PushAsync(new AttendancePage());
        }

        private async void OnViewAcademicHistoryClicked(object sender, EventArgs e)
        {
            // Navigate to the AcademicHistoryPage.xaml
            await Navigation.PushAsync(new AcademicHistoryPage());
        }
    }
}
