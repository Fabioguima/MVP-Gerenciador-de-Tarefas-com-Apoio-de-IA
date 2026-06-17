using MvpGerenciadorTarefasIa.Backend.Models;
using MvpGerenciadorTarefasIa.Backend.Repositories;

namespace MvpGerenciadorTarefasIa.Backend.Services;

public sealed class AiSupportService(InMemoryTaskRepository repository)
{
    public IReadOnlyList<string> SuggestTasks(AiGoalRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Objetivo))
        {
            throw new ArgumentException("Informe um objetivo para receber sugestoes.");
        }

        var suggestions = new[]
        {
            $"Definir o resultado esperado para: {request.Objetivo.Trim()}",
            "Quebrar o objetivo em etapas pequenas e verificaveis",
            "Separar uma tarefa de revisao e validacao final"
        };

        Register(AiInteractionType.SugestaoTarefa, request.Objetivo, string.Join(Environment.NewLine, suggestions), null);
        return suggestions;
    }

    public IReadOnlyList<TaskItem> PrioritizeTasks()
    {
        var ordered = repository.GetTasks()
            .Where(task => task.Status is not TaskStatusKind.Concluida and not TaskStatusKind.Cancelada)
            .OrderByDescending(task => task.Prioridade)
            .ThenBy(task => task.DataLimite ?? DateOnly.MaxValue)
            .ToList();

        Register(AiInteractionType.Priorizacao, "Priorizar tarefas pendentes", $"{ordered.Count} tarefa(s) priorizada(s).", null);
        return ordered;
    }

    public string ImproveDescription(AiDescriptionRequest request)
    {
        var task = repository.GetTask(request.TarefaId) ?? throw new KeyNotFoundException("Tarefa nao encontrada.");
        var source = string.IsNullOrWhiteSpace(request.Descricao) ? task.Descricao : request.Descricao.Trim();
        var improved = $"Objetivo: {task.Titulo}. Detalhamento revisado: {source}. Criterio de conclusao: validar e registrar o resultado.";

        Register(AiInteractionType.ReescritaDescricao, source, improved, task.Id);
        return improved;
    }

    public IReadOnlyList<AiInteraction> GetInteractions() => repository.GetInteractions();

    private void Register(AiInteractionType type, string prompt, string response, Guid? taskId)
    {
        repository.AddInteraction(new AiInteraction(
            Guid.NewGuid(),
            repository.DemoUser.Id,
            taskId,
            type,
            prompt,
            response,
            DateTimeOffset.UtcNow));
    }
}
