using MovieApi.Models;

namespace MovieApi.Models
{
    public class AppUser
    {
        public long Id { get; set; }
        public string FullName { get; set; }   
        public string Phone { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; }
        public string Role { get; set; }
    }
}
