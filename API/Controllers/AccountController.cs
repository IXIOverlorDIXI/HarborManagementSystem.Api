using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Extensions;
using API.Services;
using Domain.Consts;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly DataContext _context;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            TokenService tokenService, DataContext context)
        {
            _tokenService = tokenService;
            _context = context;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);
            
            if (user == null)
            {
                return Unauthorized("Invalid email.");
            }
            
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            
            if (result.Succeeded)
            {
                return Ok(await CreateUserObject(user));
            }
            
            return Unauthorized("Invalid password.");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email already taken.");
                return ValidationProblem();
            }

            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                ModelState.AddModelError("username", "Username already taken.");
                return ValidationProblem();
            }

            var subscription = await _context.Subscriptions
                .FirstOrDefaultAsync(x => x.Price == 0);

            if (subscription == null)
            {
                return BadRequest("Failure to find subscription.");
            }
            
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username,
                SubscriptionId = subscription.Id
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                return BadRequest("Problem registering user.");
            }
            
            var roleResult = await _userManager.AddToRoleAsync(user, Roles.User);

            if (!roleResult.Succeeded)
            {
                return BadRequest("Problem registering user.");
            }

            return Ok(await CreateUserObject(user));
        }

        [HttpPut("changePassword")]
        public async Task<IActionResult> ChangePassword(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                return NotFound("User with this email doesn't exist.");
            }

            if (user.Id != User.FindFirstValue(ClaimTypes.NameIdentifier))
            {
                return Forbid();
            }
            
            var passwordValidator = HttpContext.RequestServices.GetService(typeof(IPasswordValidator<AppUser>)) as IPasswordValidator<AppUser>;
            var passwordHasher = HttpContext.RequestServices.GetService(typeof(IPasswordHasher<AppUser>)) as IPasswordHasher<AppUser>;

            var validationResult = await passwordValidator.ValidateAsync(_userManager, user, loginDto.Password);

            if (!validationResult.Succeeded)
            {
                validationResult.ToModelState(ModelState);

                return ValidationProblem();
            }

            user.PasswordHash = passwordHasher.HashPassword(user, loginDto.Password);
            await _userManager.UpdateAsync(user);

            return Ok("Password has changed.");
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(x => x.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

            return Ok(await CreateUserObject(user));
        }

        private async Task<UserDto> CreateUserObject(AppUser appUser)
        {
            var response = _userManager.Users
                .Include(user => user.Photo)
                .FirstOrDefault(x => x.Id == appUser.Id);

            return new UserDto
            {
                DisplayName = response?.DisplayName,
                Token = _tokenService.CreateToken(response),
                Username = response?.UserName,
                Photo = (response?.Photo?.Url) ?? DefaultFileLinks.DefaultUserPhoto,
                IsAdmin = await _userManager.IsInRoleAsync(appUser, Roles.Admin)
            };
        }
    }
}