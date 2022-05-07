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
using System;
using System.Collections.Generic;
using System.Linq;
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
                SuccessRespone<UserResDTO> successRespone = new SuccessRespone<UserResDTO>();
                ErrorRespone errorRes = new ErrorRespone();
                errorRes.Description = new List<string>();
                IdentityUser userExist = await _userManager.FindByNameAsync(user.UserName);
                if (userExist != null)
                {
                    errorRes.Description.Add( "username already exist");
                    return BadRequest(errorRes);
                }
                 IdentityUser emailExist = await _userManager.FindByEmailAsync(user.Email);
                if (emailExist != null)
                {
                    errorRes.Description.Add("email already use");
                    return BadRequest(errorRes);
                }
                IdentityUser newUser = _mapper.Map<IdentityUser>(user);
                //for (int i = 1; i <= 100; i++)
                //{
                //    IdentityResult users = await _userManager.CreateAsync(new IdentityUser { UserName = $"duyngu{i}", Email = $"nguduy{i}" }, user.PassWord);
                //}
                //return Ok("dmm");
                IdentityResult result = await _userManager.CreateAsync(newUser, user.PassWord);
                //return Ok(_successResponse);
                if (result.Succeeded)
                {
                    TokenManager accessTokenGen = new TokenManager(_configuration, _roleManager, _userManager);
                    string accessToken = await accessTokenGen.GenerateAccessToken(newUser);
                    IList<string> roles = await _userManager.GetRolesAsync(newUser);
                    UserResDTO userResDTO = new UserResDTO();
                    userResDTO.accsesstoken = accessToken;
                    userResDTO.id = newUser.Id;

                    userResDTO.roles = await _userManager.GetRolesAsync(newUser);
                    successRespone.data = userResDTO;
                    if (await _roleManager.FindByNameAsync(RolesStorage.User) == null)
                        await _roleManager.CreateAsync(new IdentityRole(RolesStorage.User));
                    await _userManager.AddToRoleAsync(newUser, RolesStorage.User);
                    return Ok(successRespone);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        errorRes.Description.Add(error.Description.ToString());
                    }
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
                SuccessRespone<UpdateUserData> successRespone = new SuccessRespone<UpdateUserData>();
                ErrorRespone errorRes = new ErrorRespone();
                IdentityUser user = await _userManager.FindByIdAsync(updateUserData.Id);
                if (user == null)
                { 
                    //errorRes.Description.Add("wrong user name");
                    return BadRequest(errorRes) ;
                }
                IdentityUser isEmailExist = await _userManager.FindByEmailAsync(updateUserData.UpdateUserDTO.Email);
                if (isEmailExist != null && isEmailExist!=user)
                {
                    //errorRes.Description.Add("this email already use for another account");
                    return BadRequest(errorRes);
                }
                _mapper.Map(updateUserData.UpdateUserDTO, user);
                
                
                IdentityResult result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        successRespone.data = updateUserData;
                        return Ok(successRespone);
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
                SuccessRespone<UserDTO> successRespone = new SuccessRespone<UserDTO>();
                ErrorRespone errorRes = new ErrorRespone();
                IdentityUser user = await _userManager.FindByIdAsync(id);
                if (user == null)
                {
                    //errorRes.Description.Add("user not exist");
                    return BadRequest(errorRes);
                } 
                UserDTO userData = _mapper.Map<UserDTO>(user);
                successRespone.data = userData;
                return Ok(successRespone);
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
        
        [Authorize(Roles = "Admin")]
        //[AllowAnonymous]
        public async Task<IActionResult> GetAllUser(string offset)
        {
            try
            {
                SuccessRespone<GetAllTotal> successRespone = new SuccessRespone<GetAllTotal>();
                //SuccessRespone<IEnumerable<UserDTO>> successRespone = new SuccessRespone<IEnumerable<UserDTO>>();
                //IList<IdentityUser> users = await _userManager.GetUsersInRoleAsync("User");
                List<IdentityUser> users =  _userManager.Users.ToList();
                int range = Convert.ToInt32(offset);
                List<IdentityUser> pageUser = new List<IdentityUser>();
                if (range + 10 > users.Count)
                    pageUser = users.GetRange(range, users.Count - range);

               else
                     pageUser =  users.GetRange(range,range+10);

                IEnumerable<UserDTO> result =  _mapper.Map<IEnumerable<UserDTO>>(users);
                GetAllTotal getAllTotal = new GetAllTotal();
                getAllTotal.total = users.Count;
                getAllTotal.users = result;
                successRespone.data = getAllTotal;
                //successRespone.data = result;
                return Ok(successRespone);
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
                SuccessRespone<IEnumerable<UserDTO>> successRespone = new SuccessRespone<IEnumerable<UserDTO>>();
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
                    return Ok(successRespone);
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
                SuccessRespone<Tokens> successRespone = new SuccessRespone<Tokens>();
                ErrorRespone errorRes = new ErrorRespone();
                errorRes.Description = new List<string>();
                IdentityUser userExist = await _userManager.FindByNameAsync(login.Username);
                if (userExist == null)
                {
                    errorRes.Description.Add("username not exist");
                    return BadRequest(errorRes);
                }
                //string a = userExist.PasswordHash;


                //bool verified = BCrypt.Net.BCrypt.Verify(login.Password, a);
                //if(verified==true)
                //{
                //    return Ok("work");
                //}
                //if(verified != true)
                //{
                //    return BadRequest("not work");
                //}
                bool truePass = await _userManager.CheckPasswordAsync(userExist,login.Password);
                if (truePass == false)
                {
                    errorRes.Description.Add("wrong password");
                    return BadRequest(errorRes);
                }
                TokenManager accessTokenGen = new TokenManager( _configuration,_roleManager,_userManager);
                string refreshToken = accessTokenGen.GenerateRefreshToken();
                string accessToken = await accessTokenGen.GenerateAccessToken(userExist);
                IList<string> roles = await _userManager.GetRolesAsync(userExist);
                Tokens tokens = new Tokens();
                //tokens.refreshToken = refreshToken;
                tokens.accessToken = accessToken;
                tokens.Roles = roles;
                tokens.id = userExist.Id;
                successRespone.data = tokens;
                return Ok(successRespone);
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
