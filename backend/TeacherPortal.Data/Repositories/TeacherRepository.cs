using Microsoft.EntityFrameworkCore;
using TeacherPortal.Data.Models;
using TeacherPortal.Data.Repositories.Interfaces;

namespace TeacherPortal.Data.Repositories
{
    public class TeacherRepository : Repository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<Teacher>> GetTeachersWithStudentCountAsync()
        {
            return await _dbSet.Include(t => t.Students)
                              .OrderBy(t => t.FirstName)
                              .ToListAsync();
        }

        public async Task UpdateLastLoginAsync(string teacherId)
        {
            var teacher = await _dbSet.FindAsync(teacherId);
            if (teacher != null)
            {
                teacher.LastLoginAt = DateTime.UtcNow;
                _context.Entry(teacher).State = EntityState.Modified;
            }
        }
    }
}
