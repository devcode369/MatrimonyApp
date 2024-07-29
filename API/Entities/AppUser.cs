using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class AppUser : IdentityUser<int>
    {
        public AppUser()
        {
            Photos = new List<Photo>();
            LikedByUsers = new List<UserLike>();
            LikedUsers = new List<UserLike>();
            MessageSent = new List<Message>();
            MessageReceived = new List<Message>();


        }

        // identity inherited so it will take care
        // public int Id { get; set; }
        // public string UserName{get;set;}
        // public byte[] PasswordHash{get;set;}
        // public byte[] PasswordSalt{get;set;}
        public DateOnly DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime LastActive { get; set; } = DateTime.UtcNow;
        public string Gender { get; set; }
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Photo> Photos { get; set; }


        public List<UserLike> LikedByUsers { get; set; }

        public List<UserLike> LikedUsers { get; set; }

        public List<Message> MessageSent { get; set; }
        public List<Message> MessageReceived { get; set; }

        public ICollection<AppUserRole> UserRoles { get; set; }

        // public int GetAge()
        // {
        //     return DateOfBirth.CalculateAge();
        // }

    }
}