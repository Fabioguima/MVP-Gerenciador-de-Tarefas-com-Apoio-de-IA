using MvpGerenciadorTarefasIa.Backend.Models;

namespace MvpGerenciadorTarefasIa.Backend.Repositories;

public sealed class InMemoryTaskRepository
{
    private readonly object gate = new();
    private readonly Dictionary<Guid, Category> categories = new();
    private readonly Dictionary<Guid, TaskItem> tasks = new();
    private readonly List<AiInteraction> aiInteractions = [];

    public User DemoUser { get; } = new(
        Guid.Parse("11111111-1111-1111-1111-111111111111"),
        "Fabio Guimaraes",
        "fabio@example.com",
        "demo_hash",
        DateTimeOffset.UtcNow,
        DateTimeOffset.UtcNow);

    public Category AddCategory(string nome, string cor)
    {
        var category = new Category(Guid.NewGuid(), DemoUser.Id, nome.Trim(), cor.Trim(), DateTimeOffset.UtcNow);

        lock (gate)
        {
            categories[category.Id] = category;
        }

        return category;
    }

    public IReadOnlyList<Category> GetCategories()
    {
        lock (gate)
        {
            return categories.Values.OrderBy(category => category.Nome).ToList();
        }
    }

    public Category? GetCategory(Guid id)
    {
        lock (gate)
        {
            return categories.GetValueOrDefault(id);
        }
    }

    public TaskItem AddTask(TaskItem task)
    {
        lock (gate)
        {
            tasks[task.Id] = task;
        }

        return task;
    }

    public TaskItem? GetTask(Guid id)
    {
        lock (gate)
        {
            return tasks.GetValueOrDefault(id);
        }
    }

    public IReadOnlyList<TaskItem> GetTasks()
    {
        lock (gate)
        {
            return tasks.Values
                .OrderByDescending(task => task.Prioridade)
                .ThenBy(task => task.DataLimite ?? DateOnly.MaxValue)
                .ThenBy(task => task.Titulo)
                .ToList();
        }
    }

    public TaskItem SaveTask(TaskItem task)
    {
        lock (gate)
        {
            tasks[task.Id] = task;
        }

        return task;
    }

    public bool DeleteTask(Guid id)
    {
        lock (gate)
        {
            return tasks.Remove(id);
        }
    }

    public AiInteraction AddInteraction(AiInteraction interaction)
    {
        lock (gate)
        {
            aiInteractions.Add(interaction);
        }

        return interaction;
    }

    public IReadOnlyList<AiInteraction> GetInteractions()
    {
        lock (gate)
        {
            return aiInteractions.OrderByDescending(interaction => interaction.CriadaEm).ToList();
        }
    }
}
