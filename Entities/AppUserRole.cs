using Microsoft.AspNetCore.Identity;

namespace hmgAPI.Entities
{
    //join table between appUser and AppRole 
    //as relationship is many to many
    public class AppUserRole : IdentityUserRole<int>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }

    }
}