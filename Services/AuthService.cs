using ConversorMoedas.Data;
using ConversorMoedas.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using ConversorMoedas.DTOs;

namespace ConversorMoedas.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;

    public AuthService(AppDbContext context, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _context = context;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<LoginResponse> RegisterAsync(RegisterRequest request)
    {
        try
        {
            var usuarioExistente = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (usuarioExistente != null)
            {
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Username já existe"
                };
            }

            var emailExistente = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            if (emailExistente != null)
            {
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Email já está registrado"
                };
            }

            if (request.Password != request.ConfirmarPassword)
            {
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Senhas não conferem"
                };
            }

            var usuario = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                Role = "User",
                DataCriacao = DateTime.Now,
                Ativo = true
            };

            _context.Users.Add(usuario);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Usuário '{request.Username}' registrado com sucesso");

            var token = GenerateJwtToken(usuario);

            return new LoginResponse
            {
                Sucesso = true,
                Mensagem = "Registro realizado com sucesso!",
                Token = token,
                User = new UserDto
                {
                    Id = usuario.Id,
                    Username = usuario.Username,
                    Email = usuario.Email,
                    Role = usuario.Role
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao registrar: {ex.Message}");
            return new LoginResponse
            {
                Sucesso = false,
                Mensagem = $"Erro ao registrar: {ex.Message}"
            };
        }
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == request.Username);

            if (usuario == null)
            {
                _logger.LogWarning($"Tentativa de login com usuário inexistente: {request.Username}");
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Username ou senha inválidos"
                };
            }

            if (!VerifyPassword(request.Password, usuario.PasswordHash))
            {
                _logger.LogWarning($"Tentativa de login com senha incorreta: {request.Username}");
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Username ou senha inválidos"
                };
            }

            if (!usuario.Ativo)
            {
                return new LoginResponse
                {
                    Sucesso = false,
                    Mensagem = "Usuário inativo"
                };
            }

            var token = GenerateJwtToken(usuario);

            _logger.LogInformation($"Login realizado com sucesso para: {request.Username}");

            return new LoginResponse
            {
                Sucesso = true,
                Mensagem = "Login realizado com sucesso!",
                Token = token,
                User = new UserDto
                {
                    Id = usuario.Id,
                    Username = usuario.Username,
                    Email = usuario.Email,
                    Role = usuario.Role
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Erro ao fazer login: {ex.Message}");
            return new LoginResponse
            {
                Sucesso = false,
                Mensagem = $"Erro ao fazer login: {ex.Message}"
            };
        }
    }

    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    private string GenerateJwtToken(User user)
    {
        var secretKey = _configuration["Jwt:SecretKey"] ?? "";
        var issuer = _configuration["Jwt:Issuer"] ?? "";
        var audience = _configuration["Jwt:Audience"] ?? "";
        var expirationMinutesStr = _configuration["Jwt:ExpirationMinutes"] ?? "60";

        if (!int.TryParse(expirationMinutesStr, out int expirationMinutes))
        {
            expirationMinutes = 60;
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Username),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
        new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, user.Role)
    };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: credentials
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
