# Testes automatizados

Documento de entrega conforme o PDF da disciplina.

## Funcionalidade escolhida

Foi escolhida a funcionalidade de criacao e acompanhamento de tarefas, incluindo categoria, prioridade, filtros, conclusao e registro de apoio de IA.

## 1. Teste unitario

O que foi testado: a regra de negocio que impede a criacao de tarefas sem titulo.

Como foi implementado: o teste chama diretamente `TaskService.CreateTask` com titulo vazio e verifica se uma `ArgumentException` e lancada.

Por que e importante: uma tarefa sem titulo prejudica listagem, filtros, acompanhamento e entendimento do usuario. Essa validacao protege a qualidade minima dos dados antes de qualquer persistencia.

Ferramenta utilizada: runner automatizado em C# no projeto `tests`.

## 2. Teste de integracao

O que foi testado: a integracao entre repositorio em memoria, `TaskService` e `AiSupportService`.

Como foi implementado: o teste cria uma categoria, cria uma tarefa de prioridade alta, consulta usando filtros de status/prioridade/categoria e chama a melhoria de descricao com IA simulada. Ao final, verifica se a interacao com IA foi registrada.

Por que e importante: esse fluxo valida que as camadas principais cooperam corretamente e que a funcionalidade nao funciona apenas de forma isolada.

Ferramenta utilizada: runner automatizado em C# no projeto `tests`.

## 3. Teste de sistema End-to-End

O que foi testado: o fluxo completo pela API real: iniciar o sistema, criar categoria via HTTP, criar tarefa via HTTP, concluir tarefa via HTTP e consultar a tarefa concluida por filtro.

Como foi implementado: o runner inicia o projeto `backend` com `dotnet run`, espera o endpoint `/health`, executa chamadas HTTP com `HttpClient` e encerra o processo ao final.

Por que e importante: esse teste valida o comportamento do sistema em execucao, incluindo serializacao JSON, rotas HTTP, injecao de dependencias, servicos e estado da aplicacao.

Ferramenta utilizada no E2E: `HttpClient` em um runner C# automatizado, executando contra a API ASP.NET Core real.

## Como rodar

Na raiz do projeto:

```powershell
dotnet run --project tests\tests.csproj
```

Resultado obtido:

```text
[PASSOU] Unitario: valida titulo obrigatorio na criacao de tarefa
[PASSOU] Integracao: cria categoria, cria tarefa, filtra e registra IA
[PASSOU] Sistema E2E: API real cria e conclui tarefa por HTTP

Resumo: 3 passou/passaram, 0 falhou/falharam.
```
