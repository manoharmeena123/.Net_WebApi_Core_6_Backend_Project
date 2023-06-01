﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NzWalkWebApi.Models.DTO;
using NzWalkWebApi.Repositories;

namespace NzWalkWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;

        private readonly ITokenRepository tokenRepository;

        public AuthController(UserManager<IdentityUser> userManager, ITokenRepository tokenRepository)
        {
            this.userManager = userManager;
        this.tokenRepository = tokenRepository;
        }



        //POST: Register==================================================================>
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var identityUser = new IdentityUser
            {
                UserName = registerRequestDto.Username,
                Email = registerRequestDto.Username
            };
            var identityResult = await userManager.CreateAsync(identityUser, registerRequestDto.Password);
          if(identityResult.Succeeded)
            {
                //Add roles this User
                if(registerRequestDto.Roles != null && registerRequestDto.Roles.Any())
                {
                    identityResult = await userManager.AddToRoleAsync(identityUser, registerRequestDto.Roles);
                    if(identityResult.Succeeded)
                    {
                        return Ok("User was registered ! Please Login.");
                    }
                
                }
            }
            return BadRequest("Somethig went Wrong..");

        }

        //Post :Login =========================================================================>

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var user = await userManager.FindByEmailAsync(loginRequestDto.Username);
            if(user!= null)
            {
          
                var checkPasswordResult =   await userManager.CheckPasswordAsync(user, loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    // Get  Roles for this user
              var roles = await userManager.GetRolesAsync(user);
                    if(roles != null)
                    {   
                        // Create Token
                    var jwtToken =   tokenRepository.CreateJWTToken(user,roles.ToList());
                        var response = new LoginResponseDto
                        {
                            JwtToken = jwtToken
                        };
                         return Ok(response);
                    }
                
                       return Ok("Login succesfully ...");
                }
            }
            return BadRequest("Username or password incorrect");
        }


    }
}
