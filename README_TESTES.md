# README - Testes Automatizados

## 1. Funcionalidade escolhida

A funcionalidade escolhida para os testes foi o gerenciamento de tarefas do MVP. Essa funcionalidade envolve criacao de tarefas, validacao de dados, uso de categorias, definicao de prioridade, filtros, conclusao de tarefa e registro de interacoes com IA.

Essa funcionalidade foi escolhida porque representa o fluxo principal do sistema e envolve regras de negocio, armazenamento, rotas da API e comportamento esperado pelo usuario.

## 2. O que foi testado

Foram implementados tres tipos de teste, conforme solicitado na atividade:

- 1 teste unitario
- 1 teste de integracao
- 1 teste de sistema End-to-End (E2E)

### Teste unitario

Foi testada a regra que impede a criacao de uma tarefa sem titulo.

O sistema deve rejeitar uma tarefa vazia ou sem identificacao clara, pois o titulo e uma informacao obrigatoria para que o usuario consiga reconhecer e acompanhar suas atividades.

### Teste de integracao

Foi testado o funcionamento conjunto entre:

- repositorio em memoria;
- servico de tarefas;
- servico de IA simulada.

O teste cria uma categoria, cria uma tarefa com prioridade alta, consulta a tarefa usando filtros e chama o servico de IA para melhorar a descricao. Depois verifica se a interacao com IA foi registrada.

### Teste E2E

Foi testado o fluxo completo da aplicacao pela API real.

O teste inicia o back-end ASP.NET Core, espera a rota `/health` responder, cria uma categoria, cria uma tarefa, conclui essa tarefa e consulta a lista de tarefas concluidas por meio de requisicoes HTTP.

## 3. Como o teste foi implementado

Os testes foram implementados em um projeto separado chamado `tests`, usando C#.

O arquivo principal dos testes e:

```text
tests/Program.cs
```

Esse arquivo funciona como um runner simples de testes automatizados. Ele executa os cenarios, mostra no terminal quais testes passaram ou falharam e retorna erro caso algum teste falhe.

### Implementacao do teste unitario

O teste unitario instancia diretamente o `TaskService` e chama o metodo de criacao de tarefa com titulo vazio.

Resultado esperado: o sistema deve lancar uma excecao de validacao.

### Implementacao do teste de integracao

O teste de integracao instancia o `InMemoryTaskRepository`, o `TaskService` e o `AiSupportService`.

Depois executa o seguinte fluxo:

1. cria uma categoria;
2. cria uma tarefa associada a categoria;
3. consulta a tarefa usando filtros;
4. chama a IA simulada para melhorar a descricao;
5. verifica se a interacao de IA foi registrada.

### Implementacao do teste E2E

O teste E2E inicia a API real com `dotnet run`, usando uma porta local.

Depois usa `HttpClient` para chamar os endpoints:

- `GET /health`
- `POST /api/categories`
- `POST /api/tasks`
- `PATCH /api/tasks/{id}/complete`
- `GET /api/tasks?status=Concluida&priority=Alta`

Ao final, o processo da API e encerrado pelo proprio teste.

## 4. Por que o teste e importante para a qualidade do sistema

Os testes sao importantes porque aumentam a confianca de que o sistema esta funcionando corretamente e ajudam a evitar regressao durante futuras alteracoes.

O teste unitario garante que uma regra essencial de negocio esta protegida: nenhuma tarefa pode ser criada sem titulo.

O teste de integracao garante que as principais camadas do sistema funcionam em conjunto, validando o fluxo entre servicos, repositorio e IA simulada.

O teste E2E garante que o sistema funciona de ponta a ponta, como seria usado por um cliente real da API ou pelo front-end. Esse teste valida rotas, serializacao JSON, injecao de dependencias, regras de negocio e estado da aplicacao.

Com esses tres niveis de teste, o MVP fica mais confiavel, mais facil de manter e mais seguro para evolucoes futuras.

## 5. Qual ferramenta foi utilizada no teste E2E

No teste E2E foi utilizado `HttpClient`, em um runner automatizado escrito em C#.

O `HttpClient` envia requisicoes HTTP para a API ASP.NET Core real, permitindo validar o comportamento do sistema em execucao.

Tambem foi utilizado o proprio comando `dotnet run` para iniciar a API durante o teste.

## 6. Como executar os testes

Acesse a pasta do projeto implementado:

```powershell
cd caminho onde esta os testes
```

Execute:

```powershell
dotnet run --project tests\tests.csproj
```

## 7. Resultado obtido

```text
[PASSOU] Unitario: valida titulo obrigatorio na criacao de tarefa
[PASSOU] Integracao: cria categoria, cria tarefa, filtra e registra IA
[PASSOU] Sistema E2E: API real cria e conclui tarefa por HTTP

Resumo: 3 passou/passaram, 0 falhou/falharam.
```
