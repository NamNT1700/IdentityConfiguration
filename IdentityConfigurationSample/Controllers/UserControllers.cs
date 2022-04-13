using AutoMapper;
using IdentityConfigurationSample.Authentication;
using IdentityConfigurationSample.Data;
using IdentityConfigurationSample.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,User")]
    //[Authorize]
    //[Authorize(Roles = RolesStorage.User)]
    public class UserControllers : ControllerBase
    {

        private readonly ILogger<UserControllers> _logger;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public IConfiguration _configuration;
        public IMapper _mapper;
        public UserControllers(ILogger<UserControllers> logger, UserManager<IdentityUser> userManager, IMapper mapper,
                                RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDTO user)
        {
            try
            {
                var userExist = await _userManager.FindByNameAsync(user.UserName);
                if (userExist != null)
                {
                    return BadRequest("user already exist");
                }
                var newUser = _mapper.Map<IdentityUser>(user);
                var result = await _userManager.CreateAsync(newUser, user.PassWord);

                if (result.Succeeded)
                {
                    if (await _roleManager.FindByNameAsync(RolesStorage.User) == null)
                       await _roleManager.CreateAsync(new IdentityRole(RolesStorage.User));
                    await _userManager.AddToRoleAsync(newUser, RolesStorage.User);
                    return Ok(new { Description = "Register successful", result = 1, });
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("UpdateUser")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserData updateUsersData)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(updateUsersData.UseName);
                if (user == null)
                {
                    return BadRequest("wrong username");
                }
                var isUserValid = await _userManager.CheckPasswordAsync(user, updateUsersData.PassWord);
                if (isUserValid == false)
                {
                    return BadRequest("wrong password");
                }
                user = _mapper.Map(updateUsersData.UpdateUsersDTO, user);
                var isEmailExist = await _userManager.FindByEmailAsync(updateUsersData.UpdateUsersDTO.Email);
                if (isEmailExist != null)
                {
                    return BadRequest("email already exist");
                }
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok(new { Description = "Update successful", result = 1 });
                }
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetUser")]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> GetUser(string usename, string password)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(usename);
                if (user == null)
                {
                    return BadRequest("wrong username");
                }
                var trueUser = await _userManager.CheckPasswordAsync(user, password);
                if (trueUser == false)
                {
                    return BadRequest("wrong password");
                }
                var result = _mapper.Map<UserDTO>(user);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("DeleteUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(string useName) 
        {
            try
            {
                var userExelt = await _userManager.FindByNameAsync(useName);
                if (userExelt == null)
                {
                    return BadRequest("wrong user name");
                }
                var result = await  _userManager.DeleteAsync(userExelt);
                if(result.Succeeded)  return Ok(new { Description = "Delete successful", result = 1 });
                return BadRequest(result.Errors);

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                var userExist = await _userManager.FindByNameAsync(login.Usename);
                if (userExist == null)
                {
                    return BadRequest("wrong user name");
                }
                var truePass = await _userManager.CheckPasswordAsync(userExist,login.Password);
                if (truePass == false)
                {
                    return BadRequest("wrong pass");
                }
                TokenManager tokenGenerate = new TokenManager(_configuration,_roleManager,_userManager);
                var token = await tokenGenerate.GenerateToken(userExist);
                var tkens = new Tokens();
                //var result = tkens.accessToken;
                tkens.accessToken = token;
                return Ok(tkens);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }

        }
       

    }
}
