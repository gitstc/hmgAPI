using Microsoft.AspNetCore.Mvc;
using hmgAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using hmgAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using hmgAPI.Entities;
using AutoMapper;
using hmgAPI.Controllers;
using hmgAPI.Data;
using System.Linq.Expressions;
using AutoMapper.Configuration.Annotations;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;

//since we have class RegisterDto we can bind objects in it(username,password)
// with [ApiController]
// as we wanna send them in the body of request not in url 

namespace API.Controllers;
public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _dataContext;

    public AccountController
    (
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    DataContext dataContext
    )
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _dataContext = dataContext;
    }


    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        //start a new transaction 
        using (var transaction = _dataContext.Database.BeginTransaction())
        {
            try
            {
                // Check if merchant already exists
                var merchant = await _dataContext.Merchants
                    .SingleOrDefaultAsync(m => m.CompanyName == registerDto.CompanyName);

                // If merchant exists, return a notification
                if (merchant != null)
                {
                    return BadRequest("A merchant with this company name already has an account.");
                }

                // Create a new merchant
                merchant = new AppMerchant
                {
                    // Map properties of registerDto with AppMerchant
                    CompanyName = registerDto.CompanyName,
                    CoRegistrationNo = registerDto.CoRegistrationNo,

                };

                //add merchant to props
                _dataContext.Merchants.Add(merchant);
                //save changes to database
                await _dataContext.SaveChangesAsync();


                // Map properties of registerDto with AppUser
                var user = new AppUser()
                {
                    UserName = registerDto.Mobile,
                    PhoneNumber = registerDto.Mobile,
                    Email = registerDto.Email,
                    Address = registerDto.Address,
                    MerchantId = merchant.MerchantId,
                };

                // Proceed with creating a new account
                var createAccount = await _userManager.CreateAsync(user, registerDto.Password);

                if (!createAccount.Succeeded)
                    return BadRequest(createAccount.Errors);

                // Add user to role merchant
                var rolesResult = await _userManager
                .AddToRoleAsync(user, "Admin");

                if (!rolesResult.Succeeded)
                    return BadRequest(rolesResult.Errors);

                //If all operations succeed, commit the transaction,
                //to apply the changes to the database permanently
                transaction.Commit();

                // Return user DTO with token
                return new UserDto
                {
                    Username = user.UserName,
                    Token = await _tokenService.CreateToken(user),
                };
            }
            catch (Exception ex)
            {
                //If any operation fails, roll back the transaction,
                // undoing the changes made by the transaction.
                await transaction.RollbackAsync();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.InnerException + " " + ex.StackTrace);
            }
        }
    }

    //method to check if user already exists 
    // public async Task<bool> UserExists(string username)
    // {
    //     return await _userManager.Users
    //     .AnyAsync(x => x.UserName == username.ToLower());
    // }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        if (string.IsNullOrEmpty(loginDto.Username))
        {
            return BadRequest("Username cannot be null or empty.");
        }
        if (string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest("Password cannot be null or empty.");
        }

        //username in database(x.userName) matches username passed from client{loginDto}? 
        var user = await _userManager.Users
        .Include(m => m.Merchant)
        .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

        //return unauthorized if user is invalid
        if (user == null) return Unauthorized("Invalid username");

        //check if pass of user if is valid
        var result = await
         _userManager.CheckPasswordAsync(user, loginDto.Password);

        //if not valid (didnt success) return incorrect password 
        if (!result) return Unauthorized("Incorrect Password");

        //return username and token of user to the client if login info correct
        return new UserDto
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            Merchant = new MerchantDto
            {
                CompanyName = user.Merchant.CompanyName,
                CoRegistrationNo = user.Merchant.CoRegistrationNo
            }
        };
        // return Ok(new Dictionary<string, object>(){
        //     { "user", user.Merchant },
        //     { "username", user.UserName },
        //     { "token", await _tokenService.CreateToken(user) }
        // });
    }
}

