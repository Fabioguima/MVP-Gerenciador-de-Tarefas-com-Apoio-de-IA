# Planejamento Inicial - Trabalho Pratico

## MVP: Gerenciador de Tarefas com Apoio de IA

## 1. Integrante

- Nome: Fabio Guimaraes
- Desenvolvimento: individual

## 2. Tecnologias Escolhidas

Para o desenvolvimento do MVP, foram escolhidas tecnologias simples, produtivas e adequadas para a criacao rapida de uma aplicacao funcional.

### Implementado no MVP atual

- Front-end: HTML, CSS e JavaScript servidos pela propria API ASP.NET Core
- Back-end: C# com ASP.NET Core usando Minimal APIs
- Banco de dados: repositorio em memoria, para facilitar execucao local e testes automatizados
- IA de apoio ao desenvolvimento: ChatGPT / Codex
- IA dentro da aplicacao: servico de IA simulado, com sugestao de tarefas, priorizacao, melhoria de descricao e registro das interacoes
- Testes: runner automatizado em C# com teste unitario, teste de integracao e teste E2E

## 3. Arquitetura Proposta

A aplicacao sera organizada em uma arquitetura cliente-servidor, separando a interface do usuario, a API e a camada de dados.

O front-end sera responsavel pela experiencia do usuario, exibindo tarefas, filtros, formularios e interacoes com os recursos de IA. Ele se comunica com o back-end por meio de requisicoes HTTP/REST.

O back-end sera responsavel pelas regras de negocio, validacao de dados, integracao com a camada de dados e chamadas para o servico de IA. A API tambem centraliza operacoes como criacao, edicao, conclusao, exclusao e consulta de tarefas.

O banco de dados armazena usuarios, tarefas, categorias, historico de interacoes com IA e metadados importantes para acompanhamento do uso da aplicacao.

### Como ficou no MVP atual

No MVP implementado, o front-end fica na pasta `backend/wwwroot` e e servido pela propria API ASP.NET Core. A camada de dados foi implementada com um repositorio em memoria chamado `InMemoryTaskRepository`.

Fluxo geral:

1. Usuario acessa o sistema pelo front-end.
2. Front-end envia requisicoes para a API.
3. API valida os dados e executa regras de negocio.
4. API consulta ou atualiza o repositorio de dados.
5. Quando necessario, a API envia uma solicitacao para o servico de IA simulado.
6. A resposta e retornada ao front-end e exibida ao usuario.

## 4. Modelagem I das Entidades

### Usuario

Representa a pessoa que utiliza o sistema.

- id
- nome
- email
- senha_hash
- data_criacao
- data_atualizacao

### Tarefa

Representa uma atividade cadastrada pelo usuario.

- id
- usuario_id
- categoria_id
- titulo
- descricao
- status
- prioridade
- data_limite
- criada_em
- atualizada_em
- concluida_em

Status previstos:

- `Pendente`
- `EmAndamento`
- `Concluida`
- `Cancelada`

Prioridades previstas:

- baixa
- media
- alta

No codigo, as prioridades foram implementadas como enum:

- `Baixa`
- `Media`
- `Alta`

### Categoria

Permite organizar tarefas por area ou contexto.

- id
- usuario_id
- nome
- cor
- criada_em

### InteracaoIA

Registra o uso da IA dentro do sistema e tambem apoia a analise critica exigida no trabalho.

- id
- usuario_id
- tarefa_id
- tipo_interacao
- prompt
- resposta
- criada_em

Tipos de interacao previstos:

- `SugestaoTarefa`
- `Priorizacao`
- `Resumo`
- `QuebraEmSubtarefas`
- `ReescritaDescricao`

## 5. Funcionalidades Obrigatorias do MVP

### Funcionalidades implementadas

- Criacao de categorias
- Listagem de categorias
- Criacao de tarefas
- Listagem de tarefas
- Edicao de tarefas via API
- Exclusao de tarefas via API
- Marcacao de tarefa como concluida
- Definicao de prioridade
- Definicao de prazo
- Organizacao por categoria
- Filtro por status
- Filtro por prioridade
- Filtro por categoria via API
- Apoio de IA simulada para sugerir tarefas
- Apoio de IA simulada para priorizar tarefas
- Apoio de IA simulada para melhorar descricao
- Registro das interacoes com IA

## 6. Organizacao  do Repositorio

### Estrutura implementada

