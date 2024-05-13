using System.Text;
using hmgAPI.Data;
using hmgAPI.Entities;
using hmgAPI.Interfaces;
using hmgAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//CORS
builder.Services.AddCors();

//TOKEN
builder.Services.AddScoped<ITokenService, TokenService>();
//Firas - ORACLE
builder.Services.AddScoped<IOracleService, OracleService>();

//AUTOMAPPER
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


//datacontext service => so we can use it anywhere in app 
//configuration is appsettings.json
builder.Services.AddDbContext<DataContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


//IDENTITY CONFIG
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    // opt.SignIn.
    // opt.User.AllowedUserNameCharacters=
}
).AddRoles<AppRole>()
.AddRoleManager<RoleManager<AppRole>>()
.AddEntityFrameworkStores<DataContext>(); //create tables related to entity in database


//TOKEN CONFIG

//JWT Authentication so that
// when user login they should (have a token to access a specific api)
//if they don't have a token this mean they are not logged in
//so they can't access a specific api 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    //inside this we specify all rules about how server should validate this token
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //server checks token signingKey
        //to see if token i signed by issuer
        ValidateIssuerSigningKey = true,
        //specify what is our issuer signing key 
        //which is our token key
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding
        .UTF8.GetBytes(builder.Configuration.GetValue<string>("TokenKey"))),
        ValidateIssuer = false,
        ValidateAudience = false
    };
}
);

//for [Authorize(RequireAdminRole)]
builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("RequireAdminRole",
     policy => policy.RequireRole("Admin"));

});


var app = builder.Build();

//CORS CONFIG
app.UseCors(builder => builder
.AllowAnyHeader()
.AllowAnyMethod()
.AllowAnyOrigin());

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

//USERMANAGER CONFIG 
var userManager = services.GetRequiredService<UserManager<AppUser>>;

//ROLEMANAGER CONFIG 
var roleManager = services.GetRequiredService<RoleManager<AppRole>>;


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.MapFallbackToController("Index", "Fallback");
// app.UseHttpsRedirection();

//for authentication user by their tokens
//to access [authorize] controller
//for the user to send token he has to prove he's valid 
app.UseAuthentication(); // do you have a valid token?
app.UseAuthorization(); // if you have a token what are you allowed to do?

app.UseDefaultFiles(new DefaultFilesOptions {
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "browser")
    ),
    DefaultFileNames = new List<string> { "index.html" }
});
app.UseStaticFiles(new StaticFileOptions {
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "browser")
    )
});

app.Run();

