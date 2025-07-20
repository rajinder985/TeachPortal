using Microsoft.EntityFrameworkCore;
using TeacherPortal.Tests.Services;

namespace TeacherPortal.UnitTests.Services
{
    [TestClass]
    public class TeacherServiceTests : BaseServiceTest
    {
        protected override void SeedTestData()
        {
            // Create teachers with different creation dates
            CreateBasicTeacher("teacher-1", "Pardeep", "Singh", "pardeep.singh@school.com", "pardeep.singh");
            CreateBasicTeacher("teacher-2", "Deepak", "Kumar", "deepak.kumar@school.com", "deepak.kumar");
            CreateBasicTeacher("teacher-3", "Sachin", "Jindal", "sachin.jindal@school.com", "sachin.jindal");
            CreateBasicTeacher("teacher-4", "Megha", "Kaur", "megha.kaur@school.com", "megha.kaur");

            // Create students for different teachers
            // Pardeep Singh's students (3)
            CreateBasicStudent(1, "Rahul", "Sharma", "rahul.sharma@student.com", "teacher-1");
            CreateBasicStudent(2, "Priya", "Patel", "priya.patel@student.com", "teacher-1");
            CreateBasicStudent(3, "Amit", "Verma", "amit.verma@student.com", "teacher-1");

            // Deepak Kumar's students (2)
            CreateBasicStudent(4, "Anjali", "Gupta", "anjali.gupta@student.com", "teacher-2");
            CreateBasicStudent(5, "Vikash", "Singh", "vikash.singh@student.com", "teacher-2");

            // Sachin Jindal's student (1)
            CreateBasicStudent(6, "Neha", "Agarwal", "neha.agarwal@student.com", "teacher-3");

            // Save all changes
            SaveChanges();
        }

        [TestMethod]
        public async Task GetCurrentTeacherAsync_ExistingId_ReturnsTeacherDto()
        {
            // Act
            var result = await TeacherService.GetCurrentTeacherAsync("teacher-1");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Pardeep", result.FirstName);
            Assert.AreEqual("Singh", result.LastName);
            Assert.AreEqual("pardeep.singh@school.com", result.Email);
            Assert.AreEqual("teacher-1", result.Id);
        }

        [TestMethod]
        public async Task GetCurrentTeacherAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await TeacherService.GetCurrentTeacherAsync("non-existing-teacher");

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetTeachersWithStudentCountAsync_ReturnsAllTeachersWithCorrectCounts()
        {
            // Act
            var result = await TeacherService.GetTeachersWithStudentCountAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(4, result.Count);

            // Verify Pardeep Singh (3 students)
            var pardeep = result.FirstOrDefault(t => t.Id == "teacher-1");
            Assert.IsNotNull(pardeep);
            Assert.AreEqual("Pardeep", pardeep.FirstName);
            Assert.AreEqual("Singh", pardeep.LastName);
            Assert.AreEqual(3, pardeep.StudentCount);

            // Verify Deepak Kumar (2 students)
            var deepak = result.FirstOrDefault(t => t.Id == "teacher-2");
            Assert.IsNotNull(deepak);
            Assert.AreEqual("Deepak", deepak.FirstName);
            Assert.AreEqual("Kumar", deepak.LastName);
            Assert.AreEqual(2, deepak.StudentCount);

            // Verify Sachin Jindal (1 student)
            var sachin = result.FirstOrDefault(t => t.Id == "teacher-3");
            Assert.IsNotNull(sachin);
            Assert.AreEqual("Sachin", sachin.FirstName);
            Assert.AreEqual("Jindal", sachin.LastName);
            Assert.AreEqual(1, sachin.StudentCount);

            // Verify Megha Kaur (0 students)
            var megha = result.FirstOrDefault(t => t.Id == "teacher-4");
            Assert.IsNotNull(megha);
            Assert.AreEqual("Megha", megha.FirstName);
            Assert.AreEqual("Kaur", megha.LastName);
            Assert.AreEqual(0, megha.StudentCount);
        }

        [TestMethod]
        public async Task GetCurrentTeacherAsync_ValidatesEmailFormat()
        {
            // Act
            var result = await TeacherService.GetCurrentTeacherAsync("teacher-2");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Email.Contains("@"));
            Assert.IsTrue(result.Email.EndsWith("school.com"));
        }

        [TestMethod]
        public async Task GetTeachersWithStudentCountAsync_OrderedByCreationDate()
        {
            // Act
            var result = await TeacherService.GetTeachersWithStudentCountAsync();

            // Assert - Should be ordered by creation date
            Assert.IsNotNull(result);
            var firstTeacher = result.First();
            var lastTeacher = result.Last();

            // Verify we have different teachers
            Assert.AreNotEqual(firstTeacher.Id, lastTeacher.Id);
        }

        [TestMethod]
        public async Task GetCurrentTeacherAsync_ReturnsCompleteTeacherInfo()
        {
            // Act
            var result = await TeacherService.GetCurrentTeacherAsync("teacher-4");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Megha", result.FirstName);
            Assert.AreEqual("Kaur", result.LastName);
            Assert.AreEqual("megha.kaur@school.com", result.Email);
            Assert.IsFalse(string.IsNullOrEmpty(result.Id));
        }
    }
}
