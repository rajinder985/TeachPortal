using TeacherPortal.Data.Models;

namespace TeacherPortal.Data.Repositories.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
        Task<List<Teacher>> GetTeachersWithStudentCountAsync();
        Task UpdateLastLoginAsync(string teacherId);
    }
}
