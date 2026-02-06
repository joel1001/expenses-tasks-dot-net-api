using MediatR;
using Users.API.Application.DTOs;
using Users.API.Application.Interfaces;
using Users.API.Application.Mappings;
using Users.API.Models;

namespace Users.API.Application.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, UserDto>
{
    private readonly IUserRepository _repository;

    public CreateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserDto> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        // Verificar si el email ya existe usando GetByEmailAsync que usa SQL directo
        var existingUser = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser != null)
        {
            throw new InvalidOperationException($"User with email {request.Email} already exists.");
        }

        // Hash de la contraseña usando BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        // Generar token único para el usuario (GUID)
        var token = Guid.NewGuid().ToString("N"); // Generar token único

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Password = passwordHash, // Guardar el hash en el campo password (según esquema de DB)
            PasswordHash = passwordHash, // También guardar en password_hash (ahora existe en la DB)
            Phone = request.Phone,
            DateOfBirth = request.DateOfBirth.HasValue 
                ? DateTime.SpecifyKind(request.DateOfBirth.Value.Date, DateTimeKind.Unspecified) 
                : (DateTime?)null, // DateOfBirth es solo fecha, sin hora
            HaveCreditCards = request.HaveCreditCards,
            HaveLoans = request.HaveLoans,
            Token = token, // Token generado automáticamente en el servidor
            TouringStatus = request.TouringStatus,
            PreferredCurrency = request.PreferredCurrency ?? "USD",
            CreatedDate = DateTime.UtcNow // DateTime.UtcNow ya viene con Kind=Utc
        };

        var createdUser = await _repository.AddAsync(user, cancellationToken);
        return createdUser.ToDto();
    }
}

