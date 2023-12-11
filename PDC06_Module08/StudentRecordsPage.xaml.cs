using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace PDC06_Module08
{
    public class Post
    {
        public string ID { get; set; }
        public string name { get; set; }
        public string roll_number { get; set; }
        public int? age { get; set; }
        public string email { get; set; }
    }


    public partial class StudentRecordsPage : ContentPage
    {
        private const string url_retrieve = "http://192.168.100.164/pdc6/api_r2.php";
        private HttpClient _Client = new HttpClient();
        private ObservableCollection<Post> _post;


        public StudentRecordsPage()
        {
            InitializeComponent();

        }

        protected override async void OnAppearing()
        {

            var content = await _Client.GetStringAsync(url_retrieve);
            var post = JsonConvert.DeserializeObject<List<Post>>(content);

            _post = new ObservableCollection<Post>(post);
            StudentListView.ItemsSource = _post;
            base.OnAppearing();
        }



        private async void OnRefresh(object sender, EventArgs e)
        {
            try
            {
                var content = await _Client.GetStringAsync(url_retrieve);
                var post = JsonConvert.DeserializeObject<List<Post>>(content);

                _post = new ObservableCollection<Post>(post);
                StudentListView.ItemsSource = _post;
                await DisplayAlert("Success", "Data refreshed successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to refresh data. {ex.Message}", "OK");
            }
        }

        private async Task RefreshData()
        {
            try
            {
                var content = await _Client.GetStringAsync(url_retrieve);
                var posts = JsonConvert.DeserializeObject<List<Post>>(content);

                _post = new ObservableCollection<Post>(posts);
                StudentListView.ItemsSource = _post;
                await DisplayAlert("Success", "Data refreshed successfully!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to refresh data. {ex.Message}", "OK");
            }
        }


        private async void OnAddRecord(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AddStudentRecordsPage());
        }

        private async Task DeleteRecord(Post post)
        {
            var confirm = await DisplayAlert("Confirm Deletion", "Are you sure you want to delete this record?", "Yes", "No");

            if (confirm)
            {
                try
                {
                    var urlDelete = "http://192.168.100.164/pdc6/api-delete.php";
                    var data = JsonConvert.SerializeObject(new { id = post.ID });
                    var content = new StringContent(data, System.Text.Encoding.UTF8, "application/json");

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Delete,
                        RequestUri = new Uri(urlDelete),
                        Content = content
                    };

                    var response = await _Client.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        await DisplayAlert("Success", "Record deleted successfully!", "OK");
                        await RefreshData(); // Refresh the data after deletion
                                             // After a successful delete, pop the current page to go back
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to delete record. Please try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Post selectedPost)
            {
                var nameLabel = new Label
                {
                    Text = "Name: ",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Start
                };

                var rollNumberLabel = new Label
                {
                    Text = "Roll Number: ",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Start
                };

                var ageLabel = new Label
                {
                    Text = "Age: ",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Start
                };

                var emailLabel = new Label
                {
                    Text = "Email: ",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 16,
                    HorizontalOptions = LayoutOptions.Start
                };

                var customAlert = new StackLayout
                {
                    Padding = new Thickness(20),
                    Spacing = 15,
                    BackgroundColor = Color.White, // Set your preferred background color
                    Children =
            {
                new Label
                {
                    Text = "Student Details",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Start
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        nameLabel,
                        new Label { Text = selectedPost.name, FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        rollNumberLabel,
                        new Label { Text = selectedPost.roll_number, FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        ageLabel,
                        new Label { Text = selectedPost.age.ToString(), FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        emailLabel,
                        new Label { Text = selectedPost.email, FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Spacing = 15,
                    Children =
                    {
                        new Button
                        {
                            Text = "Update",
                            BackgroundColor = Color.FromHex("#4CAF50"), // Set your preferred color
                            TextColor = Color.White, // Set your preferred text color
                            Command = new Command(async () => await Navigation.PushAsync(new UpdatePage(selectedPost)))
                        },
                        new Button
                        {
                            Text = "Delete",
                            BackgroundColor = Color.FromHex("#F44336"), // Set your preferred color
                            TextColor = Color.White, // Set your preferred text color
                            Command = new Command(async () => await DeleteRecord(selectedPost))
                        },
                        new Button
                        {
                            Text = "Cancel",
                            BackgroundColor = Color.FromHex("#2196F3"), // Set your preferred color
                            TextColor = Color.White, // Set your preferred text color
                            Command = new Command(() => CloseCustomAlert())
                        }
                    }
                }
            }
                };

                var page = new ContentPage
                {
                    Content = new ScrollView { Content = customAlert }
                };

                async void CloseCustomAlert()
                {
                    await Navigation.PopAsync();
                }

                await Navigation.PushAsync(page);

                ((ListView)sender).SelectedItem = null; // Deselect the item
            }
        }
        private async void OnSearch(object sender, EventArgs e)
        {
            // Pass the existing instance of StudentRecordsPage to the SearchPage
            await Navigation.PushAsync(new SearchPage(this));
        }

        public void ShowStudentDetails(Post selectedPost)
        {
            var nameLabel = new Label
            {
                Text = "Name: ",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Start
            };

            var rollNumberLabel = new Label
            {
                Text = "Roll Number: ",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Start
            };

            var ageLabel = new Label
            {
                Text = "Age: ",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Start
            };

            var emailLabel = new Label
            {
                Text = "Email: ",
                FontAttributes = FontAttributes.Bold,
                FontSize = 16,
                HorizontalOptions = LayoutOptions.Start
            };

            var customAlert = new StackLayout
            {
                Padding = new Thickness(20),
                Spacing = 15,
                BackgroundColor = Color.White, // Set your preferred background color
                Children =
            {
                new Label
                {
                    Text = "Student Details",
                    FontAttributes = FontAttributes.Bold,
                    FontSize = 20,
                    HorizontalOptions = LayoutOptions.Start
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        nameLabel,
                        new Label { Text = selectedPost.name, FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        rollNumberLabel,
                        new Label { Text = selectedPost.roll_number, FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        ageLabel,
                        new Label { Text = selectedPost.age.ToString(), FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children =
                    {
                        emailLabel,
                        new Label { Text = selectedPost.email, FontSize = 16, HorizontalOptions = LayoutOptions.StartAndExpand }
                    }
                },
                new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    Spacing = 15,
                    Children =
                    {
                        new Button
                        {
                            Text = "Update",
                            BackgroundColor = Color.FromHex("#4CAF50"), // Set your preferred color
                            TextColor = Color.White, // Set your preferred text color
                            Command = new Command(async () => await Navigation.PushAsync(new UpdatePage(selectedPost)))
                        },
                        new Button
                        {
                            Text = "Delete",
                            BackgroundColor = Color.FromHex("#F44336"), // Set your preferred color
                            TextColor = Color.White, // Set your preferred text color
                            Command = new Command(async () => await DeleteRecord(selectedPost))
                        },
                        new Button
                        {
                            Text = "Cancel",
                            BackgroundColor = Color.FromHex("#2196F3"), // Set your preferred color
                            TextColor = Color.White, // Set your preferred text color
                            Command = new Command(() => CloseCustomAlert())
                        }
                    }
                }
            }
            };

            var page = new ContentPage
            {
                Content = new ScrollView { Content = customAlert }
            };

            async void CloseCustomAlert()
            {
                await Navigation.PopAsync();
            }

            Navigation.PushAsync(page);
        }

    }
}
