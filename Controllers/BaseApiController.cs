using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace hmgAPI.Controllers;
//to not repeat same steps each time a controller is created
//just inherit from this class 
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{

}