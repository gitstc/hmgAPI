using hmgAPI.Interfaces;
using hmgAPI.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using hmgAPI.Data;
using Microsoft.EntityFrameworkCore;

//3 parts for token(string seperated with 3 dots)
// first part(before first dot) contains algo used to encrypt signature in 3rd part of token
//second part(before second dot) contains claims (username)(user role) and dates(when token expires)
//if token expiry was in 7 days, after week user has to login again to get a new token
//third part of token is the signature 

namespace hmgAPI.Services;
public class TokenService : ITokenService
{
    //same key encrypts and decrypts data, 
    //since the server is responsible for encryting and decrypting the key we use it
    private readonly SymmetricSecurityKey _key;

    private readonly DataContext _dataContext;

    private readonly UserManager<AppUser> _userManager;

    //here we stored our secret key that we'll use to sign our token inside config
    public TokenService(IConfiguration config,
    UserManager<AppUser> userManager,
    DataContext dataContext)
    {
        _userManager = userManager;
        _dataContext = dataContext;

        //accessed from our config file (appsettings.json)
        _key = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(config.GetValue<string>("TokenKey")));
    }

    public async Task<string> CreateToken(AppUser user)
    {
        //get merchant info related to that user
        var merchant = await _dataContext.Merchants
        .FirstAsync(u => u.MerchantId == user.MerchantId);

        // get role of user
        var roles = await _userManager.GetRolesAsync(user);

        //what user claims about himself (i claim that my name is)
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()), // NameId = "id"
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName), // UniqueName = "Username" 
            new Claim("merchantCode", merchant.MerchantCode), // merchantCode = "merchant code"
        };

        //is his role as he claims?
        //add his role to the last part of list of claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        //We are going to sign this token with (HmacSha512Signature)
        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        //description of what will token that we will return
        // will include:
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims), //store user claims
            Expires = DateTime.Now.AddDays(7), //token expires after a week and we need to log in again to get a new token
            SigningCredentials = creds //how token is signed
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor); // create our token

        return tokenHandler.WriteToken(token); // pass our token
    }
}