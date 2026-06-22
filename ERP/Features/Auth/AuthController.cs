    using Erp.Shared.Interfaces;
    using ERP.Features.Auth.DTOs;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    namespace ERP.Features.Auth
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
            public async Task<IActionResult> GetMyProfile([FromServices] ICurrentUserService currentUser) {
            if(currentUser.UserId == null)
            {
                return Unauthorized();
            }
            var response = await _authService.GetMyProfileAsync(currentUser.UserId.Value);
                return Ok(response);
        }
        }
    }
