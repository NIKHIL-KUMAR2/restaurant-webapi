using System;
using System.ComponentModel.DataAnnotations;

namespace Restaurant_WebAPI.Models
{
    public class RefreshToken
    {
        public string Token { get; set; }
        public string Subject { get; set; }
        public DateTime ExpiresUtc { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }
    }

    public class RefreshTokenModel
    {
        [Required]
        public string RefreshToken { get; set; }
    }

    public class AuthResult
    {
        public bool IsValid { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; } = 900;
    }
}
