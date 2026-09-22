using DMS.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace DMS.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(
            LoginRequestDto request);
    }
}
