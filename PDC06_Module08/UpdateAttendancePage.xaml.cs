using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using static PDC06_Module08.AttendancePage;

namespace PDC06_Module08
{
    public partial class UpdateAttendancePage : ContentPage
    {
        private readonly AttendanceRecord _selectedAttendance;
        private readonly Action _refreshAction;

        public UpdateAttendancePage(AttendanceRecord selectedAttendance, Action refreshAction)
        {
            InitializeComponent();
            _selectedAttendance = selectedAttendance;
            _refreshAction = refreshAction;

            // Set initial values based on the selected attendance record
            studentNameEntry.Text = _selectedAttendance.StudentName;
            attendanceDatePicker.Date = _selectedAttendance.AttendanceDate;
            statusPicker.SelectedItem = _selectedAttendance.Status;
        }

        private async void OnUpdateAttendanceClicked(object sender, EventArgs e)
        {
            // Get updated values from the entry fields and picker
            string updatedStudentName = studentNameEntry.Text;
            DateTime updatedAttendanceDate = attendanceDatePicker.Date;
            string updatedStatus = statusPicker.SelectedItem?.ToString();

            // Validate user input
            if (string.IsNullOrWhiteSpace(updatedStudentName) || string.IsNullOrWhiteSpace(updatedStatus))
            {
                await DisplayAlert("Error", "Please fill in all fields", "OK");
                return;
            }

            // Call the API to update attendance
            bool success = await UpdateAttendance(_selectedAttendance.Id, updatedStudentName, updatedAttendanceDate, updatedStatus);

            // Display a message based on the API response
            if (success)
            {
                await DisplayAlert("Success", "Attendance record updated successfully", "OK");

                // Refresh the list on the previous page
                _refreshAction.Invoke();

                // Navigate back to the previous page
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Error updating attendance record", "OK");
            }
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            // Prompt the user if they are sure they want to discard the update
            bool isConfirmed = await DisplayAlert("Confirmation", "Are you sure you want to discard the update?", "Yes", "No");

            if (isConfirmed)
            {
                // Navigate back to the previous page or perform other cancelation actions
                await Navigation.PopAsync();
            }
        }

        private async Task<bool> UpdateAttendance(int attendanceId, string studentName, DateTime attendanceDate, string status)
        {
            // Construct the API URL
            string apiUrl = $"http://192.168.100.164/pdc6/attendance-update.php";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Prepare the data to be sent in the PUT request
                    var putData = new
                    {
                        id = attendanceId,
                        student_name = studentName,
                        attendance_date = attendanceDate.ToString("yyyy-MM-dd"),
                        status
                    };

                    // Serialize the data to JSON
                    var jsonContent = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(putData), Encoding.UTF8, "application/json");

                    // Send the PUT request
                    var response = await client.PutAsync(apiUrl, jsonContent);

                    // Check if the request was successful
                    return response.IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
    }
}
