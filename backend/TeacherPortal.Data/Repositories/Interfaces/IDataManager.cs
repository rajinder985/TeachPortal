using System;
using System.Threading.Tasks;

namespace TeacherPortal.Data.Repositories.Interfaces
{
    public interface IDataManager : IDisposable
    {
        IStudentRepository Students { get; }
        ITeacherRepository Teachers { get; }
        Task<int> SaveChangesAsync();
    }
}
