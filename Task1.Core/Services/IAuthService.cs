using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task1.Core.Dtos;

namespace Task1.Core.Services
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterDto dto);
        Task<string> LoginAsync(LoginDto dto);
        string GenerateToken(string userId, string username, string email, string role);
        string LogOut();
    }
}
