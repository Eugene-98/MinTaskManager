using Microsoft.EntityFrameworkCore;
using TaskManager.Models;

var builder = WebApplication.CreateBuilder(args);
builder
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
var app = builder.Build();

app.MapGet("/todoitems", async (TodoDb db) =>
	await db.Todos.Select(x => new TodoItemDTO(x)).ToListAsync());

app.MapGet("/todoitems/complete", async (TodoDb db) =>
	await db.Todos.Where(t => t.IsComlete).ToListAsync());

app.MapGet("/todoitems/{id}", async(int id, TodoDb db) =>
	await db.Todos.FindAsync(id)
		is TodoItem todo
			? Results.Ok(new TodoItemDTO(todo))
			: Results.NotFound());

app.MapPost("/todoitems", async(TodoItemDTO todoItemDTO, TodoDb db) =>
	{
		var todoItem = new TodoItem
		{
			IsComlete = todoItemDTO.IsComlete,
			Name = todoItemDTO.Name
		};
		db.Todos.Add(todoItem);
		await db.SaveChangesAsync();

		return Results.Created($"/todoitems/{todoItem.Id}", new TodoItemDTO(todoItem));
	});

app.MapPut("/todoitems/{id}", async (int id, TodoItemDTO todoItemDTO, TodoDb db) =>
{
	var todo = await db.Todos.FindAsync(id);
	if (todo is null) return Results.NotFound();
	todo.Name = todoItemDTO.Name;
	todo.IsComlete = todoItemDTO.IsComlete;
	await db.SaveChangesAsync();
	return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDb db) =>
{
	if (await db.Todos.FindAsync(id) is TodoItem todo)
	{
		db.Todos.Remove(todo);
		await db.SaveChangesAsync();
		return Results.Ok(new TodoItemDTO(todo));
	}
	return Results.NotFound();
});

app.Run();
