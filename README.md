# Planejamento Inicial - Trabalho Pratico

## MVP: Gerenciador de Tarefas com Apoio de IA

### 1. Integrante(s)

- Nome: Fabio Guimarães
- Desenvolvimento: individual

### 2. Tecnologias Escolhidas

Para o desenvolvimento do MVP, foram escolhidas tecnologias simples, produtivas e adequadas para a criacao rapida de uma aplicacao funcional.

- Front-end: React
- Back-end: C# com ASP.NET
- Banco de dados: PostgreSQL
- IA de apoio ao desenvolvimento: ChatGPT / Codex
- IA dentro da aplicacao: API da OpenAI, para sugestao de tarefas, organizacao de prioridades e geracao de resumos
- Versionamento: Git e GitHub

### 3. Arquitetura Proposta

A aplicacao sera organizada em uma arquitetura cliente-servidor, separando a interface do usuario, a API e a camada de dados.

O front-end sera responsavel pela experiencia do usuario, exibicao das tarefas, filtros, formularios e interacoes com os recursos de IA. Ele se comunicara com o back-end por meio de requisicoes HTTP/REST.

O back-end sera responsavel pelas regras de negocio, autenticacao, validacao dos dados, integracao com o banco de dados e chamadas para a API de IA. A API tambem centralizara operacoes como criacao, edicao, conclusao, exclusao e consulta de tarefas.

O banco de dados armazenara usuarios, tarefas, categorias, historico de interacoes com IA e metadados importantes para acompanhamento do uso da aplicacao.

Fluxo geral:

1. Usuario acessa o sistema pelo front-end.
2. Front-end envia requisicoes para a API.
3. API valida os dados e executa regras de negocio.
4. API consulta ou atualiza o banco de dados.
5. Quando necessario, a API envia uma solicitacao para o servico de IA.
6. A resposta e retornada ao front-end e exibida ao usuario.

### 4. Modelagem Inicial das Entidades

#### Usuario

Representa a pessoa que utiliza o sistema.

- id
- nome
- email
- senha_hash
- data_criacao
- data_atualizacao

#### Tarefa

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

- pendente
- em_andamento
- concluida
- cancelada

Prioridades previstas:

- baixa
- media
- alta

#### Categoria

Permite organizar tarefas por area ou contexto.

- id
- usuario_id
- nome
- cor
- criada_em

#### InteracaoIA

Registra o uso da IA dentro do sistema e tambem apoia a analise critica exigida no trabalho.

- id
- usuario_id
- tarefa_id
- tipo_interacao
- prompt
- resposta
- criada_em

Tipos de interacao previstos:

- sugestao_tarefa
- priorizacao
- resumo
- quebra_em_subtarefas
- reescrita_descricao

### 5. Funcionalidades Obrigatorias do MVP

O MVP devera contemplar as seguintes funcionalidades principais:

- Criacao de tarefas
- Listagem de tarefas
- Edicao de tarefas
- Exclusao de tarefas
- Marcacao de tarefa como concluida
- Definicao de prioridade
- Definicao de prazo
- Organizacao por categoria
- Filtro por status, prioridade e categoria
- Apoio de IA para sugerir tarefas a partir de um objetivo informado pelo usuario
- Apoio de IA para priorizar tarefas
- Apoio de IA para resumir ou melhorar a descricao de uma tarefa
- Registro das interacoes com IA para posterior analise critica

### 6. Organizacao Inicial do Repositorio

Estrutura planejada:

```text
mvp-gerenciador-tarefas-ia/
  README.md
  docs/
    planejamento.md
    registro-interacoes-ia.md
    analise-critica-ia.md
  frontend/
    src/
      components/
      pages/
      services/
      styles/
  backend/
    Controllers/
    Models/
    Services/
    Repositories/
    Program.cs
    appsettings.json
  .env.example
```

### 7. Cronograma de Desenvolvimento

| Etapa | Atividades                                                     | Prazo estimado |
| ----- | -------------------------------------------------------------- | -------------- |
| 1     | Planejamento, definicao das tecnologias e modelagem inicial    | Aula atual     |
| 2     | Criacao do repositorio, estrutura base do front-end e back-end |
| 3     | Implementacao de autenticacao e CRUD de tarefas                |
| 4     | Implementacao de categorias, filtros e prioridades             |
| 5     | Integracao com IA e registro das interacoes                    |
| 6     | Testes, ajustes de interface e documentacao                    |
| 7     | Finalizacao, deploy e preparacao da apresentacao               |

### 8. Registro Inicial de Uso de IA

Durante o planejamento inicial, a IA foi utilizada como apoio para estruturar a arquitetura, sugerir tecnologias, organizar a modelagem das entidades e transformar os requisitos do trabalho em um plano de desenvolvimento.

Analise critica inicial: a IA ajudou a acelerar a organizacao das ideias e sugeriu uma estrutura coerente para o MVP. Entretanto, as decisoes foram revisadas por mim, considerando os requisitos reais definidos pela disciplina.
