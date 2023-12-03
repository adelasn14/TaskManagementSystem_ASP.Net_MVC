using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagementSystem_ASP.Net_MVC.Models;

namespace TaskManagementSystem_ASP.Net_MVC.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<TaskManagementSystem_ASP.Net_MVC.Models.Task> Task { get; set; } = default!;
}

