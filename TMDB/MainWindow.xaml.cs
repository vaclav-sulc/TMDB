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
                    string id = result["id"].ToString();
                    string releaseDate = result["release_date"].ToString();
                    string voteAverage = result["vote_average"].ToString();
                    string popularity = result["popularity"].ToString();

                    MovieList.Items.Add($"\n\nTitle: {title}\n ID: {id}\nRelease date: {releaseDate}\nRating: {voteAverage}\nPopularity: {popularity}\n");
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

        private void MovieList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var apiKey = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJkMTY3MDBiNDQyNzYzYzcwMDk5NDNkN2JhNzFmM2ZiYyIsIm5iZiI6MTczNjg2MDkxNy41NzUsInN1YiI6IjY3ODY2NGY1MjI1NjAyM2RmZDRlOTM0NSIsInNjb3BlcyI6WyJhcGlfcmVhZCJdLCJ2ZXJzaW9uIjoxfQ.eWUC5gr5PwKtpa5zjAVxRiEABF3u69KZIEqxQjroHFc";
            int id = int.Parse(MovieList.SelectedItem.ToString().Split('\n')[3].Split(' ')[2]);
            MovieList.Visibility = Visibility.Hidden;

            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.themoviedb.org/3/movie/{id}"),
                Headers =
                {
                    { "accept", "application/json" },
                    { "Authorization", $"Bearer {apiKey}" }
                },
            };

            using (var response = client.SendAsync(request).Result)
            {
                response.EnsureSuccessStatusCode();
                var body = response.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(body);
                string title = json["title"].ToString();
                string releaseDate = json["release_date"].ToString();
                string voteAverage = json["vote_average"].ToString();
                string popularity = json["popularity"].ToString();
                string overview = json["overview"].ToString();
                FilmInfo.Text = $"Title: {title}\nRelease date: {releaseDate}\nRating: {voteAverage}\nPopularity: {popularity}\nOverview: {overview}";
                FilmInfo.Visibility = Visibility.Visible;
                Back.IsEnabled = true;
                TopRated.IsEnabled = false;
                Popular.IsEnabled = false;
                PrevPage.IsEnabled = false;
                NextPage.IsEnabled = false;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back.IsEnabled = false;
            FilmInfo.Visibility = Visibility.Hidden;
            MovieList.Visibility = Visibility.Visible;
            FilmInfo.Text = "";
            TopRated.IsEnabled = true;
            Popular.IsEnabled = true;
            PrevPage.IsEnabled = true;
            NextPage.IsEnabled = true;

        }
    }
}
