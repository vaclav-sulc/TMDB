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
using System.Collections.Specialized;

namespace TMDB
{
    /// <summary>
    /// Interakční logika pro MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int page = 1;
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void TopRated_Click(object sender, RoutedEventArgs e)
        {
            MovieList.Items.Clear();
            await ApiRequest($"https://api.themoviedb.org/3/movie/top_rated?language=en-US&page={page}");
        }

        private async Task ApiRequest(string url)
        {
            var apiKey = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJkMTY3MDBiNDQyNzYzYzcwMDk5NDNkN2JhNzFmM2ZiYyIsIm5iZiI6MTczNjg2MDkxNy41NzUsInN1YiI6IjY3ODY2NGY1MjI1NjAyM2RmZDRlOTM0NSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.eWUC5gr5PwKtpa5zjAVxRiEABF3u69KZIEqxQjroHFc";
            Page.Text = $"Page: {page}";
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {apiKey}" }
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

        private async void Popular_Click(object sender, RoutedEventArgs e)
        {
            MovieList.Items.Clear();
            await ApiRequest("https://api.themoviedb.org/3/movie/popular");
        }

        private void PrevPage_Click(object sender, RoutedEventArgs e)
        {
            MovieList.Items.Clear();
            if  (page != 1)
            {
                page--;
            }
            TopRated_Click(sender, e);
        }

        private void NextPage_Click(object sender, RoutedEventArgs e)
        {
            MovieList.Items.Clear();
            page++;
            TopRated_Click(sender, e);
        }
    }
}
