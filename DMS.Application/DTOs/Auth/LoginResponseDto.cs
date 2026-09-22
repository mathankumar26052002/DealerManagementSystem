using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public int? DealerId { get; set; }

        public DateTime ExpiresAt { get; set; }
    }
}
