using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DemoApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    [HttpGet(Name = "GetDashboard")]
    public IActionResult GetDashboard()
    {
        var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? "User";
        
        return Ok(new { 
            message = role == "Admin" ? "Admin Dashboard" : "User Dashboard",
            role = role
        });
    }
} 