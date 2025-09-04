using MovieApi.Models;
using MovieAPI.Models;
using Newtonsoft.Json;

namespace MovieApi.Models
{
    public class Movie
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }

        [JsonProperty("releaseDate")]
        public DateTime ReleaseDate { get; set; }
        public int Amount { get; set; }
        public string ImageUrl { get; set; }
        public string Genre { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Director { get; set; }
        public string Stars { get; set; }
        public string TrailerUrl { get; set; }
        public double Rating { get; set; }
    }
}
