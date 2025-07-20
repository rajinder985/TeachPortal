using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using TeacherPortal.API.Controllers;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Services.Interfaces;

namespace TeacherPortal.Tests.Controllers
{
    [TestClass]
    public class TeachersControllerTests
    {
        private Mock<ITeacherService> _mockTeacherService = null!;
        private TeachersController _controller = null!;
        private const string TestTeacherId = "teacher-123";

        [TestInitialize]
        public void Setup()
        {
            _mockTeacherService = new Mock<ITeacherService>();
            _controller = new TeachersController(_mockTeacherService.Object);

            // Mock authenticated user with teacher ID
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, TestTeacherId)
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task GetTeachers_ValidRequest_ReturnsOkWithAllTeachers()
        {
            // Arrange
            var teachersList = new List<TeacherDto>
            {
                new TeacherDto
                {
                    Id = "teacher-1",
                    FirstName = "Pardeep",
                    LastName = "Singh",
                    Email = "pardeep.singh@school.com",
                    UserName = "pardeep.singh",
                    StudentCount = 3
                },
                new TeacherDto
                {
                    Id = "teacher-2",
                    FirstName = "Deepak",
                    LastName = "Kumar",
                    Email = "deepak.kumar@school.com",
                    UserName = "deepak.kumar",
                    StudentCount = 2
                },
                new TeacherDto
                {
                    Id = "teacher-3",
                    FirstName = "Sachin",
                    LastName = "Jindal",
                    Email = "sachin.jindal@school.com",
                    UserName = "sachin.jindal",
                    StudentCount = 1
                },
                new TeacherDto
                {
                    Id = "teacher-4",
                    FirstName = "Megha",
                    LastName = "Kaur",
                    Email = "megha.kaur@school.com",
                    UserName = "megha.kaur",
                    StudentCount = 0
                }
            };

            _mockTeacherService.Setup(x => x.GetTeachersWithStudentCountAsync())
                              .ReturnsAsync(teachersList);

            // Act
            var result = await _controller.GetTeachers();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult?.Value, typeof(List<TeacherDto>));

            var returnedTeachers = okResult?.Value as List<TeacherDto>;
            Assert.IsNotNull(returnedTeachers);
            Assert.AreEqual(4, returnedTeachers.Count);

            // Verify specific teacher data
            var pardeep = returnedTeachers.Find(t => t.FirstName == "Pardeep");
            Assert.IsNotNull(pardeep);
            Assert.AreEqual(3, pardeep.StudentCount);
            Assert.AreEqual("Singh", pardeep.LastName);
        }

        [TestMethod]
        public async Task GetTeachers_EmptyList_ReturnsOkWithEmptyList()
        {
            // Arrange
            var emptyTeachersList = new List<TeacherDto>();

            _mockTeacherService.Setup(x => x.GetTeachersWithStudentCountAsync())
                              .ReturnsAsync(emptyTeachersList);

            // Act
            var result = await _controller.GetTeachers();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var returnedTeachers = okResult?.Value as List<TeacherDto>;
            Assert.IsNotNull(returnedTeachers);
            Assert.AreEqual(0, returnedTeachers.Count);
        }

        [TestMethod]
        public async Task GetCurrentTeacher_ValidAuthenticatedUser_ReturnsOkWithTeacherData()
        {
            // Arrange
            var currentTeacher = new TeacherDto
            {
                Id = TestTeacherId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@school.com",
                UserName = "john.doe",
                StudentCount = 5
            };

            _mockTeacherService.Setup(x => x.GetCurrentTeacherAsync(TestTeacherId))
                              .ReturnsAsync(currentTeacher);

            // Act
            var result = await _controller.GetCurrentTeacher();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult?.Value, typeof(TeacherDto));

            var returnedTeacher = okResult?.Value as TeacherDto;
            Assert.IsNotNull(returnedTeacher);
            Assert.AreEqual(TestTeacherId, returnedTeacher.Id);
            Assert.AreEqual("John", returnedTeacher.FirstName);
            Assert.AreEqual("Doe", returnedTeacher.LastName);
            Assert.AreEqual("john.doe@school.com", returnedTeacher.Email);
            Assert.AreEqual(5, returnedTeacher.StudentCount);
        }

        [TestMethod]
        public async Task GetCurrentTeacher_TeacherNotFound_ReturnsNotFound()
        {
            // Arrange
            _mockTeacherService.Setup(x => x.GetCurrentTeacherAsync(TestTeacherId))
                              .ReturnsAsync((TeacherDto?)null);

            // Act
            var result = await _controller.GetCurrentTeacher();

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task GetCurrentTeacher_ValidatesServiceCall_CallsWithCorrectTeacherId()
        {
            // Arrange
            var currentTeacher = new TeacherDto
            {
                Id = TestTeacherId,
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@school.com",
                UserName = "jane.smith",
                StudentCount = 0
            };

            _mockTeacherService.Setup(x => x.GetCurrentTeacherAsync(TestTeacherId))
                              .ReturnsAsync(currentTeacher);

            // Act
            var result = await _controller.GetCurrentTeacher();

            // Assert
            _mockTeacherService.Verify(x => x.GetCurrentTeacherAsync(TestTeacherId), Times.Once);

            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var returnedTeacher = okResult?.Value as TeacherDto;
            Assert.IsNotNull(returnedTeacher);
            Assert.AreEqual("Jane", returnedTeacher.FirstName);
        }
    }
}
