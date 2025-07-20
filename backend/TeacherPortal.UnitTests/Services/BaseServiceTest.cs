using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TeacherPortal.Data;
using TeacherPortal.Data.Repositories;
using TeacherPortal.Data.Repositories.Interfaces;
using TeacherPortal.Services;
using TeacherPortal.Services.Interfaces;
using TeacherPortal.Services.Mapping;

namespace TeacherPortal.Tests.Services
{
    public abstract class BaseServiceTest
    {
        protected ServiceProvider ServiceProvider = null!;
        protected ApplicationDbContext Context = null!;
        protected IStudentService StudentService = null!;
        protected ITeacherService TeacherService = null!;
        protected IDataManager DataManager = null!;

        [TestInitialize]
        public virtual void Setup()
        {
            // Create service collection
            var services = new ServiceCollection();

            // Setup in-memory database with unique name per test
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()));

            // Add AutoMapper
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            // Add logging
            services.AddLogging();

            // Add repositories
            services.AddScoped<IDataManager, DataManager>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();

            // Add services
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();

            // Build service provider
            ServiceProvider = services.BuildServiceProvider();

            // Get services
            Context = ServiceProvider.GetRequiredService<ApplicationDbContext>();
            StudentService = ServiceProvider.GetRequiredService<IStudentService>();
            TeacherService = ServiceProvider.GetRequiredService<ITeacherService>();
            DataManager = ServiceProvider.GetRequiredService<IDataManager>();

            // Call child class seed method
            SeedTestData();
        }

        [TestCleanup]
        public virtual void Cleanup()
        {
            ServiceProvider?.Dispose();
        }

  
        protected abstract void SeedTestData();

        /// <summary>
        /// Helper method to create a basic teacher for testing
        /// </summary>
        protected void CreateBasicTeacher(string id, string firstName, string lastName, string email, string userName)
        {
            var teacher = new TeacherPortal.Data.Models.Teacher
            {
                Id = id,
                UserName = userName,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedAt = DateTime.UtcNow
            };

            Context.Users.Add(teacher);
        }

        /// <summary>
        /// Helper method to create a basic student for testing
        /// </summary>
        protected void CreateBasicStudent(int id, string firstName, string lastName, string email, string teacherId)
        {
            var student = new TeacherPortal.Data.Models.Student
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                TeacherId = teacherId,
                CreatedAt = DateTime.UtcNow
            };

            Context.Students.Add(student);
        }

        /// <summary>
        /// Save all changes to the test database
        /// </summary>
        protected void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
