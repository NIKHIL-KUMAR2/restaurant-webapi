using Restaurant_WebAPI.Enums;
using Restaurant_WebAPI.Exceptions;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenRepository tokenRepository;

        public AccountService(IUserRepository userRepository, ITokenRepository tokenRepository)
        {
            this.userRepository = userRepository;
            this.tokenRepository = tokenRepository;
        }

        public bool AddNewAccount(SignUpRequest signUpRequest)
        {
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(signUpRequest.Password);
            UserModel userModel = new UserModel()
            {
                Email = signUpRequest.Email,
                FirstName = signUpRequest.FirstName,
                LastName = signUpRequest.LastName,
                Role = UserRole.Customer,
                PasswordHash = hashedPassword
            };

            int result = userRepository.AddNewUser(userModel);
            if (result == 1)
            {
                return true;
            }
            else if (result == 0)
            {
                return false;
            }
            else
            {
                throw new AccountRequestException("Unexpected error has occured");
            }
        }

        public bool UpdateAccount(UpdateRequest updateRequest, int userId)
        {
            if (!string.IsNullOrEmpty(updateRequest.Password))
            {
                updateRequest.Password = BCrypt.Net.BCrypt.HashPassword(updateRequest.Password);
            }
            int result = userRepository.UpdateUser(updateRequest, userId);
            if (result == 1)
            {
                return true;
            }
            else if (result == 0)
            {
                return false;
            }
            else
            {
                throw new AccountRequestException("Unexpected error has occured");
            }
        }

        public bool DeactivateAccount(int userId)
        {
            int result = userRepository.DeactivateUser(userId);

            // Removing Refresh token from DB of Deactivated Account
            tokenRepository.RemoveRefreshTokenByUserId(userId.ToString());

            if (result == 1)
            {
                return true;
            }
            else if (result == 0)
            {
                return false;
            }
            else
            {
                throw new AccountRequestException("Unexpected error has occured");
            }
        }
    }
}
