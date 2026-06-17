using System.Text.Json.Serialization;
using MvpGerenciadorTarefasIa.Backend.Models;
using MvpGerenciadorTarefasIa.Backend.Repositories;
using MvpGerenciadorTarefasIa.Backend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddSingleton<InMemoryTaskRepository>();
builder.Services.AddSingleton<TaskService>();
builder.Services.AddSingleton<AiSupportService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/categories", (TaskService service) => service.GetCategories());
app.MapPost("/api/categories", (TaskService service, CategoryCreateRequest request) =>
{
    try
    {
        return Results.Created("/api/categories", service.CreateCategory(request));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapGet("/api/tasks", (
    TaskService service,
    TaskStatusKind? status,
    TaskPriority? priority,
    Guid? categoryId) => service.GetTasks(new TaskQuery(status, priority, categoryId)));

app.MapPost("/api/tasks", (TaskService service, TaskCreateRequest request) =>
{
    try
    {
        var task = service.CreateTask(request);
        return Results.Created($"/api/tasks/{task.Id}", task);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPut("/api/tasks/{id:guid}", (TaskService service, Guid id, TaskUpdateRequest request) =>
{
    try
    {
        return Results.Ok(service.UpdateTask(id, request));
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapPatch("/api/tasks/{id:guid}/complete", (TaskService service, Guid id) =>
{
    try
    {
        return Results.Ok(service.CompleteTask(id));
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapDelete("/api/tasks/{id:guid}", (TaskService service, Guid id) =>
    service.DeleteTask(id) ? Results.NoContent() : Results.NotFound(new { error = "Tarefa nao encontrada." }));

app.MapPost("/api/ai/suggest", (AiSupportService service, AiGoalRequest request) =>
{
    try
    {
        return Results.Ok(new { sugestoes = service.SuggestTasks(request) });
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/api/ai/prioritize", (AiSupportService service) => service.PrioritizeTasks());
app.MapPost("/api/ai/improve-description", (AiSupportService service, AiDescriptionRequest request) =>
{
    try
    {
        return Results.Ok(new { descricao = service.ImproveDescription(request) });
    }
    catch (KeyNotFoundException ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});
app.MapGet("/api/ai/interactions", (AiSupportService service) => service.GetInteractions());

app.Run();
