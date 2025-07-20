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
    public class StudentsControllerTests
    {
        private Mock<IStudentService> _mockStudentService = null!;
        private StudentsController _controller = null!;
        private const string TestTeacherId = "teacher-123";

        [TestInitialize]
        public void Setup()
        {
            _mockStudentService = new Mock<IStudentService>();
            _controller = new StudentsController(_mockStudentService.Object);

            // Mock authenticated user with teacher ID
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, TestTeacherId)
            };

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "test"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [TestMethod]
        public async Task GetMyStudents_ValidRequest_ReturnsOkWithPagedResult()
        {
            // Arrange
            var pagedResult = new PagedResult<StudentDto>
            {
                Items =
                [
                    new StudentDto
                    {
                        Id = 1,
                        FirstName = "Rahul",
                        LastName = "Sharma",
                        Email = "rahul@test.com",
                        TeacherId = TestTeacherId
                    },
                    new StudentDto
                    {
                        Id = 2,
                        FirstName = "Priya",
                        LastName = "Patel",
                        Email = "priya@test.com",
                        TeacherId = TestTeacherId
                    }
                ],
                TotalCount = 2,
                PageNumber = 1,
                PageSize = 10,
            };

            _mockStudentService.Setup(x => x.GetStudentsByTeacherAsync(TestTeacherId, 1, 10, null))
                              .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetMyStudents(1, 10);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult?.Value, typeof(PagedResult<StudentDto>));

            var returnedResult = okResult?.Value as PagedResult<StudentDto>;
            Assert.IsNotNull(returnedResult);
            Assert.AreEqual(2, returnedResult.TotalCount);
            Assert.AreEqual(2, returnedResult.Items.Count);
        }

        [TestMethod]
        public async Task GetMyStudents_WithSearch_ReturnsFilteredResults()
        {
            // Arrange
            var pagedResult = new PagedResult<StudentDto>
            {
                Items = new List<StudentDto>
                {
                    new StudentDto
                    {
                        Id = 1,
                        FirstName = "Rahul",
                        LastName = "Sharma",
                        Email = "rahul@test.com",
                        TeacherId = TestTeacherId
                    }
                },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10,
            };

            _mockStudentService.Setup(x => x.GetStudentsByTeacherAsync(TestTeacherId, 1, 10, "Rahul"))
                              .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetMyStudents(1, 10, "Rahul");

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            var returnedResult = okResult?.Value as PagedResult<StudentDto>;
            Assert.IsNotNull(returnedResult);
            Assert.AreEqual(1, returnedResult.TotalCount);
            Assert.AreEqual("Rahul", returnedResult.Items[0].FirstName);
        }

        [TestMethod]
        public async Task GetStudent_ExistingId_ReturnsOkWithStudent()
        {
            // Arrange
            var studentDto = new StudentDto
            {
                Id = 1,
                FirstName = "Amit",
                LastName = "Verma",
                Email = "amit@test.com",
                TeacherId = TestTeacherId
            };

            _mockStudentService.Setup(x => x.GetStudentByIdAsync(1))
                              .ReturnsAsync(studentDto);

            // Act
            var result = await _controller.GetStudent(1);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult?.Value, typeof(StudentDto));

            var returnedStudent = okResult?.Value as StudentDto;
            Assert.IsNotNull(returnedStudent);
            Assert.AreEqual("Amit", returnedStudent.FirstName);
            Assert.AreEqual("Verma", returnedStudent.LastName);
        }

      
       
        [TestMethod]
        public async Task CreateStudent_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("FirstName", "FirstName is required");
            var createDto = new CreateStudentDto();

            // Act
            var result = await _controller.CreateStudent(createDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(BadRequestObjectResult));
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
        }

        [TestMethod]
        public async Task DeleteStudent_ExistingStudent_ReturnsNoContent()
        {
            // Arrange
            _mockStudentService.Setup(x => x.DeleteStudentAsync(1, TestTeacherId))
                              .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteStudent(1);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NoContentResult));
        }

        [TestMethod]
        public async Task DeleteStudent_NonExistingStudent_ReturnsNotFound()
        {
            // Arrange
            _mockStudentService.Setup(x => x.DeleteStudentAsync(999, TestTeacherId))
                              .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteStudent(999);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundResult));
        }

       
        [TestMethod]
        public async Task GetMyStudents_DefaultPagination_UsesCorrectDefaults()
        {
            // Arrange
            var pagedResult = new PagedResult<StudentDto>
            {
                Items = [],
                TotalCount = 0,
                PageNumber = 1,
                PageSize = 10,
            };

            _mockStudentService.Setup(x => x.GetStudentsByTeacherAsync(TestTeacherId, 1, 10, null))
                              .ReturnsAsync(pagedResult);

            // Act
            var result = await _controller.GetMyStudents(); // No parameters = defaults

            // Assert
            _mockStudentService.Verify(x => x.GetStudentsByTeacherAsync(TestTeacherId, 1, 10, null), Times.Once);
        }
    }
}
