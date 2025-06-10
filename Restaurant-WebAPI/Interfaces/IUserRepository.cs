using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Interfaces
{
    public interface IUserRepository
    {
        UserModel GetUserByEmail(string email);

        int AddNewUser(UserModel model);

        int UpdateUser(UpdateRequest request, int userId);

        int DeactivateUser(int userId);
    }
}
