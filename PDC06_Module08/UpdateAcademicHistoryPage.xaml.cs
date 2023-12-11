using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PDC06_Module08
{
    public partial class UpdateAcademicHistoryPage : ContentPage
    {
        private const string UpdateApiUrl = "http://192.168.100.164/pdc6/academichistory-update.php"; // Replace with your actual update API URL
        private AcademicHistoryPage.AcademicHistoryRecord _selectedRecord;

        public UpdateAcademicHistoryPage(AcademicHistoryPage.AcademicHistoryRecord selectedRecord)
        {
            InitializeComponent();
            _selectedRecord = selectedRecord;

            // Populate entry fields with current values
            ID.Text = selectedRecord.id.ToString();
            studentNameEntry.Text = selectedRecord.student_name;
            studentIdEntry.Text = selectedRecord.student_id;
            yearLevelEntry.Text = selectedRecord.year_level.ToString();
            yearEntry.Text = selectedRecord.year.ToString();
            degreeEntry.Text = selectedRecord.degree;
            universityEntry.Text = selectedRecord.university;
            majorEntry.Text = selectedRecord.major;
            gpaEntry.Text = selectedRecord.gpa?.ToString();
        }

        private async void OnUpdateClicked(object sender, EventArgs e)
        {
            // Get updated data from entry fields
            int id = int.Parse(ID.Text);
            string name = studentNameEntry.Text;
            string studentId = studentIdEntry.Text;
            int yearLevel = int.Parse(yearLevelEntry.Text);
            int year = int.Parse(yearEntry.Text);
            string degree = degreeEntry.Text;
            string university = universityEntry.Text;
            string major = majorEntry.Text;
            decimal? gpa = string.IsNullOrEmpty(gpaEntry.Text) ? null : (decimal?)decimal.Parse(gpaEntry.Text);

            // Prepare updated data
            var updatedData = new
            {
                id = id,
                student_name = name,
                student_id = studentId,
                year_level = yearLevel,
                year = year,
                degree = degree,
                university = university,
                major = major,
                gpa = gpa
            };

            // Send a PUT request to the API to update academic record
            bool updateResult = await UpdateAcademicRecord(updatedData);

            if (updateResult)
            {
                await DisplayAlert("Success", "Academic record updated successfully", "OK");
                // You may choose to navigate back to the previous page or perform any other actions
            }
            else
            {
                await DisplayAlert("Error", "Error updating academic record", "OK");
            }
        }

        private async Task<bool> UpdateAcademicRecord(object updatedData)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    // Construct the JSON payload
                    var jsonPayload = JsonConvert.SerializeObject(updatedData);
                    var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                    // Send a PUT request with the updated data to the API to update academic record
                    var response = await client.PutAsync(UpdateApiUrl, content);

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
