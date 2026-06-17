using System.Text.Json.Serialization;

namespace MvpGerenciadorTarefasIa.Backend.Models;

public sealed record CategoryCreateRequest(string Nome, string Cor);

public sealed record TaskCreateRequest(
    string Titulo,
    string Descricao,
    Guid? CategoriaId,
    TaskPriority Prioridade,
    DateOnly? DataLimite);

public sealed record TaskUpdateRequest(
    string Titulo,
    string Descricao,
    Guid? CategoriaId,
    TaskStatusKind Status,
    TaskPriority Prioridade,
    DateOnly? DataLimite);

public sealed record TaskQuery(
    [property: JsonPropertyName("status")] TaskStatusKind? Status,
    [property: JsonPropertyName("priority")] TaskPriority? Prioridade,
    [property: JsonPropertyName("categoryId")] Guid? CategoriaId);

public sealed record AiGoalRequest(string Objetivo);

public sealed record AiDescriptionRequest(Guid TarefaId, string Descricao);
