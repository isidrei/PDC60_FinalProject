using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;

namespace PDC06_Module08
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UpdatePage : ContentPage
    {
        private const string url_update = "http://192.168.100.164/pdc6/api-update.php";
        private HttpClient _Client = new HttpClient();
        private ObservableCollection<Post> _posts;

        public UpdatePage(Post post)
        {
            InitializeComponent();
            xID.Text = post.ID;
            xName.Text = post.name;
            xRollNumber.Text = post.roll_number;
            xAge.Text = post.age?.ToString(); // Assuming age is nullable
            xEmail.Text = post.email;

            // Add a "Previous Page" button to the toolbar
            ToolbarItems.Add(new ToolbarItem("Previous Page", null, () => OnPreviousPage()));
        }

        private async Task UpdatePostAsync()
        {
            try
            {
                Post post = new Post
                {
                    ID = xID.Text,
                    name = xName.Text,
                    roll_number = xRollNumber.Text,
                    age = string.IsNullOrEmpty(xAge.Text) ? (int?)null : int.Parse(xAge.Text),
                    email = xEmail.Text,
                };

                var content = JsonConvert.SerializeObject(post);
                var response = await _Client.PostAsync(url_update, new StringContent(content, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Success", "Record updated successfully", "OK");

                    // After a successful update, navigate to the StudentRecordsPage
                    await Navigation.PushAsync(new StudentRecordsPage());
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update record. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        private async void OnUpdate(object sender, EventArgs e)
        {
            bool result = await DisplayAlert("Update Confirmation",
                $"Are you sure you want to update ID: {xID.Text}?",
                "OK", "Cancel");

            if (result)
            {
                await UpdatePostAsync();
            }
            else
            {
                // Handle user clicked cancel
            }
        }

        private async void OnCancel(object sender, EventArgs e)
        {
            // Handle cancel action here, for example, navigate back
            await Navigation.PopAsync();
        }

        private async void OnPreviousPage()
        {
            // Navigate back to the previous page
            await Navigation.PopAsync();
        }
    }
}

