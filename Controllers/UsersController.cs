using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Security.Claims;

namespace ProjectManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository _userRepository;

        public UsersController(UserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllUsers();
            return Ok(users);
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var user = await _userRepository.GetUserById(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        // POST: api/users
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            var newUserId = await _userRepository.CreateUser(request);
            await _userRepository.SetUserVerified(request.Email);
            return CreatedAtAction(nameof(GetById), new { id = newUserId }, new { UserId = newUserId });
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
        {
            await _userRepository.UpdateUser(id, request);
            return NoContent();
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _userRepository.DeleteUser(id);
            return NoContent();
        }

        // PATCH: api/users/bulk-status
        [HttpPatch("bulk-status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkSetActiveStatus([FromBody] BulkSetActiveStatusRequest request)
        {
            var rowsAffected = await _userRepository.BulkSetActiveStatus(request.UserIds, request.IsActive);
            return Ok(new { RowsAffected = rowsAffected });
        }
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetUserById(userId);
            return Ok(user);
        }

        [HttpPut("me")]
        public async Task<IActionResult> UpdateMe([FromBody] UpdateProfileRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _userRepository.UpdateOwnProfile(userId, request.FullName, request.ProfilePhoto);
            return NoContent();
        }

        [HttpPost("me/change-password")]
        public async Task<IActionResult> ChangeMyPassword([FromBody] ChangePasswordRequest request)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _userRepository.GetUserById(userId);
            var fullUser = await _userRepository.GetUserByEmail(user.Email); // has PasswordHash

            if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, fullUser.PasswordHash))
                return BadRequest(new { message = "Current password is incorrect." });

            if (request.NewPassword != request.ConfirmNewPassword)
                return BadRequest(new { message = "New passwords do not match." });

            var hashed = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateUserPassword(user.Email, hashed);
            return Ok(new { message = "Password changed successfully." });
        }
    }
}