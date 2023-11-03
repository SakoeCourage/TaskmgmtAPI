using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TaskmgmtAPI.Db;
using TaskmgmtAPI.Models;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using TaskmgmtAPI.Helpers;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Authorization;
using TaskmgmtAPI.Controllers.Auth;

namespace TaskmgmtAPI.Controllers
{
    public class UserTaskApiResponse
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public string? NextPageUrl { get; set; }
        public string? PreviousPageUrl { get; set; }
        public List<UserTask> UserTasks { get; set; }
    }

    [Authorize]
    [Route("api/task")]
    [ApiController]
    public class UserTaskController : ControllerBase
    {
        private readonly Context _context;
        public UserTaskController(Db.Context context) => _context = context;

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserTaskApiResponse>> AllTask([FromQuery] int? page = 1, [FromQuery] int? pageSize = 10, [FromQuery] string? search = "")
        {
         
            var TaskQuery = _context.UserTask.AsQueryable();
          
            if (!string.IsNullOrWhiteSpace(search))
            {
                TaskQuery = TaskQuery.Where(task => task.description.Contains(search) || task.title.Contains(search))
                            .OrderBy(t => t.createdAt);
            }
           
            var pagedList = Helpers.PagedList<UserTask>.ToPagedList(TaskQuery.OrderBy(t => t.createdAt).Include(t => t.author), page ?? 1, pageSize ?? 10, this.HttpContext);

            var authUser = await new AuthHelper(this._context).GetAuthenticatedUser(HttpContext);

            var uTask =  pagedList.Select(u_task => {
                return new
                {
                    id = u_task.id,
                    authorID = u_task.authorID,
                    author = u_task.author, 
                    title = u_task.title,
                    description = u_task.description,
                    isCompletes = u_task.isCompleted,
                    createAt = u_task.createdAt,
                    isAuthor = u_task.authorID == authUser.id
                };
            });


            var response = new
            {
                tasks  = uTask,
                CurrentPage = pagedList.CurrentPage,
                TotalPages = pagedList.TotalPages,
                PageSize = pagedList.PageSize,
                TotalCount = pagedList.TotalCount,
                prevPageUrl = pagedList.prevPageUrl,
                nextPageUrl = pagedList.nextPageUrl
             };


            return Ok(response);


        }

        [Authorize]
        [HttpGet("getTaskById/{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTaksByID(int id)
        {
            var u_task = await _context.UserTask.Include(t => t.author)
                .Where(t => t.id == id)
                .FirstOrDefaultAsync();

            if (u_task == null) return NotFound();

            var authUser = await new AuthHelper(this._context).GetAuthenticatedUser(HttpContext);

            var uTask = new
                {
                    id = u_task.id,
                    authorID = u_task.authorID,
                    author = u_task.author,
                    title = u_task.title,
                    description = u_task.description,
                    isCompletes = u_task.isCompleted,
                    createAt = u_task.createdAt,
                    isAuthor = u_task.authorID == authUser.id
                };

            return Ok(uTask);
        }

        [Authorize]
        [HttpPost("newTask")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTask([FromBody] UserTask userTask)
        {
            if (userTask is null)
            {
                return BadRequest("Invalid user data.");
            }

            var authUser = await new AuthHelper(this._context).GetAuthenticatedUser(HttpContext);

            userTask.createdAt = DateTime.UtcNow;
            userTask.updatedAt = DateTime.UtcNow;
            userTask.isCompleted = false;
            userTask.authorID = authUser.id;
            userTask.author = await _context.User.FirstOrDefaultAsync(user => user.id == userTask.authorID);

            if (!ModelState.IsValid)
            {
                return UnprocessableEntity();
            }

            await _context.UserTask.AddAsync(userTask);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTaksByID), new { id = userTask.id }, userTask);
        }

        [Authorize] 
        [HttpPut("ToggleTaskCompletion/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleTaskAsComplete(int id)
        {
            var task = await _context.UserTask.FindAsync(id);

            if (task is null) return NotFound();

            task.isCompleted = !task.isCompleted;
            task.updatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTask([FromBody] int id)
        {
            var task = await _context.UserTask.FindAsync(id);

            if (task is null) return NotFound();

            _context.UserTask.Remove(task);

            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
