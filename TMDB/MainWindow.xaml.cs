using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace TMDB
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Action_Click(object sender, RoutedEventArgs e)
        {
            Action.Content = "Top Rated Movies";
            Action.IsEnabled = false;
            var apiKey = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJkMTY3MDBiNDQyNzYzYzcwMDk5NDNkN2JhNzFmM2ZiYyIsIm5iZiI6MTczNjg2MDkxNy41NzUsInN1YiI6IjY3ODY2NGY1MjI1NjAyM2RmZDRlOTM0NSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.eWUC5gr5PwKtpa5zjAVxRiEABF3u69KZIEqxQjroHFc";
            var url = "https://api.themoviedb.org/3/movie/top_rated";

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {apiKey}" },
                },
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                JObject json = JObject.Parse(body);
                var results = json["results"];

                foreach (var result in results)
                {
                    string title = result["title"].ToString();
                    string releaseDate = result["release_date"].ToString();
                    string voteAverage = result["vote_average"].ToString();
                    string popularity = result["popularity"].ToString();

                    MovieList.Items.Add($"\n\nTitle: {title}\nRelease date: {releaseDate}\nRating: {voteAverage}\nPopularity: {popularity}\n");
                }
            }
        }
    }
}
