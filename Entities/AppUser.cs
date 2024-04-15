using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace hmgAPI.Entities;

//this class will be for login and register
//identity user class will deal by itself
// with id,username,password hash,password salt, email, and phone  
// IdentityUser<int> => we tell identity user that id of user is int 
public class AppUser : IdentityUser<int>
{
   //username is mobile number
   [Column(TypeName = "nvarchar(100)")]
   public string Address { get; set; }

   // to configure 
   //many to many relationship between users and roles
   public ICollection<AppUserRole> UserRoles { get; set; }

   // to configure 
   //many to many relationship between users and merchants
   //F.K for many side of relationship 
   //merchant can have many users(accounts)
   public int MerchantId { get; set; }

   //navigation property
   public AppMerchant Merchant { get; set; }

}