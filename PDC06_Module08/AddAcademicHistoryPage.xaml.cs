using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PDC06_Module08
{
    public partial class AddAcademicHistoryPage : ContentPage
    {
        private const string ApiUrl = "http://192.168.100.164/pdc6/academichistory-create.php"; // Replace with your actual API URL

        public AddAcademicHistoryPage()
        {
            InitializeComponent();
        }

        private async void OnAddAcademicRecordClicked(object sender, EventArgs e)
        {
            try
            {
                // Retrieve data from entry fields
                string studentName = studentNameEntry.Text;
                string studentId = studentIdEntry.Text;
                string yearLevel = yearLevelEntry.Text;
                string year = yearEntry.Text;
                string degree = degreeEntry.Text;
                string university = universityEntry.Text;
                string major = majorEntry.Text;
                string gpa = gpaEntry.Text;

                // Validate if required fields are not empty
                if (string.IsNullOrWhiteSpace(studentName) || string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(yearLevel) || string.IsNullOrWhiteSpace(year) || string.IsNullOrWhiteSpace(degree) || string.IsNullOrWhiteSpace(university) || string.IsNullOrWhiteSpace(major) || string.IsNullOrWhiteSpace(gpa))
                {
                    await DisplayAlert("Error", "All fields are required", "OK");
                    return;
                }

                // Create a data object
                var academicRecord = new
                {
                    student_name = studentName,
                    student_id = studentId,
                    year_level = yearLevel,
                    year = year,
                    degree = degree,
                    university = university,
                    major = major,
                    gpa = gpa
                };

                // Serialize the data object to JSON
                string jsonData = JsonConvert.SerializeObject(academicRecord);

                using (HttpClient client = new HttpClient())
                {
                    // Create the StringContent for the request
                    StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                    // Send a POST request to the API to add academic record
                    var response = await client.PostAsync(ApiUrl, content);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "Academic record added successfully", "OK");
                        // Optionally, you can navigate back to the AcademicHistoryPage
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Error adding academic record", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
        }
    }
}
