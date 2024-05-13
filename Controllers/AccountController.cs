using Microsoft.AspNetCore.Mvc;
using hmgAPI.DTOs;
using Microsoft.EntityFrameworkCore;
using hmgAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using hmgAPI.Entities;
using AutoMapper;
using hmgAPI.Controllers;
using hmgAPI.Data;
using hmgAPI.Services;


namespace API.Controllers;
public class AccountController : BaseApiController
{
    private readonly ITokenService _tokenService;
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly IOracleService _oracleService;


    public AccountController
    (
    UserManager<AppUser> userManager,
    ITokenService tokenService,
    DataContext dataContext,
    IMapper mapper,
    IOracleService oracleService
    )
    {
        _tokenService = tokenService;
        _userManager = userManager;
        _dataContext = dataContext;
        _mapper = mapper;
        _oracleService = oracleService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthUserDto>> Register(RegisterDto registerDto)
    {
        //start a new transaction 
        //(using) to collect garbage from Memory 
        using (var transaction = _dataContext.Database.BeginTransaction())
        {
            try
            {
                // Check if merchant already exists
                var merchant = await _dataContext.Merchants
                    .SingleOrDefaultAsync(m => m.CoRegistrationNo == registerDto.CoRegistrationNo);

                // If merchant exists, return a notification
                if (merchant != null)
                {
                    return BadRequest("A merchant with this company number already has an account.");
                }

                var erpMerchant = await _oracleService.GetMerchantByMobileNumber(registerDto.Mobile);
                if(erpMerchant == null) {
                    return NotFound("Merchant does not exist in ERP");
                }

                // Create a new merchant
                merchant = new AppMerchant
                {
                    // Map properties of registerDto with AppMerchant
                    CompanyName = registerDto.CompanyName,
                    CoRegistrationNo = registerDto.CoRegistrationNo,
                    MerchantCode = erpMerchant.Code
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

                // Return user with token
                return new AuthUserDto
                {
                    User = new UserDto
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        Mobile = user.PhoneNumber,
                        Address = user.Address,

                    }, // entire user object
                    Token = await _tokenService.CreateToken(user), //return user's token
                    Merchant = new MerchantDto
                    {
                        MerchantId = user.MerchantId,
                        CompanyName = user.Merchant.CompanyName,
                        CoRegistrationNo = user.Merchant.CoRegistrationNo,
                    } // return merchant info related to that user
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

    [HttpPost("login")]
    public async Task<ActionResult<AuthUserDto>> Login(LoginDto loginDto)
    {
        if (string.IsNullOrEmpty(loginDto.Username))
        {
            return BadRequest("Username cannot be null or empty.");
        }
        if (string.IsNullOrEmpty(loginDto.Password))
        {
            return BadRequest("Password cannot be null or empty.");
        }

        //username in database(x.userName) matches username passed from client{loginDto} ??
        var user = await _userManager.Users
        .Include(m => m.Merchant)
        .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

        //return unauthorized if user is invalid
        if (user == null) return Unauthorized("Invalid username");

        //check if pass of user if is valid
        var result = await
         _userManager.CheckPasswordAsync(user, loginDto.Password);

        //if not valid (didnt succeed) return incorrect password 
        if (!result) return Unauthorized("Incorrect Password");

        //return user and token of user to the client if login info correct
        return new AuthUserDto
        {
            User = new UserDto
            {
                Username = user.UserName,
                Email = user.Email,
                Mobile = user.PhoneNumber,
                Address = user.Address,

            }, // entire user object
            Token = await _tokenService.CreateToken(user), //return user's token
            Merchant = new MerchantDto
            {
                MerchantId = user.MerchantId,
                CompanyName = user.Merchant.CompanyName,
                CoRegistrationNo = user.Merchant.CoRegistrationNo,
            } // return merchant info related to that user
        };
    }
}