```text
mvp-gerenciador-tarefas-ia/
  README.md
  README_TESTES.md
  .env.example
  MvpGerenciadorTarefasIa.slnx
  backend/
    Models/
      Domain.cs
      Requests.cs
    Repositories/
      InMemoryTaskRepository.cs
    Services/
      TaskService.cs
      AiSupportService.cs
    wwwroot/
      index.html
      styles.css
      app.js
    Program.cs
    backend.csproj
  docs/
    testes-automatizados.md
  tests/
    Program.cs
    tests.csproj
```

## 7. Rotas da API Implementadas

### Sistema

| Metodo | Rota | Descricao |
| ------ | ---- | --------- |
| GET | `/health` | Verifica se a API esta ativa |

### Categorias

| Metodo | Rota | Descricao |
| ------ | ---- | --------- |
| GET | `/api/categories` | Lista categorias cadastradas |
| POST | `/api/categories` | Cria uma nova categoria |

Exemplo de criacao de categoria:

```json
{
  "nome": "Faculdade",
  "cor": "#2563eb"
}
```

### Tarefas

| Metodo | Rota | Descricao |
| ------ | ---- | --------- |
| GET | `/api/tasks` | Lista tarefas |
| GET | `/api/tasks?status=Concluida` | Filtra tarefas por status |
| GET | `/api/tasks?priority=Alta` | Filtra tarefas por prioridade |
| GET | `/api/tasks?categoryId={id}` | Filtra tarefas por categoria |
| POST | `/api/tasks` | Cria uma tarefa |
| PUT | `/api/tasks/{id}` | Atualiza uma tarefa |
| PATCH | `/api/tasks/{id}/complete` | Marca uma tarefa como concluida |
| DELETE | `/api/tasks/{id}` | Exclui uma tarefa |

Exemplo de criacao de tarefa:

```json
{
  "titulo": "Preparar apresentacao",
  "descricao": "Montar roteiro e validar demo",
  "categoriaId": null,
  "prioridade": "Alta",
  "dataLimite": "2026-06-20"
}
```

### IA

| Metodo | Rota | Descricao |
| ------ | ---- | --------- |
| POST | `/api/ai/suggest` | Sugere tarefas a partir de um objetivo |
| POST | `/api/ai/prioritize` | Retorna tarefas priorizadas |
| POST | `/api/ai/improve-description` | Melhora a descricao de uma tarefa |
| GET | `/api/ai/interactions` | Lista interacoes registradas com IA |

Exemplo de sugestao de tarefas:

```json
{
  "objetivo": "Finalizar o MVP da disciplina"
}
```

## 8. Cronograma de Desenvolvimento

| Etapa | Atividades | Prazo |
| ----- | ---------- | ----- |
| 1 | Planejamento, definicao das tecnologias e modelagem inicial | Aula atual |
| 2 | Criacao do repositorio, estrutura base do front-end e back-end | Concluido no MVP |
| 3 | Implementacao de autenticacao e CRUD de tarefas | CRUD de tarefas implementado; autenticacao fica para evolucao futura |
| 4 | Implementacao de categorias, filtros e prioridades | Concluido no MVP |
| 5 | Integracao com IA e registro das interacoes | IA simulada e registro implementados |
| 6 | Testes, ajustes de interface e documentacao | Testes e documentacao implementados |
| 7 | Finalizacao, deploy e preparacao da apresentacao | Deploy fica para evolucao futura |

## 9. Registro Inicial de Uso de IA

Durante o planejamento inicial, a IA foi utilizada como apoio para estruturar a arquitetura, conhecer tecnologias, organizar a modelagem das entidades e transformar os requisitos do trabalho em um plano de desenvolvimento.

Analise critica inicial: a IA ajudou a acelerar a organizacao das ideias e sugeriu uma estrutura consistente para o MVP. Entretanto, as decisoes foram revisadas por mim, considerando os requisitos reais definidos pela disciplina.

## 10. Como Executar o Projeto

Acesse a pasta do projeto:

```powershell
cd "Caminho onde se encontra o projeto"
```

Execute a API:

```powershell
dotnet run --project backend\backend.csproj --urls http://127.0.0.1:5280
```

Depois abra no navegador:

```text
http://127.0.0.1:5280
```

## 11. Como Executar os Testes

Na raiz do projeto, execute:

```powershell
dotnet run --project tests\tests.csproj
```

Resultado esperado:

```text
[PASSOU] Unitario: valida titulo obrigatorio na criacao de tarefa
[PASSOU] Integracao: cria categoria, cria tarefa, filtra e registra IA
[PASSOU] Sistema E2E: API real cria e conclui tarefa por HTTP

Resumo: 3 passou/passaram, 0 falhou/falharam.
```

Mais detalhes estao no arquivo `README_TESTES.md`.
