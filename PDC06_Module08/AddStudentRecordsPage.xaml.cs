using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace PDC06_Module08
{
    public partial class AddStudentRecordsPage : ContentPage
    {
        public const string url = "http://192.168.100.164/pdc6/api_create.php";

        private HttpClient _Client = new HttpClient();

        public AddStudentRecordsPage()
        {
            InitializeComponent();
        }

        private async void OnAddRecord(object sender, EventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(xName.Text) ||
                string.IsNullOrWhiteSpace(xRollNumber.Text) ||
                string.IsNullOrWhiteSpace(xAge.Text) ||
                string.IsNullOrWhiteSpace(xEmail.Text))
            {
                await DisplayAlert("Error", "All fields are mandatory. Please provide values for all fields.", "OK");
                return;
            }

            // Validate Roll Number format
            if (!int.TryParse(xRollNumber.Text, out _))
            {
                await DisplayAlert("Error", "Roll Number should be a numeric value.", "OK");
                return;
            }

            // Validate Age format
            if (!int.TryParse(xAge.Text, out _))
            {
                await DisplayAlert("Error", "Age should be a numeric value.", "OK");
                return;
            }

            // Validate Email format
            if (!IsValidEmail(xEmail.Text))
            {
                await DisplayAlert("Error", "Invalid email format. Please provide a valid email address.", "OK");
                return;
            }

            bool userConfirmation = await DisplayAlert("Confirm", "Are you sure you want to add this record?", "Yes", "No");

            if (userConfirmation)
            {
                Post post = new Post
                {
                    name = xName.Text,
                    roll_number = xRollNumber.Text,
                    age = int.Parse(xAge.Text),
                    email = xEmail.Text
                };

                var content = JsonConvert.SerializeObject(post);

                var response = await _Client.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Success", "Record added successfully!", "OK");
                    // Optionally navigate back to the StudentRecordsPage after adding a record
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "Failed to add record. Please try again.", "OK");
                }

                await Navigation.PopAsync();
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        protected override bool OnBackButtonPressed()
        {
            ShowExitAlert();
            return true; // Prevent the default back button behavior
        }

        private async void ShowExitAlert()
        {
            bool exit = await DisplayAlert("Exit", "Changes will be unsaved. Are you sure you want to exit?", "Yes", "No");

            if (exit)
            {
                // Navigate back when the user confirms
                await Navigation.PopAsync();
            }
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            bool userConfirmation = await DisplayAlert("Cancel", "Are you sure you want to cancel adding a record?", "Yes", "No");

            if (userConfirmation)
            {
                // Navigate back when the user confirms
                await Navigation.PopAsync();
            }
        }

    }
}