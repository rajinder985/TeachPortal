using TeacherPortal.Data.DTOs;

namespace TeacherPortal.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TeacherDto> RegisterAsync(RegisterDto model);
        Task<AuthResponseDto> LoginAsync(LoginDto model);
    }
}
