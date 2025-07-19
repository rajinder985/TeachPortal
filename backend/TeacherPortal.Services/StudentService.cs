using AutoMapper;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Data.Models;
using TeacherPortal.Data.Repositories.Interfaces;
using TeacherPortal.Services.Interfaces;

namespace TeacherPortal.Services
{
    public class StudentService : IStudentService
    {
        private readonly IDataManager _dataManager;
        private readonly IMapper _mapper;

        public StudentService(IDataManager dataManager, IMapper mapper)
        {
            _dataManager = dataManager;
            _mapper = mapper;
        }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto model, string teacherId)
        {
            // Check email uniqueness
            if (await _dataManager.Students.EmailExistsAsync(model.Email))
                throw new ArgumentException("Email already exists");

            var student = _mapper.Map<Student>(model);
            student.TeacherId = teacherId;
            student.CreatedAt = DateTime.UtcNow;

            await _dataManager.Students.AddAsync(student);
            await _dataManager.SaveChangesAsync();

            return _mapper.Map<StudentDto>(student);
        }

        public async Task<PagedResult<StudentDto>> GetStudentsByTeacherAsync(string teacherId, int pageNumber, int pageSize, string? search = null)
        {
            var result = await _dataManager.Students.GetStudentsByTeacherAsync(teacherId, pageNumber, pageSize, search);

            return new PagedResult<StudentDto>
            {
                Items = _mapper.Map<List<StudentDto>>(result.Items),
                TotalCount = result.TotalCount,
                PageNumber = result.PageNumber,
                PageSize = result.PageSize
            };
        }

        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var student = await _dataManager.Students.GetByIdAsync(id);
            return student != null ? _mapper.Map<StudentDto>(student) : null;
        }

        public async Task<bool> DeleteStudentAsync(int id, string teacherId)
        {
            var student = await _dataManager.Students.GetByIdAsync(id);
            if (student == null || student.TeacherId != teacherId)
                return false;

            _dataManager.Students.Delete(student);
            await _dataManager.SaveChangesAsync();
            return true;
        }
    }
}
