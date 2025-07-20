using TeacherPortal.Data.DTOs;
using TeacherPortal.Tests.Services;

namespace TeacherPortal.UnitTests.Services
{
    [TestClass]
    public class StudentServiceTests : BaseServiceTest
    {
        protected override void SeedTestData()
        {
            // Create teacher
            CreateBasicTeacher(
                id: "teacher-1",
                firstName: "Demo",
                lastName: "Jay",
                email: "teacher@test.com",
                userName: "testteacher"
            );

            // Create students
            CreateBasicStudent(1, "Samy", "Kumar", "samy@test.com", "teacher-1");
            CreateBasicStudent(2, "Raj", "Kumar", "raj@test.com", "teacher-1");

            // Save changes
            SaveChanges();
        }

       
        [TestMethod]
        public async Task CreateStudentAsync_ValidData_ReturnsStudentDto()
        {
            // Arrange
            var createDto = new CreateStudentDto
            {
                FirstName = "Ram",
                LastName = "Singh",
                Email = "ram@test.com"
            };

            // Act
            var result = await StudentService.CreateStudentAsync(createDto, "teacher-1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Ram", result.FirstName);
            Assert.AreEqual("Singh", result.LastName);
            Assert.AreEqual("ram@test.com", result.Email);
            Assert.AreEqual("teacher-1", result.TeacherId);
        }

        [TestMethod]
        public async Task CreateStudentAsync_DuplicateEmail_ThrowsArgumentException()
        {
            // Arrange
            var createDto = new CreateStudentDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "samy@test.com"
            };

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => StudentService.CreateStudentAsync(createDto, "teacher-1"));
        }

        [TestMethod]
        public async Task GetStudentsByTeacherAsync_ValidTeacherId_ReturnsPagedResult()
        {
            // Act
            var result = await StudentService.GetStudentsByTeacherAsync("teacher-1", 1, 10);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.TotalCount);
            Assert.AreEqual(2, result.Items.Count);
            Assert.AreEqual("Raj", result.Items[0].FirstName);
        }

        [TestMethod]
        public async Task GetStudentsByTeacherAsync_WithSearch_ReturnsFilteredResults()
        {
            // Act
            var result = await StudentService.GetStudentsByTeacherAsync("teacher-1", 1, 10, "Raj");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);
            Assert.AreEqual("Raj", result.Items[0].FirstName);
        }

        [TestMethod]
        public async Task GetStudentByIdAsync_ExistingId_ReturnsStudentDto()
        {
            // Act
            var result = await StudentService.GetStudentByIdAsync(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Samy", result.FirstName);
            Assert.AreEqual("Kumar", result.LastName);
        }


        [TestMethod]
        public async Task DeleteStudentAsync_ExistingStudent_ReturnsTrue()
        {
            // Act
            var result = await StudentService.DeleteStudentAsync(1, "teacher-1");

            // Assert
            Assert.IsTrue(result);

            // Verify student is deleted
            var deletedStudent = await StudentService.GetStudentByIdAsync(1);
            Assert.IsNull(deletedStudent);
        }

        [TestMethod]
        public async Task DeleteStudentAsync_WrongTeacher_ReturnsFalse()
        {
            // Act
            var result = await StudentService.DeleteStudentAsync(1, "wrong-teacher");

            // Assert
            Assert.IsFalse(result);
        }

   
    }

}
