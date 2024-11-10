using KartverketApplikasjon.Models;

namespace KartverketApplikasjon.Services
{
    // Interface defining user-related services
    public interface IUserService
    {
        // Registers a new user asynchronously, taking registration details from a RegisterViewModel
        Task<UserData> RegisterUserAsync(RegisterViewModel model);

        // Authenticates a user asynchronously using their email and password
        Task<UserData> AuthenticateAsync(string email, string password);
    }
}