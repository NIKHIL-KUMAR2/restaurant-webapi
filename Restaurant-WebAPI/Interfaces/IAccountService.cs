using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Interfaces
{
    public interface IAccountService
    {
        bool AddNewAccount(SignUpRequest signUpRequest);
        bool UpdateAccount(UpdateRequest updateRequest, int userId);
        bool DeactivateAccount(int userId);
    }
}
