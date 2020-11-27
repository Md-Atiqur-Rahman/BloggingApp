using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using coreIdentity.Models;
using coreIdentity.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace coreIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagerController : ControllerBase
    {
        private readonly UserManager<User> _usermanager;
        private readonly RoleManager<Role> _rolemanager;
        private readonly JwtSettings _jwtSettings;
        public UserManagerController(UserManager<User> m, RoleManager<Role> r, IOptionsSnapshot<JwtSettings> jwtSettings)
        {
            this._usermanager = m;
            this._rolemanager = r;
            _jwtSettings = jwtSettings.Value;

        }
        [HttpPost]
        public async Task<IActionResult> SignUp(UserVM vm){

            var user = new User
            {
                UserName = vm.UserName,
                Email = vm.Email

            };
            var result = await _usermanager.CreateAsync(user,vm.Password);
            if(result.Succeeded)
            {
                return Created("", vm);
            }
            else if(result.Errors.Count()>0)
            {
                foreach (var e in result.Errors)
                {
                    ModelState.AddModelError("", e.Description);
                        }
            }
            return Problem();
        }
        [HttpPost("Roles")]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
            {
                return BadRequest("Role name should be provided.");
            }

            var newRole = new Role
            {
                Name = roleName
            };

            var roleResult = await _rolemanager.CreateAsync(newRole);

            if (roleResult.Succeeded)
            {
                return Ok();
            }

            return Problem(roleResult.Errors.First().Description, null, 500);
        }
        [HttpPost("signin")]
        public async Task<IActionResult> Login(UserVM userVM)
        {
            var user = _usermanager.Users.Where(u => u.Email == userVM.Email).FirstOrDefault();
            if(user== null)
            {
                //return NoContent(); 
                return Problem("Invalid username ");
            }
             var result=  await _usermanager.CheckPasswordAsync(user, userVM.Password);
            if(result)
            {
                var rolelist = await  _usermanager.GetRolesAsync(user);
               
                return Ok(GenerateJwt(user, rolelist));
            }
            return Problem("Invalid username or password" );


        }



        [HttpPost("AddRole")]
        public async Task<IActionResult> AddUserToRole(string userEmail,  string roleName)
        {
            var user = _usermanager.Users.SingleOrDefault(u => u.Email == userEmail);

            var result = await _usermanager.AddToRoleAsync(user, roleName);

            if (result.Succeeded)
            {
                return Ok();
            }

            return Problem(result.Errors.First().Description, null, 500);
        }


        private string GenerateJwt(User user, IList<string> roles)
        {
            var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                    };

            var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r));
            claims.AddRange(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_jwtSettings.ExpirationInDays));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Issuer,
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
