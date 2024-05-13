using Microsoft.AspNetCore.Identity;

namespace hmgAPI.Entities
{
    public class AppRole : IdentityRole<int>
    {
        //roles can have many users
        //to configure 
        //many to many relationship between users and roles
        public ICollection<AppUserRole> UserRoles { get; set; }
    }
}