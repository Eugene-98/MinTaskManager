using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

namespace TaskManager.Context
{
	public class TodoContext : DbContext
	{
		public TodoContext(DbContextOptions<TodoContext> options)
			: base(options) { }
		public DbSet<TodoItem> TodoItems { get; set; } = null;
	}
}
