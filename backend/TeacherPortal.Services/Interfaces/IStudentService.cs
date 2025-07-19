using TeacherPortal.Data.DTOs;

namespace TeacherPortal.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDto> CreateStudentAsync(CreateStudentDto model, string teacherId);
        Task<PagedResult<StudentDto>> GetStudentsByTeacherAsync(string teacherId, int pageNumber, int pageSize, string? search = null);
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<bool> DeleteStudentAsync(int id, string teacherId);
    }
}
