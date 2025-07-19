using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using TeacherPortal.Data.DTOs;
using TeacherPortal.Data.Models;
using TeacherPortal.Data.Repositories.Interfaces;
using TeacherPortal.Services.Interfaces;

namespace TeacherPortal.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<Teacher> _userManager;
        private readonly SignInManager<Teacher> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IDataManager _dataManager;

        public AuthService(UserManager<Teacher> userManager, SignInManager<Teacher> signInManager,
                          IConfiguration configuration, IMapper mapper, IDataManager dataManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _mapper = mapper;
            _dataManager = dataManager;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterDto model)
        {
            // Checking if user exists
            if (await _userManager.FindByNameAsync(model.UserName) != null)
                throw new ArgumentException("Username already exists");

            if (await _userManager.FindByEmailAsync(model.Email) != null)
                throw new ArgumentException("Email already exists");

            var teacher = new Teacher
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(teacher, model.Password);
            if (!result.Succeeded)
                throw new ArgumentException(string.Join(", ", result.Errors.Select(e => e.Description)));

            var token = GenerateJwtToken(teacher);

            // Update last login
            await _dataManager.Teachers.UpdateLastLoginAsync(teacher.Id);
            await _dataManager.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Teacher = _mapper.Map<TeacherDto>(teacher)
            };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto model)
        {
            var teacher = await _userManager.FindByNameAsync(model.UserName);
            if (teacher == null)
                throw new UnauthorizedAccessException("Invalid credentials");

            var result = await _signInManager.CheckPasswordSignInAsync(teacher, model.Password, false);
            if (!result.Succeeded)
                throw new UnauthorizedAccessException("Invalid credentials");

            var token = GenerateJwtToken(teacher);

            // Update last login
            await _dataManager.Teachers.UpdateLastLoginAsync(teacher.Id);
            await _dataManager.SaveChangesAsync();

            return new AuthResponseDto
            {

                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                Teacher = _mapper.Map<TeacherDto>(teacher)
            };
        }

        private string GenerateJwtToken(Teacher teacher)
        {
            var jwtKey = _configuration["JwtSettings:Key"];
            if (string.IsNullOrEmpty(jwtKey))
                throw new InvalidOperationException("JWT key not configured properly");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, teacher.Id),
                new Claim(ClaimTypes.Name, teacher.UserName ?? ""),
                new Claim(ClaimTypes.Email, teacher.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
