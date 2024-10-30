using KartverketApplikasjon.Models;

namespace KartverketApplikasjon.Services
{
    public interface IUserService
    {
        Task<UserData> RegisterUserAsync(RegisterViewModel model);
        Task<UserData> AuthenticateAsync(string email, string password);
    }
}