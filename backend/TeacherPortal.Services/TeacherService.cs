using AutoMapper;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Data.Repositories.Interfaces;
using TeacherPortal.Services.Interfaces;

namespace TeacherPortal.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IDataManager _dataManager;
        private readonly IMapper _mapper;

        public TeacherService(IDataManager dataManager, IMapper mapper)
        {
            _dataManager = dataManager;
            _mapper = mapper;
        }

        public async Task<List<TeacherDto>> GetTeachersWithStudentCountAsync()
        {
            var teachers = await _dataManager.Teachers.GetTeachersWithStudentCountAsync();
            return _mapper.Map<List<TeacherDto>>(teachers);
        }

        public async Task<TeacherDto?> GetCurrentTeacherAsync(string teacherId)
        {
            var teacher = await _dataManager.Teachers.GetByIdAsync(teacherId);
            return teacher != null ? _mapper.Map<TeacherDto>(teacher) : null;
        }
    }
}
