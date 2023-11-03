using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskmgmtAPI.Authmanager;
using TaskmgmtAPI.Db;
using TaskmgmtAPI.Models;

namespace TaskmgmtAPI.Controllers.Auth
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Context _context;
        private readonly JwtAuthenticationManager _jwtManager;

        public AuthController(JwtAuthenticationManager jwtManager, Context context) {
            _jwtManager = jwtManager;
            _context = context;
        }


        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] UserloginDto credentials) {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authUser = await _context.User.FirstOrDefaultAsync(u => u.email == credentials.email);

            if (authUser == null) {
                ModelState.AddModelError("email", "email or password is not correct.");
                return UnprocessableEntity(ModelState);
            };

            if (BCrypt.Net.BCrypt.Verify(credentials.password, authUser.password) == false) {
                ModelState.AddModelError("email", "email or password is not correct.");
                return UnprocessableEntity(ModelState);
            };

            var result = new {
                authUser = authUser,
                accesToken = _jwtManager.authenticate(authUser.id,authUser.email)
            };
            return Ok(result);
        }


        [Authorize]
        [HttpGet("user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Authuser()
        {
            var authUser = await new AuthHelper(this._context).GetAuthenticatedUser(HttpContext);
            if(authUser == null) return BadRequest("User information not found.");
            return Ok(authUser);

        }


        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] RegisterDto registerParams)
        {

            if (registerParams is null)
            {
                return BadRequest("Invalid user data.");
            }

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity(ModelState);
            }

            var user = new User()
            {
                name = registerParams.name,
                email = registerParams.email,
                password = BCrypt.Net.BCrypt.HashPassword(registerParams.password),
                createdAt = DateTime.Now,
                updatedAt = DateTime.Now
            };

            await _context.User.AddAsync(user);

            await _context.SaveChangesAsync();
            //var newUser =  await GetUserByID(user.id) as User;
            return NoContent();
        }


        [HttpPut("updatecredentials/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateCredentials(int id, User user)
        {
            if (id != user.id) return BadRequest();
             
            user.updatedAt = DateTime.Now;
            _context.Entry(user).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }

}
