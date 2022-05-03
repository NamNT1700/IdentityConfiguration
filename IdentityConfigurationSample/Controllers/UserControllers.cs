using AutoMapper;
using IdentityConfigurationSample.Authentication;
using IdentityConfigurationSample.Data;
using IdentityConfigurationSample.DTO;
using IdentityConfigurationSample.Res;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
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
    [EnableCors("Origins")]
    [ApiController]
    //[Authorize(Roles = "Admin,User")]
    //[Authorize]
    //[Authorize(Roles = RolesStorage.User)]
    public class UserControllers : ControllerBase
    {

        private readonly ILogger<UserControllers> _logger;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private IConfiguration _configuration;
        private IMapper _mapper;
        //private IErrorResponse _errorResponse;
        public UserControllers(ILogger<UserControllers> logger, UserManager<IdentityUser> userManager, IMapper mapper,
                                RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _configuration = configuration;
            //_errorResponse = errorResponse;
        }
        [HttpPost("RegisterUser")]
        
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] CreateUserDTO user)
        {
            try
            {
                SuccessRespone<CreateUserDTO> successRes = new SuccessRespone<CreateUserDTO>();
                ErrorRespone errorRes = new ErrorRespone();
                IdentityUser userExist = await _userManager.FindByNameAsync(user.UserName);
                if (userExist != null)
                {
                   // errorRes.Description.Add( "username already exist");
                    return BadRequest(errorRes);
                }
                 IdentityUser emailExist = await _userManager.FindByEmailAsync(user.Email);
                if (emailExist != null)
                {
                   // errorRes.Description.Add("email already use");
                    return BadRequest(errorRes);
                }
                IdentityUser newUser = _mapper.Map<IdentityUser>(user);
                //for (int i = 1; i <= 100; i++)
                //{
                //    IdentityResult users = await _userManager.CreateAsync(new IdentityUser { UserName = $"duyngu{i}", Email = $"nguduy{i}" }, user.PassWord);
                //}
                IdentityResult result = await _userManager.CreateAsync(newUser, user.PassWord);
                //return Ok(successRes);
                if (result.Succeeded)
                {
                    successRes.data = user;
                    if (await _roleManager.FindByNameAsync(RolesStorage.User) == null)
                        await _roleManager.CreateAsync(new IdentityRole(RolesStorage.User));
                    await _userManager.AddToRoleAsync(newUser, RolesStorage.User);
                    return Ok(successRes);
                }
                else
                {
                    //foreach (var error in result.Errors)
                    //{
                    //    //errorRes.Description.Add(Convert.ToString(error));
                    //}
                    return BadRequest(errorRes);
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        [HttpPut("User")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserData updateUserData)
        {
            try
            {
                SuccessRespone<UpdateUserData> successRes = new SuccessRespone<UpdateUserData>();
                ErrorRespone errorRes = new ErrorRespone();
                IdentityUser user = await _userManager.FindByNameAsync(updateUserData.UserName);
                if (user == null)
                {

                    //errorRes.Description.Add("wrong user name");
                    return BadRequest(errorRes) ;
                }
                  _mapper.Map(updateUserData.UpdateUserDTO, user);
                IdentityUser isEmailExist = await _userManager.FindByEmailAsync(updateUserData.UpdateUserDTO.Email);
                if (isEmailExist != null && isEmailExist!=user)
                {
                    //errorRes.Description.Add("this email already use for another account");
                    return BadRequest(errorRes);
                }
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    successRes.data = updateUserData;
                    return Ok(successRes);
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

        [HttpGet("User")]
        //[Authorize(Roles = "User")]
        [AllowAnonymous]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                SuccessRespone<UserDTO> successRes = new SuccessRespone<UserDTO>();
                ErrorRespone errorRes = new ErrorRespone();
                IdentityUser user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    //errorRes.Description.Add("user not exist");
                    return BadRequest(errorRes);
                } 
                UserDTO userData = _mapper.Map<UserDTO>(user);
                successRes.data = userData;
                return Ok(successRes);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        
        [HttpGet("AllUsers")]
        //_myAllowSpecificOrigins
        
        [Authorize(Roles = "User")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                SuccessRespone<IEnumerable<UserDTO>> successRes = new SuccessRespone<IEnumerable<UserDTO>>();
                //IList<IdentityUser> users = await _userManager.GetUsersInRoleAsync("User");
                IList<IdentityUser> users =   _userManager.Users.ToList();
                IEnumerable<UserDTO> result =  _mapper.Map<IEnumerable<UserDTO>>(users);
                successRes.data = result;
                return Ok(successRes);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("User")]
        //[Authorize(Roles = "Admin")]
        [AllowAnonymous]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUsersDTO deleteUsersDTO)
        {
            try
            {
                SuccessRespone<DeleteUsersDTO> successRes = new SuccessRespone<DeleteUsersDTO>();
                ErrorRespone errorRes = new ErrorRespone();
                List<string> deleteFail = new List<string>();
                foreach (string id in deleteUsersDTO.Ids)
                {
                    IdentityUser userExist = await _userManager.FindByIdAsync(id);
                    if (userExist == null)
                        deleteFail.Add(id);

                    else await _userManager.DeleteAsync(userExist);

                }
                if (deleteFail.Count == 0) 
                    return Ok(successRes);
                else
                {
                    //errorRes.Description.Add("can't delete user");
                    return BadRequest(errorRes);
                }
                
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                SuccessRespone<Tokens> successRes = new SuccessRespone<Tokens>();
                ErrorRespone errorRes = new ErrorRespone();
                errorRes.Description = new List<string>();
                IdentityUser userExist = await _userManager.FindByNameAsync(login.Username);
                if (userExist == null)
                {
                    errorRes.Description.Add("username not exist");
                    return BadRequest(errorRes);
                }
                bool truePass = await _userManager.CheckPasswordAsync(userExist,login.Password);
                if (truePass == false)
                {
                    errorRes.Description.Add("wrong password");
                    return BadRequest(errorRes);
                }
                TokenManager accessTokenGen = new TokenManager( _configuration,_roleManager,_userManager);
                string refreshToken = accessTokenGen.GenerateRefreshToken();
                string accessToken = await accessTokenGen.GenerateAccessToken(userExist);
                Tokens tokens = new Tokens();
                tokens.accessToken = accessToken;
                tokens.refreshToken = refreshToken;
                successRes.data = tokens;
                return Ok(successRes);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }

        }
        //[HttpPost("AddClaim")]
        //[AllowAnonymous]
        //public async Task<IActionResult> AddClaim(string usenam )//, [FromBody] Claim claim)
        //{
        //    try
        //    {
        //        var userExist = await _userManager.FindByNameAsync(usenam);
        //        if (userExist == null)
        //        {
        //            return BadRequest("wrong user name");
        //        }
        //        var addcalim = await _userManager.AddClaimAsync(userExist, new Claim("User", "User"));
        //        return Ok(new { Description = "Add Claim successful", result = 1 });
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.InnerException != null)
        //            return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
        //        return BadRequest(ex.Message);
        //    }
        //}

    }
}
