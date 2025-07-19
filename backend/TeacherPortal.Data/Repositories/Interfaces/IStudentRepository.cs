using TeacherPortal.Data.DTOs;
using TeacherPortal.Data.Models;

namespace TeacherPortal.Data.Repositories.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<PagedResult<Student>> GetStudentsByTeacherAsync(string teacherId, int pageNumber, int pageSize, string? search = null);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}
