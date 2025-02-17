using api.Dtos.User;
using api.Models;

namespace api.Mappers
{
    public static class UserMappers
    {
        public static UserRequestDto ToUserDto(this User userModel)
        {
            return new UserRequestDto
            {
                Id = userModel.Id,
                Username = userModel.Username,
                Email = userModel.Email,
                DateTime = userModel.DateTime
            };
        }
    }
}