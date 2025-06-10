using System.ComponentModel.DataAnnotations;

namespace Restaurant_WebAPI.Models
{
    public class UpdateRequest
    {
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
    ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [StringLength(50, ErrorMessage = "First name can't be longer than 50 characters.")]
        public string FirstName { get; set; }

        [StringLength(50, ErrorMessage = "Last name can't be longer than 50 characters.")]
        public string LastName { get; set; }

        [StringLength(200, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string Password { get; set; }
    }
}
