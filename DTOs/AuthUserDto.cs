
namespace hmgAPI.DTOs
{
    public class AuthUserDto
    {
        public UserDto User { get; set; }
        public string Token { get; set; }
        //user role is passed along inside the token 
        //so we will get hold of it in the client

        public MerchantDto Merchant { get; set; }

    }
}