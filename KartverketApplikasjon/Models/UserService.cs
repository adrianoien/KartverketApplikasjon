using KartverketApplikasjon.Data;
using KartverketApplikasjon.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace KartverketApplikasjon.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }
        // Takes input from a RegisterViewModel, creates a new UserData object, and saving it to the database
        public async Task<UserData> RegisterUserAsync(RegisterViewModel model)
        {
            var user = new UserData
            {
                Name = model.Username,
                Email = model.Email,
                Password = HashPassword(model.Password),
                Role = model.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        // Authenticates use based on email and password
        public async Task<UserData> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user != null && VerifyPassword(password, user.Password))
            {
                return user;
            }
            return null;
        }

        // Password encryption 
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        // Compares hashed version of a password with a plaintext string
        private bool VerifyPassword(string password, string hashedPassword)
        {
            return HashPassword(password) == hashedPassword;
        }
    }
}