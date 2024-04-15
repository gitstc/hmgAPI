namespace hmgAPI.DTOs;

//returned to the client after logging in and register so we can review the user info in client
//we need an automapper  for this class, otherwise it will be pain to map manually
public class UserDto
{
    public string Username { get; set; }
    public string Token { get; set; }
    //user role is passed along inside the token 
    //so we will get hold of it in the client
}