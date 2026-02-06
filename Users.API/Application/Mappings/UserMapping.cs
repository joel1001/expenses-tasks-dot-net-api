using Users.API.Application.DTOs;
using Users.API.Models;

namespace Users.API.Application.Mappings;

public static class UserMapping
{
    public static UserDto ToDto(this User user)
    {
        return new UserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            DateOfBirth = user.DateOfBirth,
            CreatedDate = user.CreatedDate,
            UpdatedDate = user.UpdatedDate,
            HaveCreditCards = user.HaveCreditCards,
            HaveLoans = user.HaveLoans,
            Token = user.Token,
            TouringStatus = user.TouringStatus,
            Role = user.Role,
            PreferredCurrency = user.PreferredCurrency
        };
    }
}
