using MvpGerenciadorTarefasIa.Backend.Models;
using MvpGerenciadorTarefasIa.Backend.Repositories;

namespace MvpGerenciadorTarefasIa.Backend.Services;

public sealed class TaskService(InMemoryTaskRepository repository)
{
    public Category CreateCategory(CategoryCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
        {
            throw new ArgumentException("O nome da categoria e obrigatorio.");
        }

        var color = string.IsNullOrWhiteSpace(request.Cor) ? "#2563eb" : request.Cor;
        return repository.AddCategory(request.Nome, color);
    }

    public IReadOnlyList<Category> GetCategories() => repository.GetCategories();

    public TaskItem CreateTask(TaskCreateRequest request)
    {
        ValidateTaskFields(request.Titulo, request.CategoriaId);

        var now = DateTimeOffset.UtcNow;
        var task = new TaskItem(
            Guid.NewGuid(),
            repository.DemoUser.Id,
            request.CategoriaId,
            request.Titulo.Trim(),
            request.Descricao.Trim(),
            TaskStatusKind.Pendente,
            request.Prioridade,
            request.DataLimite,
            now,
            now,
            null);

        return repository.AddTask(task);
    }

    public IReadOnlyList<TaskItem> GetTasks(TaskQuery query)
    {
        return repository.GetTasks()
            .Where(task => query.Status is null || task.Status == query.Status)
            .Where(task => query.Prioridade is null || task.Prioridade == query.Prioridade)
            .Where(task => query.CategoriaId is null || task.CategoriaId == query.CategoriaId)
            .ToList();
    }

    public TaskItem UpdateTask(Guid id, TaskUpdateRequest request)
    {
        var existing = repository.GetTask(id) ?? throw new KeyNotFoundException("Tarefa nao encontrada.");
        ValidateTaskFields(request.Titulo, request.CategoriaId);

        DateTimeOffset? completedAt = request.Status == TaskStatusKind.Concluida
            ? existing.ConcluidaEm ?? DateTimeOffset.UtcNow
            : null;

        return repository.SaveTask(existing with
        {
            Titulo = request.Titulo.Trim(),
            Descricao = request.Descricao.Trim(),
            CategoriaId = request.CategoriaId,
            Status = request.Status,
            Prioridade = request.Prioridade,
            DataLimite = request.DataLimite,
            AtualizadaEm = DateTimeOffset.UtcNow,
            ConcluidaEm = completedAt
        });
    }

    public TaskItem CompleteTask(Guid id)
    {
        var existing = repository.GetTask(id) ?? throw new KeyNotFoundException("Tarefa nao encontrada.");

        return repository.SaveTask(existing with
        {
            Status = TaskStatusKind.Concluida,
            AtualizadaEm = DateTimeOffset.UtcNow,
            ConcluidaEm = DateTimeOffset.UtcNow
        });
    }

    public bool DeleteTask(Guid id) => repository.DeleteTask(id);

    private void ValidateTaskFields(string titulo, Guid? categoriaId)
    {
        if (string.IsNullOrWhiteSpace(titulo))
        {
            throw new ArgumentException("O titulo da tarefa e obrigatorio.");
        }

        if (categoriaId is not null && repository.GetCategory(categoriaId.Value) is null)
        {
            throw new ArgumentException("Categoria informada nao existe.");
        }
    }
}
