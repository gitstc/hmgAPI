using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using hmgAPI.Entities;
using Microsoft.AspNetCore.Identity;

namespace hmgAPI.Data
{
    //to tell identityDbContect about classes we created and their types
    // IdentityDbContext<AppUser, AppRole, int> 
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        { }

        // Identity takes care of this
        // public DbSet<AppUser> Users { get; set; }

        public DbSet<AppMerchant> Merchants { get; set; }



        // EF doesnt do good with many to many relationships
        //we made our own config
        //so we can override identity defualt tables to edit them
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            //configure join table between appUser and appRole( many to many relationship)
            ///side of relationship
            builder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles) // user has many roles
            .WithOne(u => u.User) // each role can belong to one user 
            .HasForeignKey(ur => ur.UserId) //foreign key is userId
            .IsRequired(); //for foreign key not to be null

            ///other side of relationship
            builder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles) // role have many users
            .WithOne(u => u.Role) // each user belong to one role
            .HasForeignKey(ur => ur.RoleId) //foreign key is RoleId
            .IsRequired();//for foreign key not to be null


            builder.Entity<AppMerchant>()
            .HasMany(u => u.Users) // merhcant has many users
            .WithOne(m => m.Merchant)  // eash user belongs to one merchant
            .HasForeignKey(m => m.MerchantId) //foreign key is merchantId 
            .IsRequired();//for foreign key not to be null

        }
    }
}