using MediatR;
using Microsoft.EntityFrameworkCore;
using Users.API.Application.DTOs;
using Users.API.Application.Interfaces;
using Users.API.Application.Mappings;

namespace Users.API.Application.Features.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IUserRepository _repository;
    private readonly IUsersDbContext _context;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IUserRepository repository, IUsersDbContext context, ILogger<LoginCommandHandler> logger)
    {
        _repository = repository;
        _context = context;
        _logger = logger;
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Buscar usuario por email
        var user = await _repository.GetByEmailAsync(request.Email, cancellationToken);

        if (user == null)
        {
            _logger.LogWarning("Login attempt with non-existent email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");
        }

        // Verificar contraseña usando BCrypt
        // Usar Password (el campo password_hash puede no existir en la base de datos)
        var passwordHash = user.Password;

        if (string.IsNullOrEmpty(passwordHash))
        {
            _logger.LogWarning("User {Email} has no password set", request.Email);
            throw new UnauthorizedAccessException("Este usuario no tiene contraseña configurada. Por favor, contacte al administrador.");
        }

        bool isPasswordValid = false;
        try
        {
            isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, passwordHash);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying password for user {Email}", request.Email);
            throw new UnauthorizedAccessException("Error al verificar la contraseña");
        }

        if (!isPasswordValid)
        {
            _logger.LogWarning("Invalid password attempt for email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");
        }

        // Si el usuario no tiene token, generar uno nuevo usando SQL directo
        if (string.IsNullOrEmpty(user.Token))
        {
            var newToken = Guid.NewGuid().ToString("N");
            // Usar el DbContext directamente desde el Set<T> para acceder a Database
            var dbContext = _context as Microsoft.EntityFrameworkCore.DbContext;
            if (dbContext != null)
            {
                await dbContext.Database.ExecuteSqlRawAsync(
                    "UPDATE \"user\" SET token = {0} WHERE email = {1}",
                    newToken, request.Email, cancellationToken);
                user.Token = newToken;
            }
        }

        var userDto = user.ToDto();

        return new LoginResponseDto
        {
            User = userDto,
            Token = user.Token,
            Message = "Login exitoso"
        };
    }
}
