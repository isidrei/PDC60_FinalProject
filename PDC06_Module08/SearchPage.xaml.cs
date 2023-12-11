using System;
using System.Collections.Generic;
using System.Net.Http;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using Xamarin.Forms.Xaml;

using PDC06_Module08;

namespace PDC06_Module08
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        private const string url_search = "http://192.168.100.164/pdc6/api-search.php";
        private HttpClient _Client = new HttpClient();
        private ObservableCollection<Post> _posts;
        private StudentRecordsPage _studentRecordsPage;

        public SearchPage(StudentRecordsPage studentRecordsPage)
        {
            InitializeComponent();
            BindingContext = this;
            NoResultsLabel.IsVisible = false;
            _studentRecordsPage = studentRecordsPage; // Store the instance of StudentRecordsPage
        }

        public class ResponseObject
        {
            public bool status { get; set; }
            public JToken data { get; set; }
            public string message { get; set; }
        }

        private async void OnSearchTextchanged(object sender, TextChangedEventArgs e)
{
    string searchQuery = e.NewTextValue;
    if (string.IsNullOrWhiteSpace(searchQuery))
    {
        // Handle empty search query
        NoResultsLabel.IsVisible = false; // Hide the label when the search query is empty
    }
    else
    {
        try
        {
            // Include all search parameters in the API request
            var searchUrl = $"{url_search}?name={searchQuery}&roll_number={searchQuery}&age={searchQuery}&email={searchQuery}";
            System.Diagnostics.Debug.WriteLine($"Search URL: {searchUrl}");

            var content = await _Client.GetStringAsync(searchUrl);
            var responseObject = JsonConvert.DeserializeObject<ResponseObject>(content);

            if (responseObject.status)
            {
                var searchResult = JsonConvert.DeserializeObject<List<Post>>(responseObject.data.ToString());
                _posts = new ObservableCollection<Post>(searchResult);
                PostListView.ItemsSource = _posts;

                // Show or hide the label based on search results
                NoResultsLabel.IsVisible = _posts.Count == 0;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"Error: {responseObject.message}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}
        private async void OnItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is Post selectedPost)
            {
                // Use the existing instance of StudentRecordsPage
                _studentRecordsPage.ShowStudentDetails(selectedPost);
            }
        }


    }
}