using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Task1.BL.Enums;
using Task1.BL.Helpers;
using Task1.Core.Dtos;
using Task1.Core.Models;
using Task1.Core.Services;

namespace Task1.BL.Services
{
    public class AuthService(UserManager<User> _userManager, IConfiguration _config) : IAuthService
    {
        public string GenerateToken(string userId, string username, string email, string role)
        {
            JwtOptions jwtOptions = new JwtOptions();
            jwtOptions.Audience = _config.GetSection("JwtOptions")["Audience"]!;
            jwtOptions.Issuer = _config.GetSection("JwtOptions")["Issuer"]!;
            jwtOptions.Secret = _config.GetSection("JwtOptions")["Secret"]!;

            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Secret));
            SigningCredentials cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            List<Claim> claims =
            [
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            ];

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: jwtOptions.Issuer,
                audience: jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(60),
                signingCredentials: cred
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            User? user;
            if (dto.UserNameOrEmail.Contains('@'))
                user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == dto.UserNameOrEmail);
            else
                user = await _userManager.Users.FirstOrDefaultAsync(x => x.UserName == dto.UserNameOrEmail); ;

            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                throw new NullReferenceException("Email/Username or Password is wrong");

            var roles = await _userManager.GetRolesAsync(user);
            var token = GenerateToken(user.Id, user.UserName!, user.Email!, roles[0]);
            return token;
        }

        public string LogOut()
        {
            return "Logged out successfully";
            ;
        }

        public async Task<bool> Register(RegisterDto dto)
        {
            if (dto == null)
                throw new NullReferenceException();
            if (await _userManager.FindByEmailAsync(dto.Email) != null)
                throw new ExistException("This Email already exist");
            if (await _userManager.FindByNameAsync(dto.Username) != null)
                throw new ExistException("This Username already exist");

            User user = new User()
            {
                Fullname = dto.Fullname,
                UserName = dto.Username,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                foreach (var item in result.Errors)
                {
                    throw new Exception($"{item.Description}");
                }
            }
            var result1 = await _userManager.AddToRoleAsync(user, nameof(Roles.Patience));
            if (!result1.Succeeded)
            {
                foreach (var item in result1.Errors)
                {
                    throw new Exception($"{item.Description}");
                }
            }
            return result.Succeeded;
        }
    }
}
