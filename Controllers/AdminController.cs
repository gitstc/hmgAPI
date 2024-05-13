using hmgAPI.Entities;
using hmgAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace hmgAPI.Controllers
{
    public class AdminController : BaseApiController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IOracleService _oracleService;
        public AdminController(UserManager<AppUser> userManager, IOracleService oracleService)
        {
            _userManager = userManager;
            _oracleService = oracleService;
        }
        //only admins can access this endpoint based on policy set in program.cs
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet]
        public async Task<ActionResult> GetUsersWithRoles()
        {
            var users = await _userManager.Users
            .OrderBy(u => u.UserName)//get users in alphabatical order

            //Users (AppUser) table has a related table called (userRoles)
            // which is the (join table) that relates (AppUser) and (AppRoles) tables 
            // we need to project through (userRoles) table from (AppUser)
            //so then we can access the other table related to it (Roles table)
            .Select(u => new
            {
                u.Id,
                Username = u.UserName,
                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
            }).ToListAsync();

            return Ok(users); //returns users alongside with their roles
        }

        //only admins can edit user roles
        [Authorize(Policy = "RequireAdminRole")]
        [HttpPost("edit-roles/{username}")]
        //string username, username of user we wanna edit 
        //string roles, comma separated list of roles
        //we will get roles from query string
        public async Task<ActionResult> EditRoles(string username,
         [FromQuery] string roles)
        {

            if (string.IsNullOrEmpty(roles))
                return BadRequest("you must select atleast one role for user " + username);

            //split comma seperated list that came from the client
            var selectedRoles = roles.Split(",").ToArray();

            //look for user 
            var user = await _userManager.FindByNameAsync(username);

            //if user not found
            if (user == null) return NotFound();

            //if user found, get the roles he is currently in
            var userRoles = await _userManager.GetRolesAsync(user);

            //add roles to user, except the ones he is currently in 
            //if user is an admin already it will be excluded but added to another role 
            var result = await _userManager
            .AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if (!result.Succeeded) return BadRequest("failed to add to roles");

            //if admin removed user from roles except the roles he added user to 
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

            if (!result.Succeeded) return BadRequest("failed to remove from roles");

            //return updated list of roles user is in
            return Ok(await _userManager.GetRolesAsync(user));

        }

     
    }
}