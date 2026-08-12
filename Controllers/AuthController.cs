using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Services;

namespace ProjectManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserRepository _userRepository;
        private readonly OtpRepository _otpRepository;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;

        public AuthController(
            UserRepository userRepository,
            OtpRepository otpRepository,
            IEmailService emailService,
            IJwtService jwtService)
        {
            _userRepository = userRepository;
            _otpRepository = otpRepository;
            _emailService = emailService;
            _jwtService = jwtService;
        }

        private string GenerateOtpCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private bool IsPasswordStrong(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8) return false;
            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));
            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (request.Password != request.ConfirmPassword)
                return BadRequest(new { message = "Passwords do not match." });

            if (!IsPasswordStrong(request.Password))
                return BadRequest(new { message = "Password must be at least 8 characters and include an uppercase letter, lowercase letter, number, and special character." });

            var existingUser = await _userRepository.GetUserByEmail(request.Email);
            if (existingUser != null)
                return BadRequest(new { message = "An account with this email already exists." });

            var createRequest = new CreateUserRequest
            {
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = request.Password, // hashed inside CreateUser
                Role = "Team Member"
            };
            await _userRepository.CreateUser(createRequest);

            var otpCode = GenerateOtpCode();
            await _otpRepository.CreateOtp(request.Email, otpCode, "Registration", DateTime.UtcNow.AddMinutes(5));
            await _emailService.SendOtpEmail(request.Email, otpCode, "Registration");

            return Ok(new { message = "Account created. Please check your email for a verification code." });
        }

        // POST: api/auth/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            var otp = await _otpRepository.VerifyOtp(request.Email, request.OtpCode, request.Purpose);
            if (otp == null)
                return BadRequest(new { message = "Invalid verification code." });

            if (otp.Value.IsUsed)
                return BadRequest(new { message = "This code has already been used." });

            if (otp.Value.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { message = "This code has expired. Please request a new one." });

            await _otpRepository.MarkOtpUsed(otp.Value.OtpId);

            if (request.Purpose == "Registration")
                await _userRepository.SetUserVerified(request.Email);

            return Ok(new { message = "Verification successful." });
        }

        // POST: api/auth/resend-otp
        [HttpPost("resend-otp")]
        public async Task<IActionResult> ResendOtp([FromBody] ResendOtpRequest request)
        {
            var otpCode = GenerateOtpCode();
            await _otpRepository.CreateOtp(request.Email, otpCode, request.Purpose, DateTime.UtcNow.AddMinutes(5));
            await _emailService.SendOtpEmail(request.Email, otpCode, request.Purpose);

            return Ok(new { message = "A new verification code has been sent." });
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid email or password." });

            if (!user.IsVerified)
                return BadRequest(new { message = "Please verify your email before logging in." });

            if (!user.IsActive)
                return Unauthorized(new { message = "This account has been deactivated." });

            var token = _jwtService.GenerateToken(user);

            return Ok(new LoginResponse
            {
                Token = token,
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role
            });
        }

        // POST: api/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var user = await _userRepository.GetUserByEmail(request.Email);
            if (user == null)
                return Ok(new { message = "If that email exists, a code has been sent." }); // don't reveal existence

            var otpCode = GenerateOtpCode();
            await _otpRepository.CreateOtp(request.Email, otpCode, "PasswordReset", DateTime.UtcNow.AddMinutes(5));
            await _emailService.SendOtpEmail(request.Email, otpCode, "PasswordReset");

            return Ok(new { message = "If that email exists, a code has been sent." });
        }

        // POST: api/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest(new { message = "Passwords do not match." });

            if (!IsPasswordStrong(request.NewPassword))
                return BadRequest(new { message = "Password must be at least 8 characters and include an uppercase letter, lowercase letter, number, and special character." });

            var otp = await _otpRepository.VerifyOtp(request.Email, request.OtpCode, "PasswordReset");
            if (otp == null || otp.Value.IsUsed || otp.Value.ExpiresAt < DateTime.UtcNow)
                return BadRequest(new { message = "Invalid or expired code." });

            await _otpRepository.MarkOtpUsed(otp.Value.OtpId);

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserPassword(request.Email, hashedPassword);

            return Ok(new { message = "Password reset successful. You can now log in." });
        }
    }
}