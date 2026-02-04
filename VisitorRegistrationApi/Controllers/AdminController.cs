using Microsoft.AspNetCore.Mvc;
using VisitorRegistrationService;
using VisitorRegistrationShared.Dtos.Admin;

namespace VisitorRegistrationApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AdminService adminService;
        public AdminController(AdminService adminService)
        {
            this.adminService = adminService;
        }

        [HttpGet("check")]
        public IActionResult CheckAuth()
        {
            var hasCookie = Request.Cookies.ContainsKey("AdminAuth");
            return Ok(new { IsAdmin = hasCookie });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginAdminDto loginAdminDto)
        {
            var isValid = await adminService.ValidateAdminLogin(loginAdminDto);

            if (isValid)
            {
                Response.Cookies.Append("AdminAuth", "true", new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.UtcNow.AddHours(2)
                });

                return Ok(new { Message = "Login successful" });
            }

            return Unauthorized(new { Message = "Ongeldige inloggegevens" });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AdminAuth");
            return Ok(new { Message = "Logged out" });
        }
    }
}
