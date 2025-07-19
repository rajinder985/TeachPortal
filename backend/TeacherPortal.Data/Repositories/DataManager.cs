using System;
using System.Threading.Tasks;
using TeacherPortal.Data.Repositories.Interfaces;

namespace TeacherPortal.Data.Repositories
{
    public class DataManager : IDataManager
    {
        private readonly ApplicationDbContext _context;

        public DataManager(ApplicationDbContext context)
        {
            _context = context;
            Students = new StudentRepository(_context);
            Teachers = new TeacherRepository(_context);
        }

        public IStudentRepository Students { get; private set; }
        public ITeacherRepository Teachers { get; private set; }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
