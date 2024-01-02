using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem_ASP.Net_MVC.Data;

namespace TaskManagementSystem_ASP.Net_MVC.Controllers
{
    public class TaskController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TaskController> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        public TaskController(ApplicationDbContext context, ILogger<TaskController> logger, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }

        // GET: Task
        [Authorize]
        public async Task<IActionResult> Index(string sortBy)
        {

            ViewBag.SortBy = sortBy;

            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser != null)
            {
                var tasks = await _context.Task.Where(t => t.AssignedUser == currentUser.Id).ToListAsync();

                return View(SortTasks(tasks, sortBy));
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        private List<Models.Task> SortTasks(List<Models.Task> tasks, string sortBy)
        {
            switch (sortBy)
            {
                case "DueDate":
                    tasks = tasks.OrderBy(t => t.DueDate).ToList();
                    break;
                case "Priority":
                    tasks = tasks.OrderBy(t => t.Priority == "High" ? 0 : t.Priority == "Medium" ? 1 : 2).ToList();
                    break;
                case "Title":
                    tasks = tasks.OrderBy(t => t.Title).ToList();
                    break;
            }

            return tasks;
        }

        // GET: Task/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // GET: Task/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Task/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,DueDate,Priority,AssignedUser,IsCategories")] Models.Task task)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (ModelState.IsValid)
            {
                if (currentUser != null)
                {
                    task.AssignedUser = currentUser.Id;

                    _context.Add(task);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    _logger.LogError("Current user is null.");
                }
            }
            else
            {
                _logger.LogError("Model state is invalid. ModelState: {@ModelState}", ModelState);
            }

            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                _logger.LogError(error.ErrorMessage);
            }

            return View(task);
        }

        // GET: Task/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var task = await _context.Task.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }
            return View(task);
        }

        // POST: Task/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,DueDate,Priority,AssignedUser,IsCategories")] Models.Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                task.AssignedUser = currentUser.Id;

                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        // GET: Task/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Task == null)
            {
                return NotFound();
            }

            var task = await _context.Task
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            return View(task);
        }

        // POST: Task/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Task == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Task'  is null.");
            }
            var task = await _context.Task.FindAsync(id);
            if (task != null)
            {
                _context.Task.Remove(task);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
          return (_context.Task?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
