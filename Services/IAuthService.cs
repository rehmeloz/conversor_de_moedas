using ConversorMoedas.Models;

namespace ConversorMoedas.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(RegisterRequest request);
    Task<User> GetUserByUsernameAsync(string username);
}
