    using Erp.Shared.Interfaces;
    using ERP.DTOs.Auth;
    using ERP.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace ERP.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class AuthController :ControllerBase
        {
            private readonly IConfiguration _configuration;
            private readonly IAuthService _authService;
            public AuthController(IAuthService authService,IConfiguration configuration)
            {
                _authService = authService;
                _configuration = configuration;
            }
            [HttpPost("login")]
            [AllowAnonymous]
            public async Task<IActionResult> Login(LoginRequest request)
            {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
            }
            [HttpGet("me")]
            [Authorize]
            public IActionResult GetMyProfile([FromServices] ICurrentUserService currentUser) { 
                return Ok(new
                {
                    Message = "You have successfully breached the mainframe.",
                    UserId = currentUser.UserId,
                    TenantId = currentUser.TenantId,
                    IsSuperAdmin = currentUser.IsSuperAdmin
                });
            }
        }
    }
