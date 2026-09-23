namespace ConversorMoedas.Models;

public class RegisterRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string ConfirmarPassword { get; set; } = string.Empty;
}
