using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using MvpGerenciadorTarefasIa.Backend.Models;
using MvpGerenciadorTarefasIa.Backend.Repositories;
using MvpGerenciadorTarefasIa.Backend.Services;

var runner = new TestRunner();

await runner.RunAsync("Unitario: valida titulo obrigatorio na criacao de tarefa", UnitTaskTitleValidation);
await runner.RunAsync("Integracao: cria categoria, cria tarefa, filtra e registra IA", IntegrationTaskFlow);
await runner.RunAsync("Sistema E2E: API real cria e conclui tarefa por HTTP", EndToEndHttpFlow);

runner.PrintSummary();
return runner.FailedCount == 0 ? 0 : 1;

static Task UnitTaskTitleValidation()
{
    var service = new TaskService(new InMemoryTaskRepository());

    Assert.Throws<ArgumentException>(() => service.CreateTask(new TaskCreateRequest(
        "",
        "Descricao qualquer",
        null,
        TaskPriority.Media,
        null)));

    return Task.CompletedTask;
}

static Task IntegrationTaskFlow()
{
    var repository = new InMemoryTaskRepository();
    var taskService = new TaskService(repository);
    var aiService = new AiSupportService(repository);

    var category = taskService.CreateCategory(new CategoryCreateRequest("Faculdade", "#0f766e"));
    var task = taskService.CreateTask(new TaskCreateRequest(
        "Implementar CRUD de tarefas",
        "Criar endpoints e validacoes",
        category.Id,
        TaskPriority.Alta,
        DateOnly.FromDateTime(DateTime.Today.AddDays(2))));

    var highPriorityTasks = taskService.GetTasks(new TaskQuery(TaskStatusKind.Pendente, TaskPriority.Alta, category.Id));
    Assert.Equal(1, highPriorityTasks.Count, "O filtro deveria encontrar a tarefa criada.");
    Assert.Equal(task.Id, highPriorityTasks[0].Id, "A tarefa filtrada deveria ser a tarefa criada.");

    var improved = aiService.ImproveDescription(new AiDescriptionRequest(task.Id, task.Descricao));
    Assert.Contains("Criterio de conclusao", improved, "A IA simulada deveria melhorar a descricao.");
    Assert.Equal(1, aiService.GetInteractions().Count, "A interacao de IA deveria ser registrada.");

    return Task.CompletedTask;
}

static async Task EndToEndHttpFlow()
{
    var projectRoot = LocateProjectRoot();
    var port = 5197;
    using var process = StartBackend(projectRoot, port);

    try
    {
        using var client = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{port}") };
        await WaitForHealthAsync(client);

        var category = await PostAsync<Category>(client, "/api/categories", new CategoryCreateRequest("Trabalho", "#1f6feb"));
        var created = await PostAsync<TaskItem>(client, "/api/tasks", new TaskCreateRequest(
            "Preparar apresentacao",
            "Montar roteiro e validar demo",
            category.Id,
            TaskPriority.Alta,
            DateOnly.FromDateTime(DateTime.Today.AddDays(1))));

        Assert.Equal(TaskStatusKind.Pendente, created.Status, "A tarefa deve nascer pendente.");

        var completed = await PatchAsync<TaskItem>(client, $"/api/tasks/{created.Id}/complete");
        Assert.Equal(TaskStatusKind.Concluida, completed.Status, "A tarefa deve ser concluida pelo endpoint.");
        Assert.True(completed.ConcluidaEm is not null, "A conclusao deve registrar data/hora.");

        var completedTasks = await client.GetFromJsonAsync<List<TaskItem>>(
            "/api/tasks?status=Concluida&priority=Alta",
            JsonOptions());
        Assert.Equal(1, completedTasks?.Count ?? 0, "O filtro HTTP deve retornar a tarefa concluida.");
    }
    finally
    {
        if (!process.HasExited)
        {
            process.Kill(entireProcessTree: true);
            await process.WaitForExitAsync();
        }
    }
}

static Process StartBackend(string projectRoot, int port)
{
    var backendPath = Path.Combine(projectRoot, "backend", "backend.csproj");
    var process = new Process
    {
        StartInfo = new ProcessStartInfo("dotnet", $"run --project \"{backendPath}\" --urls http://127.0.0.1:{port}")
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        }
    };

    process.Start();
    return process;
}

static async Task WaitForHealthAsync(HttpClient client)
{
    var deadline = DateTimeOffset.UtcNow.AddSeconds(25);
    Exception? lastError = null;

    while (DateTimeOffset.UtcNow < deadline)
    {
        try
        {
            var response = await client.GetAsync("/health");
            if (response.IsSuccessStatusCode)
            {
                return;
            }
        }
        catch (Exception ex)
        {
            lastError = ex;
        }

        await Task.Delay(500);
    }

    throw new TimeoutException($"A API nao iniciou dentro do tempo esperado. Ultimo erro: {lastError?.Message}");
}

static async Task<T> PostAsync<T>(HttpClient client, string path, object body)
{
    var response = await client.PostAsJsonAsync(path, body, JsonOptions());
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<T>(JsonOptions())
        ?? throw new InvalidOperationException($"Resposta vazia em {path}.");
}

static async Task<T> PatchAsync<T>(HttpClient client, string path)
{
    var request = new HttpRequestMessage(HttpMethod.Patch, path);
    var response = await client.SendAsync(request);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadFromJsonAsync<T>(JsonOptions())
        ?? throw new InvalidOperationException($"Resposta vazia em {path}.");
}

static JsonSerializerOptions JsonOptions()
{
    var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
    options.Converters.Add(new JsonStringEnumConverter());
    return options;
}

static string LocateProjectRoot()
{
    var candidates = new[]
    {
        new DirectoryInfo(Environment.CurrentDirectory),
        new DirectoryInfo(AppContext.BaseDirectory)
    };

    foreach (var candidate in candidates)
    {
        var current = candidate;

        while (current is not null)
        {
            if (File.Exists(Path.Combine(current.FullName, "MvpGerenciadorTarefasIa.sln"))
                || File.Exists(Path.Combine(current.FullName, "MvpGerenciadorTarefasIa.slnx")))
            {
                return current.FullName;
            }

            current = current.Parent;
        }
    }

    throw new DirectoryNotFoundException("Nao foi possivel localizar a raiz da solucao.");
}

sealed class TestRunner
{
    private int passedCount;
    public int FailedCount { get; private set; }

    public async Task RunAsync(string name, Func<Task> test)
    {
        try
        {
            await test();
            passedCount++;
            Console.WriteLine($"[PASSOU] {name}");
        }
        catch (Exception ex)
        {
            FailedCount++;
            Console.WriteLine($"[FALHOU] {name}");
            Console.WriteLine($"        {ex.GetType().Name}: {ex.Message}");
        }
    }

    public void PrintSummary()
    {
        Console.WriteLine();
        Console.WriteLine($"Resumo: {passedCount} passou/passaram, {FailedCount} falhou/falharam.");
    }
}

static class Assert
{
    public static void Equal<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"{message} Esperado: {expected}. Atual: {actual}.");
        }
    }

    public static void True(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    public static void Contains(string expected, string actual, string message)
    {
        if (!actual.Contains(expected, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(message);
        }
    }

    public static void Throws<TException>(Action action) where TException : Exception
    {
        try
        {
            action();
        }
        catch (TException)
        {
            return;
        }

        throw new InvalidOperationException($"Era esperada uma excecao do tipo {typeof(TException).Name}.");
    }
}
