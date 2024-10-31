using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.Models.DTO;

namespace NZWalks.API.Controllers
{
    [Route( "api/[controller]" )]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;

        public AuthController(UserManager<IdentityUser> userManager)
        {
            this.userManager = userManager;
        }

        //POST: api/Auth/register
        [HttpPost]
        [Route( "register" )]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };
            var identityResult = await userManager.CreateAsync( identityUser, registerRequestDto.Password );

            if(identityResult.Succeeded)
            {
                //TODO: add roles to this User
                if(registerRequestDto.Roles != null && registerRequestDto.Roles.Any( ))
                {
                    identityResult = await userManager.AddToRolesAsync( identityUser, registerRequestDto.Roles );
                    if(identityResult.Succeeded)
                    {
                        return Ok( "User was registered! Please login. " );
                    }
                }

            }
            return BadRequest( identityResult.Errors );

        }
    
        // POST: api/Auth/login
        [HttpPost]
        [Route( "login" )]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync( loginRequestDto.Username );
            if(user != null && await userManager.CheckPasswordAsync( user, loginRequestDto.Password ))
            {
                //TODO: generate token
                return Ok( "User was logged in!" );
            }
            return BadRequest( "Username or password is incorrect!" );
        }
    }
}
