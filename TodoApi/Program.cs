using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));

var app = builder.Build();

app.MapGet("/todoitems", async (TodoDb db) => await db.Todos.ToListAsync());
app.MapGet("/todoitems/{id:int}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is TodoItem todo)
    {
        return Results.Ok(todo);
    }
    return Results.NotFound();
});
app.MapPost("/todoitems", async (TodoItem todo, TodoDb db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todoitems/{todo.Id}", todo);
});
app.MapPut("/todoitems/{id:int}", async (int id, TodoItem inputTodo, TodoDb db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();
    todo.Name = inputTodo.Name;
    todo.IsComplete = inputTodo.IsComplete;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id:int}", async (int id, TodoDb db) =>
{
    if (await db.Todos.FindAsync(id) is TodoItem todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

app.Run();
