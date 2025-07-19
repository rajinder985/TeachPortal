using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Data.Models;
using TeacherPortal.Data.Repositories.Interfaces;

namespace TeacherPortal.Data.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PagedResult<Student>> GetStudentsByTeacherAsync(string teacherId, int pageNumber, int pageSize, string? search = null)
        {
            IQueryable<Student> query = _dbSet.Where(s => s.TeacherId == teacherId)
                                             .Include(s => s.Teacher);

            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(s => s.FirstName.ToLower().Contains(search) ||
                                   s.LastName.ToLower().Contains(search) ||
                                   s.Email.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query.OrderBy(s => s.FirstName)
                                  .Skip((pageNumber - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToListAsync();

            return new PagedResult<Student>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
        {
            var query = _dbSet.Where(s => s.Email.ToLower() == email.ToLower());

            if (excludeId.HasValue)
                query = query.Where(s => s.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
