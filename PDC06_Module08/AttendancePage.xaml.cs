using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PDC06_Module08
{
    public partial class AttendancePage : ContentPage
    {
        private const string ApiUrl = "http://192.168.100.164/pdc6/attendance-read.php"; // Replace with your actual API URL

        public AttendancePage()
        {
            InitializeComponent();
            LoadAttendanceData();
        }

        private async void LoadAttendanceData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Make a request to the API to get attendance data
                    var response = await client.GetStringAsync(ApiUrl);

                    // Deserialize the JSON response
                    List<AttendanceRecord> attendanceRecords = JsonConvert.DeserializeObject<List<AttendanceRecord>>(response, new JsonSerializerSettings
                    {
                        DateFormatHandling = DateFormatHandling.MicrosoftDateFormat // Adjust based on your actual date format
                    });

                    // Bind the data to the ListView
                    attendanceListView.ItemsSource = attendanceRecords;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network error, JSON parsing error)
                Console.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        private async void OnAddAttendanceClicked(object sender, EventArgs e)
        {
            // Navigate to the AddAttendancePage.xaml and pass the callback function
            await Navigation.PushAsync(new AddAttendancePage(RefreshList));
        }

        // Method to refresh the list on the page
        public void RefreshList()
        {
            LoadAttendanceData();
        }

        private async void OnAttendanceItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var selectedAttendance = e.SelectedItem as AttendanceRecord;

            // Prompt the user with options
            string action = await DisplayActionSheet($"Options for {selectedAttendance.StudentName}", "Cancel", null, "Update", "Delete");

            // Perform action based on user selection
            switch (action)
            {
                case "Update":
                    // Navigate to the UpdateAttendancePage.xaml and pass the selected attendance record
                    await Navigation.PushAsync(new UpdateAttendancePage(selectedAttendance, RefreshList));
                    break;

                case "Delete":
                    // Prompt the user if they want to delete the attendance record
                    bool isConfirmed = await DisplayAlert("Confirmation", $"Do you want to delete the attendance record for {selectedAttendance.StudentName}?", "Yes", "No");

                    if (isConfirmed)
                    {
                        // Call the API to delete the selected attendance record
                        bool success = await DeleteAttendance(selectedAttendance.Id);

                        // Display a message based on the API response
                        if (success)
                        {
                            await DisplayAlert("Success", "Attendance record deleted successfully", "OK");

                            // Refresh the list on the page
                            RefreshList();
                        }
                        else
                        {
                            await DisplayAlert("Error", "Error deleting attendance record", "OK");
                        }
                    }
                    break;
            }

            // Deselect the item
            attendanceListView.SelectedItem = null;
        }


        private async Task<bool> DeleteAttendance(int attendanceId)
        {
            // Construct the API URL
            string apiUrl = "http://192.168.100.164/pdc6/attendance-delete.php";

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Prepare the data to be sent in the POST request
                    var postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("id", attendanceId.ToString())
            };

                    // Send the POST request
                    var response = await client.PostAsync(apiUrl, new FormUrlEncodedContent(postData));

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

        // Define a class to represent the structure of your attendance records
        public class AttendanceRecord
        {
            public int Id { get; set; }
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public DateTime AttendanceDate { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}
