using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task1.Core.Dtos
{
    public class RegisterDto
    {
        [MaxLength(32)]
        public string Fullname { get; set; } = null!;
        [MaxLength(32)]
        public string Username { get; set; } = null!;
        [MaxLength(64), EmailAddress]
        public string Email { get; set; } = null!;
        [MaxLength(20), Phone]
        public string PhoneNumber { get; set; } = null!;
        [MaxLength(32), DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        [MaxLength(32), DataType(DataType.Password), Compare("Password")]
        public string RePassword { get; set; } = null!;
    }
}
