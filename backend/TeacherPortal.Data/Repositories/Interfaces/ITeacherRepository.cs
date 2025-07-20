using TeacherPortal.Data.Models;

namespace TeacherPortal.Data.Repositories.Interfaces
{
    public interface ITeacherRepository : IRepository<Teacher>
    {
        Task<List<Teacher>> GetTeachersWithStudentCountAsync();
        Task<Teacher?> GetTeacherWithStudentsAsync(string teacherId);
        Task UpdateLastLoginAsync(string teacherId);
    }
}
