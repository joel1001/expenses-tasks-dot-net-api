using MediatR;
using Users.API.Application.Interfaces;

namespace Users.API.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserRepository _repository;

    public UpdateUserCommandHandler(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.Id} not found.");
        }

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        // Email NO se actualiza - no se puede cambiar después de la creación
        // El email se mantiene con el valor original del usuario (ignorar request.Email)
        // user.Email = request.Email; // COMENTADO: El email no se puede cambiar
        user.Phone = request.Phone;
        user.DateOfBirth = request.DateOfBirth;
        user.HaveCreditCards = request.HaveCreditCards;
        user.HaveLoans = request.HaveLoans;
        // Token no se actualiza desde UpdateUserCommand (se maneja por separado si es necesario)
        user.TouringStatus = request.TouringStatus;
        user.PreferredCurrency = request.PreferredCurrency;
        user.UpdatedDate = DateTime.UtcNow;

        await _repository.UpdateAsync(user, cancellationToken);
        return Unit.Value;
    }
}
