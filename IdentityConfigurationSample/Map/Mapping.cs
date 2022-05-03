using AutoMapper;
using IdentityConfigurationSample.DTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Map
{
    public class Mapping: Profile
    {
        public Mapping()
        {
            CreateMap<CreateUserDTO, IdentityUser>();
            CreateMap<UpdateUserDTO, IdentityUser>();
            CreateMap<IdentityUser, UserDTO>();
            CreateMap<IdentityRole, RoleDTO>();
            CreateMap<UpdateRoleDTO, IdentityRole>().ForMember(name=> name.Name,newRole=>newRole.MapFrom(newName=>newName.newRole));

        }
    }
}
