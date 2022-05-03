using AutoMapper;
using IdentityConfigurationSample.Data;
using IdentityConfigurationSample.DTO;
using IdentityConfigurationSample.Res;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize (Roles = "Admin")]
    [AllowAnonymous]
    public class RoleControllers : ControllerBase
    {
        private readonly ILogger<UserControllers> _logger;
        private UserManager<IdentityUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public IMapper _mapper;
        public RoleControllers(ILogger<UserControllers> logger, UserManager<IdentityUser> userManager, IMapper mapper,
                               RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }
        [HttpPost("CreateRole")]
        public async Task<IActionResult> CreateRole([FromBody] string role)
        {
            try
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    return BadRequest(new { Description = "role already exist ", result = 0 }); ;
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                
                if (result.Succeeded)
                {

                    return Ok();
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

        [HttpPut("Role")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleData role)
        {
            try
            {
                IdentityRole roleExist = await _roleManager.FindByNameAsync(role.Name);
                if (roleExist == null)
                {
                    return BadRequest("wrong role name");
                }

                IdentityRole newRoleName = _mapper.Map(role.UpdateRoleDTO, roleExist);
                IdentityRole isRoleExist = await _roleManager.FindByNameAsync(role.UpdateRoleDTO.newRole);
                if (isRoleExist != null)
                {
                    return BadRequest(new { Description = "role already exist", result = 0  });
                }

                IdentityResult result = await _roleManager.UpdateAsync(roleExist);
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

        [HttpGet("Role")]
        public async Task<IActionResult> GetRole(string roleName)
        {
            try
            {
                IdentityRole roleExist = await _roleManager.FindByNameAsync(roleName);
                if (roleExist == null)
                {
                    return BadRequest(new { Description = "role dont exist", result = 0 });
                }
                var result = _mapper.Map<RoleDTO>(roleExist);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("AllRole")]
        public async Task<IActionResult> GetAllRole()
        {
            try
            {
                SuccessRespone<IEnumerable<RoleDTO>> successRes = new SuccessRespone<IEnumerable<RoleDTO>>();
                IList<IdentityRole> roles = _roleManager.Roles.ToList();
                IEnumerable<RoleDTO> result = _mapper.Map<IEnumerable<RoleDTO>>(roles);
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
        [HttpPut("AddRoleForUser")]
        public async Task<IActionResult> AddRoleForUser([FromBody] UserData UpdateUser)
        {
            try
            {
                IdentityUser user = await _userManager.FindByNameAsync(UpdateUser.UserName);
                if (user == null)
                {
                    return BadRequest(new { Description = "user dont exist", result = 0 });
                }
                IList<string> roles = await _userManager.GetRolesAsync(user);
                foreach (string role in roles)
                {
                   await _userManager.RemoveFromRoleAsync(user, role);
                }
                IdentityResult result = await _userManager.AddToRolesAsync(user, UpdateUser.UpdateUserRolesDTO.Roles);
                if (result.Succeeded)
                    return Ok(new { Description = "add role successful", result = 1 });
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("RemoveRoleForUser")]
        public async Task<IActionResult> RemoveRoleForUser([FromBody] UserData UpdateUser)
        {
            try
            {
                IdentityUser user = await _userManager.FindByNameAsync(UpdateUser.UserName);
                if (user == null)
                {
                    return BadRequest(new { Description = "user dont exilt", result = 0 }); ;
                }
                IdentityResult result = await _userManager.RemoveFromRolesAsync(user, UpdateUser.UpdateUserRolesDTO.Roles);
                if (result.Succeeded)
                    return Ok(new { Description = "remove role successful", result = 1 });
                return BadRequest(result.Errors);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return BadRequest($"Ex: {ex.Message}, Inner: {ex.InnerException.Message} ");
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("AddClaimForRole")]
        public async Task<IActionResult> AddClaimForRole([FromBody] UpdateRoleClaimData updateUserClaimData)
        {
            try
            {
                IdentityRole roleExilt = await _roleManager.FindByNameAsync(updateUserClaimData.RoleName);
                if (roleExilt == null)
                    return BadRequest(new { Description = "role not exist", result = 0 }); ;
                foreach (string claimUser in updateUserClaimData.UpdateUserClaimDTO.RoleClaims)
                {
                    Claim claim = new Claim(claimUser, claimUser);
                    IdentityResult addclaim = await _roleManager.AddClaimAsync(roleExilt, claim);
                }
                var result = await _roleManager.GetClaimsAsync(roleExilt);
                return Ok(result);
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
