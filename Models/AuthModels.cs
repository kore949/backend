namespace ProjectManagementAPI.Models
{
    public class RegisterRequest
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string Purpose { get; set; } // "Registration" or "PasswordReset"
    }

    public class ResendOtpRequest
    {
        public string Email { get; set; }
        public string Purpose { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }

    public class ResetPasswordRequest
    {
        public string Email { get; set; }
        public string OtpCode { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
    public class UpdateProfileRequest
    {
        public string FullName { get; set; }
        public string? ProfilePhoto { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }
}