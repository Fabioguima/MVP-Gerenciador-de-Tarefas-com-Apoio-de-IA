namespace MvpGerenciadorTarefasIa.Backend.Models;

public enum TaskStatusKind
{
    Pendente,
    EmAndamento,
    Concluida,
    Cancelada
}

public enum TaskPriority
{
    Baixa,
    Media,
    Alta
}

public enum AiInteractionType
{
    SugestaoTarefa,
    Priorizacao,
    Resumo,
    QuebraEmSubtarefas,
    ReescritaDescricao
}

public sealed record User(
    Guid Id,
    string Nome,
    string Email,
    string SenhaHash,
    DateTimeOffset DataCriacao,
    DateTimeOffset DataAtualizacao);

public sealed record Category(
    Guid Id,
    Guid UsuarioId,
    string Nome,
    string Cor,
    DateTimeOffset CriadaEm);

public sealed record TaskItem(
    Guid Id,
    Guid UsuarioId,
    Guid? CategoriaId,
    string Titulo,
    string Descricao,
    TaskStatusKind Status,
    TaskPriority Prioridade,
    DateOnly? DataLimite,
    DateTimeOffset CriadaEm,
    DateTimeOffset AtualizadaEm,
    DateTimeOffset? ConcluidaEm);

public sealed record AiInteraction(
    Guid Id,
    Guid UsuarioId,
    Guid? TarefaId,
    AiInteractionType TipoInteracao,
    string Prompt,
    string Resposta,
    DateTimeOffset CriadaEm);
