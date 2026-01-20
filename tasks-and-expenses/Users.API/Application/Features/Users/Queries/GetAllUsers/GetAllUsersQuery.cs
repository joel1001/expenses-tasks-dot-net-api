using MediatR;
using Users.API.Application.DTOs;

namespace Users.API.Application.Features.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<IEnumerable<UserDto>>
{
}

