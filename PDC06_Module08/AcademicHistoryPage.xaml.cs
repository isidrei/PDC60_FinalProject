using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PDC06_Module08
{
    public partial class AcademicHistoryPage : ContentPage
    {
        private const string ApiUrl = "http://192.168.100.164/pdc6/academichistory-read.php"; // Replace with your actual API URL
        private const string DeleteApiUrl = "http://192.168.100.164/pdc6/academichistory-delete.php"; // Replace with your actual delete API URL

        public AcademicHistoryPage()
        {
            InitializeComponent();
            LoadAcademicHistoryData();
        }

        private async void OnAcademicHistoryItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            // Get the selected item
            AcademicHistoryRecord selectedRecord = (AcademicHistoryRecord)e.SelectedItem;

            // Display academic history details in a Xamarin.Forms popup
            await DisplayAlert($"Details for {selectedRecord.student_name}",
                $"Student ID: {selectedRecord.student_id}\n" +
                $"Year Level: {selectedRecord.year_level}\n" +
                $"Year: {selectedRecord.year}\n" +
                $"Degree: {selectedRecord.degree}\n" +
                $"University: {selectedRecord.university}\n" +
                $"Major: {selectedRecord.major}\n" +
                $"GPA: {selectedRecord.gpa}",
                "OK");

            // Display an action sheet for further options
            string action = await DisplayActionSheet($"Options for {selectedRecord.student_name}", "Cancel", null, "Update", "Delete");

            switch (action)
            {
                case "Update":
                    // Navigate to the UpdateAcademicHistoryPage with the selected record
                    await Navigation.PushAsync(new UpdateAcademicHistoryPage(selectedRecord));
                    break;

                case "Delete":
                    // Perform the delete operation
                    bool deleteResult = await DeleteAcademicRecord(selectedRecord);

                    if (deleteResult)
                    {
                        await DisplayAlert("Success", "Academic record deleted successfully", "OK");
                        LoadAcademicHistoryData(); // Refresh the data after deletion
                    }
                    else
                    {
                        await DisplayAlert("Error", "Error deleting academic record", "OK");
                    }
                    break;
            }

            // Deselect the item
            academicHistoryListView.SelectedItem = null;
        }


        private async Task<bool> DeleteAcademicRecord(AcademicHistoryRecord record)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Send a DELETE request to the API to delete academic record
                    var response = await client.DeleteAsync($"http://192.168.100.164/pdc6/academichistory-delete.php?id={record.id}");

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

        public async void LoadAcademicHistoryData()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Make a request to the API to get academic history data
                    var response = await client.GetStringAsync(ApiUrl);

                    // Deserialize the JSON response
                    List<AcademicHistoryRecord> academicHistoryRecords = JsonConvert.DeserializeObject<List<AcademicHistoryRecord>>(response);

                    // Bind the data to the ListView
                    academicHistoryListView.ItemsSource = academicHistoryRecords;
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., network error, JSON parsing error)
                Console.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }

        // Handle button click to navigate to AddAcademicHistoryPage
        private async void OnAddAcademicRecordClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddAcademicHistoryPage());
        }

        public class AcademicHistoryRecord
        {
            public int id { get; set; }
            public string student_name { get; set; }
            public string student_id { get; set; }
            public int year_level { get; set; }
            public int year { get; set; }
            public string degree { get; set; }
            public string university { get; set; }
            public string major { get; set; }
            public decimal? gpa { get; set; } // Make GPA nullable
        }
    }
}
