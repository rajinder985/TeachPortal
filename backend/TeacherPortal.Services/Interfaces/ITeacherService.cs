using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherPortal.Data.DTOs;

namespace TeacherPortal.Services.Interfaces
{
    public interface ITeacherService
    {
        Task<List<TeacherDto>> GetTeachersWithStudentCountAsync();
        Task<TeacherDto?> GetCurrentTeacherAsync(string teacherId);
    }
}
