using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PDC06_Module08
{
    public partial class AddAttendancePage : ContentPage
    {
        private readonly Action onAttendanceAdded;

        public AddAttendancePage(Action onAttendanceAddedCallback)
        {
            InitializeComponent();
            onAttendanceAdded = onAttendanceAddedCallback;
        }

        // Event handler for the "Submit Attendance" button click
        private async void OnSubmitAttendanceClicked(object sender, EventArgs e)
        {
            // Get user input from the entry fields and picker
            string studentName = studentNameEntry.Text;
            string studentIdText = studentIdEntry.Text;
            DateTime attendanceDate = attendanceDatePicker.Date;
            string status = statusPicker.SelectedItem?.ToString();

            // Validate user input
            if (string.IsNullOrWhiteSpace(studentName) ||
                string.IsNullOrWhiteSpace(studentIdText) ||
                string.IsNullOrWhiteSpace(status))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            // Convert student ID to an integer
            if (!int.TryParse(studentIdText, out int studentId))
            {
                await DisplayAlert("Error", "Invalid student ID", "OK");
                return;
            }

            // Call the API to add attendance
            bool success = await AddAttendance(studentName, studentId, attendanceDate, status);

            // Display a message based on the API response
            if (success)
            {
                await DisplayAlert("Success", "Attendance added successfully", "OK");

                // Call the callback function to refresh the list on the previous page
                onAttendanceAdded?.Invoke();

                // Navigate back to the previous page
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Error adding attendance", "OK");
            }
        }

        // Event handler for the "Cancel" button click
        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Prompt the user if they are sure they want to discard the attendance
            bool isConfirmed = await DisplayAlert("Confirmation", "Are you sure you want to discard the attendance?", "Yes", "No");

            if (isConfirmed)
            {
                // Navigate back to the previous page or perform other cancelation actions
                await Navigation.PopAsync();
            }
        }

        // Method to call the API and add attendance
        private async Task<bool> AddAttendance(string studentName, int studentId, DateTime attendanceDate, string status)
        {
            // Construct the API URL
            string apiUrl = "http://192.168.100.164/pdc6/attendance-create.php";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Prepare the data to be sent in the POST request
                    var postData = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("student_name", studentName),
                        new KeyValuePair<string, string>("student_id", studentId.ToString()),
                        new KeyValuePair<string, string>("attendance_date", attendanceDate.ToString("yyyy-MM-dd")),
                        new KeyValuePair<string, string>("status", status)
                    };

                    // Send the POST request
                    var response = await client.PostAsync(apiUrl, new FormUrlEncodedContent(postData));

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return false;
        }
    }
}
