using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class TodosController : ControllerBase
{
    private readonly AppDbContext _context;

    public TodosController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<IEnumerable<TodoTask>>> GetTasksByUsername(string username)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return NotFound();
        }
        var tasks = _context.TodoTasks.Where(t => t.UserId == user.Id).ToList();
        return Ok(tasks);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTask([FromBody] TodoTask task)
    {
        _context.TodoTasks.Add(task);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetTasksByUsername), new { username = task.User.Username }, task);
    }
}