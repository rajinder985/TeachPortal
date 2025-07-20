using Microsoft.AspNetCore.Mvc;
using Moq;
using TeacherPortal.API.Controllers;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Services.Interfaces;

namespace TeacherPortal.Tests.Controllers
{
    [TestClass]
    public class AuthControllerTests
    {
        private Mock<IAuthService> _mockAuthService = null!;
        private AuthController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [TestMethod]
        public async Task Register_ValidModel_ReturnsOkResult()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            var teacherResponse = new TeacherDto
            {
                Id = "1",
                UserName = "testuser",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
                           .ReturnsAsync(teacherResponse);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            Assert.IsInstanceOfType<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType<TeacherDto>(okResult?.Value);

            var returnedTeacher = okResult?.Value as TeacherDto;
            Assert.IsNotNull(returnedTeacher);
            Assert.AreEqual("testuser", returnedTeacher.UserName);
            Assert.AreEqual("test@test.com", returnedTeacher.Email);
        }

        [TestMethod]
        public async Task Register_ServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User",
                Password = "Password123!",
                ConfirmPassword = "Password123!"
            };

            _mockAuthService.Setup(x => x.RegisterAsync(It.IsAny<RegisterDto>()))
                          .ThrowsAsync(new ArgumentException("Email already exists"));

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => _controller.Register(registerDto));
        }

        [TestMethod]
        public async Task Login_ValidCredentials_ReturnsOkResult()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "testuser",
                Password = "Password123!"
            };

            var authResponse = new AuthResponseDto
            {
                Token = "fake-token",
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                Teacher = new TeacherDto { Id = "1", UserName = "testuser" }
            };

            _mockAuthService.Setup(x => x.LoginAsync(It.IsAny<LoginDto>()))
                          .ReturnsAsync(authResponse);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsInstanceOfType(result.Result, typeof(OkObjectResult));
            var okResult = result.Result as OkObjectResult;
            Assert.IsInstanceOfType(okResult!.Value, typeof(AuthResponseDto));
        }
    }
}
