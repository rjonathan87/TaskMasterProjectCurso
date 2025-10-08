using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskMaster.Application.DTOs;
using TaskMaster.Application.Services;
using TaskMaster.Domain.Entities;

namespace TaskMaster.Presentation.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        // Inyectamos los servicios de Identity y nuestro servicio de token
        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new User { UserName = request.Email, Email = request.Email };
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponse { Success = false, Errors = result.Errors.Select(e => e.Description) });
            }

            // Por defecto, asignamos el rol "User" a los nuevos usuarios
            await _userManager.AddToRoleAsync(user, "User");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return Ok(new AuthResponse { Success = true, Token = token, UserId = user.Id.ToString() });
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) 
            {
                return Unauthorized(new AuthResponse { Success = false, Errors = new[] { "Credenciales inválidas." } });
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized(new AuthResponse { Success = false, Errors = new[] { "Credenciales inválidas." } });
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateJwtToken(user, roles);

            return Ok(new AuthResponse { Success = true, Token = token, UserId = user.Id.ToString() });
        }
    }
}