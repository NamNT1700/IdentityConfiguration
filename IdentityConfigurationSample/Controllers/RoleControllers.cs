using AutoMapper;
using IdentityConfigurationSample.Data;
using IdentityConfigurationSample.DTO;
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
                    return BadRequest("role already exist ");
                var result = await _roleManager.CreateAsync(new IdentityRole(role));
                if (result.Succeeded)
                {
                    return Ok(new { Description = "successful", result = 1 });
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

        [HttpPut("UpdateRole")]
        public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleData role)
        {
            try
            {
                var roleExist = await _roleManager.FindByNameAsync(role.Name);
                if (roleExist == null)
                {
                    return BadRequest("wrong role name");
                }

                var newRoleName = _mapper.Map(role.UpdateRoleDTO, roleExist);
                var isRoleExist = await _roleManager.FindByNameAsync(role.UpdateRoleDTO.newName);
                if (isRoleExist != null)
                {
                    return BadRequest("role already exist");
                }
                
                var result = await _roleManager.UpdateAsync(roleExist);
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

        [HttpGet("GetRole")]
        public async Task<IActionResult> GetRole(string roleName)
        {
            try
            {
                var roleExist = await _roleManager.FindByNameAsync(roleName);
                if (roleExist == null)
                {
                    return BadRequest("wrong role name");
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
        [HttpPut("AddRoleForUser")]
        public async Task<IActionResult> AddRoleForUser([FromBody] UserNameDTO UpdateUser)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(UpdateUser.UseName);
                if (user == null)
                {
                    return BadRequest("user dont exist");
                }
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                   await _userManager.RemoveFromRoleAsync(user, role);
                }
                var result = await _userManager.AddToRolesAsync(user, UpdateUser.UpdateUserRolesDTO.Roles);
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
        public async Task<IActionResult> RemoveRoleForUser([FromBody]UserNameDTO UpdateUser)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(UpdateUser.UseName);
                if (user == null)
                {
                    return BadRequest("user dont exilt");
                }
                var result = await _userManager.RemoveFromRolesAsync(user, UpdateUser.UpdateUserRolesDTO.Roles);
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
                var roleExilt = await _roleManager.FindByNameAsync(updateUserClaimData.RoleName);
                if (roleExilt == null)
                    return BadRequest("role not exist");
                foreach (string claimUser in updateUserClaimData.UpdateUserClaimDTO.RoleClaims)
                {
                    Claim claim = new Claim(claimUser, claimUser);
                    var addclaim = await _roleManager.AddClaimAsync(roleExilt, claim);
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
