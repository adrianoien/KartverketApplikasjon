
using KartverketApplikasjon.Data;
using KartverketApplikasjon.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace KartverketApplikasjon.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
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
    return BCrypt.Net.BCrypt.HashPassword(password);
}

    private bool VerifyPassword(string password, string hashedPassword)
{
    return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}
    }
}
