using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Services.Interfaces;

namespace TeacherPortal.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeachersController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeacherDto>>> GetTeachers()
        {
            var teachers = await _teacherService.GetTeachersWithStudentCountAsync();
            return Ok(teachers);
        }

        [HttpGet("me")]
        public async Task<ActionResult<TeacherDto>> GetCurrentTeacher()
        {
            var teacherId = GetCurrentUserId();
            var teacher = await _teacherService.GetCurrentTeacherAsync(teacherId);

            if (teacher == null)
                return NotFound();

            return Ok(teacher);
        }

        private string GetCurrentUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User not found");
        }
    }
}
