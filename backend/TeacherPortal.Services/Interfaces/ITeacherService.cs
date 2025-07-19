using TeacherPortal.Data.DTOs;

namespace TeacherPortal.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<List<TeacherDto>> GetTeachersWithStudentCountAsync();
        Task<TeacherDto?> GetCurrentTeacherAsync(string teacherId);
    }
}
