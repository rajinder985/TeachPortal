using AutoMapper;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Data.Models;

namespace TeacherPortal.Services.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Teacher, TeacherDto>()
                .ForMember(dest => dest.StudentCount, opt => opt.MapFrom(src => src.Students.Count));

            CreateMap<Student, StudentDto>()
                .ForMember(dest => dest.TeacherName, opt => opt.MapFrom(src => src.Teacher.FullName));

            CreateMap<CreateStudentDto, Student>();
        }
    }
}
