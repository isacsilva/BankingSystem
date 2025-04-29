# BankingSystem

MVP de um sistema bancário capaz de gerir contas e transações.

## Sumário

1. Arquitetura.
2. Tecnologias utilizadas.
3. Execução.
4. Dificuldades, aprendizados e observações.

---

## 1. Arquitetura.

### 1.1 Backend

Decidi fazer meu projeto em .NET Core, por que eu estou familiarizado e tenho algum conhecimento sobre a arquitetura que ele usa. Estruturei minha solução em vários projetos separados, cada um com sua responsabilidade, seguindo os princípios da Clean Architecture.
O primeiro projeto criado foi o `Core`, responsável por manter as entidades, interfaces e DTOs(se necessários).

```
Core/
├── Service/
│ ├── IBankAccountService.cs
│ ├── ITransactionService.cs
├── Balance.cs
├── Bankaccount.cs
├── BankingDbContext.cs
├── Transaction.cs
```

O Core mantém as regras de negócio. Declarando os métodos nas interfaces para que eles possam ser implementados por qualquer um que implements a interface.
Exemplo:

```
public interface IBankAccountService {
Task<Bankaccount> CreateAccount(Bankaccount account);
{
```

O segundo projeto criado foi o `Service`, responsável por implementar as interfaces do Core, então para cada “IArquivoService” no Core, existe um “ArquivoService” no Service.

```
Service/
├── BankAccountService.cs
├── TransactionService.cs
```

O Service implementa as interfaces do Core e seus métodos.
Exemplo:

```
public class BankAccountService : IBankAccountService {
public async Task<Bankaccount> CreateAccount(Bankaccount account){
//Implementação da logica
}
```

Em seguida, resolvi seguir boas praticas e criar um projeto chamado `ServiceTest`.
Ele serve especificamente para validar se os métodos do Service (ou da lógica de negócio) funcionam corretamente, utilizando um padrão de testes unitários com MSTest e uma base de dados em memória (InMemoryDatabase) para simular os dados e rodar os métodos utilizando-os, simulando uma execução real dos métodos.

```
ServiceTests/
├── BankAccountServiceTests.cs
├── TransactionServiceTests.cs
```

E por último (backend) e não menos importante, foi criado o `BankingAPI`.

```
BankingAPI/
├── Connected Services/
├── Dependências/
├── Properties/
├── Controllers/
│ ├── AuthController.cs
│ ├── BankAccountController.cs
│ ├── TransactionController.cs
├── DTOs/
│ ├── TransactionAmountDto.cs
│ ├── TransferRequestDto.cs
├── appsettings.json
├── Program.cs
```

Nela foi criado rotas de acesso (GET, POST e PUT) para a API em Controllers, cada rota chama o método do “ArquivoService” relacionado a ela e executa uma operação. Os DTOs foram utilizados quando ouve a necessidade de encapsular dados sensíveis, como dados envolvendo movimentações bancarias.

```
[Authorize]
[HttpPost]
public async Task<IActionResult> Create([FromBody] Bankaccount account) {
var created = await \_service.CreateAccount(account);
return CreatedAtAction(nameof(GetByNumber), new { number = created.Number }, created);
}
```

Enfim, com a implementação da API, a arquitetura do backend foi completa.
BankingSystem (Solução)/

```
├── BankingAPI/
│ ├── Connected Services/
│ ├── Dependências/
│ ├── Properties/
│ ├── Controllers/
│ │ ├── AuthController.cs
│ │ ├── BankAccountController.cs
│ │ ├── TransactionController.cs
│ ├── DTOs/
│ │ ├── TransactionAmountDto.cs
│ │ ├── TransferRequestDto.cs
│ ├── appsettings.json
│ ├── Program.cs
│
├── Core/
│ ├── Dependências/
│ ├── Service/
│ │ ├── IBankAccountService.cs
│ │ ├── ITransactionService.cs
│ ├── Balance.cs
│ ├── Bankaccount.cs
│ ├── BankingDbContext.cs
│ ├── Transaction.cs
│
├── Service/
│ ├── Dependências/
│ ├── BankAccountService.cs
│ ├── TransactionService.cs
```

### 1.2 Frontend

Como o Vue.js foi a base de todo o Frontend, a arquitetura foi baseada em componentes que podem ser reutilizados em diferentes partes da aplicação.
Foi criado uma pasta no denominada pages no src, onde ela armazenava as páginas independentes. E outra pasta denominada componentes no mesmo diretório que armazenava os componentes que podem e foram utilizados por outras páginas.

```
/src
├── components
│ ├── BlockAmountModal.vue
│ ├── TransactionDetailsModal.vue
│ ├── TransferModal.vue
├── pages
│ ├── Backoffice.vue
│ ├── CreateAccount.vue
│ ├── Dashboard.vue
│ ├── Home.vue
│ ├── Login.vue
│ ├── LoginDashboard.vue
├── router
│ ├── index.js
```

Deixando o código mais organizado e fácil de entender.
As rotas ficaram organizadas no arquivo `src/router/index.js`, que mapeia os caminhos para cada página.
Essa arquitetura é simples e direta, facilitando a organização e a reutilização do código. O que é ótimo para quem ainda está aprendendo o seu uso.

---

## 2. Tecnologias utilizadas

### 2.1 Backend

- **.NET (C#)**: Utilizado para o desenvolvimento do Backend, incluindo a implementação da API e a gestão de processos relacionados ao sistema bancário.
- **Entity Framework Core**: Utilizado para o gerenciamento do banco de dados e realização de operações CRUD em relação às tabelas, como BankAccount, Balance, e Transaction.
- **Pomelo.EntityFrameworkCore.MySql**: Fornece a integração entre o Entity Framework Core e o MySQL, permitindo que o EF Core realize operações no banco de dados MySQL.
- **MySQL**: Sistema de gerenciamento de banco de dados relacional utilizado para armazenar dados como contas bancárias, transações e saldos.
- **Swagger**: Usado para documentar a API RESTful, facilitando os testes e a visualização dos endpoints da aplicação.
- **Postman**: Ferramenta de testes para API, utilizada para enviar requisições e verificar as respostas da aplicação, ajudando na validação das funcionalidades implementadas.

### 2.2 Frontend

- **Vue.js 3**: Usado como o framework principal para criar toda a estrutura do frontend. Ele gerencia a criação de componentes, a manipulação do estado das telas e a renderização dinâmica dos dados.
- **Quasar Framework**: Usado para acelerar o desenvolvimento da interface com componentes prontos e responsivos, como tabelas, cards, botões, inputs e modais. Deixou o visual mais bonito e o desenvolvimento mais rápido.
- **Vue Router**: Foi usado para gerenciar as rotas da aplicação. Permite navegar entre páginas como Home, Login, LoginDashboard, Backoffice, Dashboard, etc., sem recarregar o site.
- **Axios**: Utilizado para fazer as requisições HTTP entre o frontend e a API (backend). Com ele conseguimos enviar e buscar dados como login, contas, transações, saldos e transferências.

---

## 3. Execução

### Clone o repositório para sua máquina:

```bash
git clone https://github.com/isacsilva/BankingSystem
```

### Configure o banco de dados

1. Verifique se o MySQL está rodando na sua máquina.

2. Crie o banco de dados:

- Abra a pasta `AnaliseProjeto` no repositorio clonado.

- Copie os comandos SQL para a criação do banco do arquivo `Banco de Dados.txt`

- execute no seu gerenciador de banco de dados (ex: MySQL Workbench)

3. Carregar o banco de dados com informações.

- Abra a pasta `AnaliseProjeto`,o repositorio clonado.
- Copie o código SQL pronto (com inserts para todas as tabelas) do arquivo `DUMP do Banco de Dados.txt` e execute no seu banco de dados.
- execute no seu gerenciador de banco de dados (ex: MySQL Workbench)

5. Configure a string de conexão:

- Abra o arquivo `BankingSystemAPI/appsettings.json`

- Ajuste a `ConnectionStrings:DefaultConnection` se necessário (usuário, senha, nome do banco, etc.)

Exemplo de appsettings.json:

```bash
"ConnectionStrings": {
  "DefaultConnection": "server=localhost;port=3306;user=root;password=123456;database=BankingDB"
}
```

### Backend

#### Rode o servidor pelo Visual Studio - Forma mais facil

1. Garante aque o Visutal Studio esteja instalado em sua maquina.

2. Abra a pasta do repositorio clonado e em seguida a do backend `BankingSystem`, e abra o projeto clicando no arquivo `BankingSystem.sln`

3. Quando o projeto abrir, execute a solução em https

#### Rode o servidor pelo prompt

2. Acesse a pasta do repositorio clonado via prompt

3. Vá até a pasta `BankingSystem`:

```bash
cd BankingSystem
```

6. Rode o servidor:

```bash
set ASPNETCORE_ENVIRONMENT=Development && dotnet run --project BankingSystemAPI/BankingAPI.csproj
```

7. O servidor será rodado em http, então para não dar o erro **ERR_SSL_PROTOCOL_ERROR** ao utilizar o frontend você terá que alterar o acesso a URL de `https` para `http` no arquivo `banking-web/src/services/https-common.js`. Caso você utilize o Visual Studio para executar o backend esse problema não existe.

### Frontend

1. Acesse a pasta do repositorio clonado via prompt

2. Vá até a pasta do projeto Vue.js `banking-web`:

```bash
cd banking-web
```

3. Instale as dependências:

```bash
npm install
```

4. Rode o servidor de desenvolvimento:

```bash
npm run serve
```

5. Acesse a aplicação em `http://localhost:***`. (A porta é exibida no terminal)

---

## 4. Dificuldades, aprendizados e observações

### Dificuldades e Aprendizados

#### Não conhecia o conceito de Backoffice e Dashboard antes de começar a responder essa prova.

- **Problema:**
  Problema: Não conhecia o conceito de Backoffice e Dashboard antes de começar a responder essa prova, pois infelizmente nunca me envolvi com temas relacionados. E isso me levou a cometer alguns erros durante a fase de criação dos protótipos das páginas. Erros que foram apontados por um amigo que avaliou meus protótipos.
- **Solução:**
  Descartei os protótipos incorretos, estudei o conceito de Backoffice e Dashboard mais profundamente, escutei os conselhos do meu amigo e refiz as páginas agora mais adequadas.
- **Resultado** Páginas mais intuitivas e mais adequadas para o Backoffice e Dashboard.
  Reforçou a importância da fase da prototipagem para mim, pois sem passar por esse processo de validação de protótipo, poderia ter resultado em um desastre. É uma boa pratica que sempre gostei de seguir e valorizo mais agora depois dessa experiencia.

#### Segurança das rotas da API

- **Problema**: Após a criação e validação das rotas da API, concluí o básico de uma API. Aí surgiu o problema com a segurança de acesso das rotas, pois elas estavam completamente desprotegidas.
  Essa noção não tinha vindo a mim antes quando criei a API REST utilizando o Node.js, pois era um projeto pequeno e voltado ao aprendizado, diferente do projeto atual, que é um MVP de um sistema bancário, responsável por gerenciar transações entre contas bancarias, transações essas que em um sistema bancário completo atual (como o Nubank) não podem ser deixadas desprotegidas de forma alguma, então mesmo que o MVP em questão não alcance o patamar de um sistema bancário completo, por princípio e logica, eu teria que proteger essas rotas onde os dados então à mercê. E também eu acredito que, é um fato que qualquer informação pessoal ou sensíveis devem ser protegidas. E outra, uma API não pode ficar desprotegida. Porque todas as APIs das grades empresas sempre solicitam uma chave de acesso?
- **Solução**: Implementei a autenticação com JWT (JSON Web Token), protegendo as rotas da API com token de acesso e permitindo controle seguro de acesso as rotas.
- **Resultados**: Rotas protegidas. Apenas sendo acessadas através de uma chave de acesso retornada para a aplicação após uma autentificação de login.

#### Segurança das rotas da API Frontend no ASP.NET Core ou em Node.js

- **Problema**: Quando concluir a API e cheguei na fase de desenvolver o frontend me deparei com um dilema. Seguir o caminho mais fácil e utilizar Razor Pages do ASP.NET Core, que eu estou mais familiarizado e confortável em usar ou utilizar o Vue.js, que sou iniciante, mas será uma boa oportunidade de aprendizado?
- **Solução**: Recebi um conselho de um amigo “O importante nessa prova que você está fazendo é o aprendizado que você está tendo”.
- **Resultados**: Não me arrependo de ter escolhido desenvolver o Frontend em Vue.js. O desafio valeu a pena. Eu aprendi bastante coisa e não vou mais parar de usar ele de agora em diante.

#### Fui um pouco míope durante a análise de requisitos

- **Problema**: Durante o processo de desenvolvimento do frontend, percebi que as rotas de filtro não foram implementadas corretamente. No caso do **api/bankaccount/filter**, ela não foi implementada, pois nos requisitos funcionais não dizia explicitamente `“O sistema deve permitir a busca das contas bancarias aplicando um filtro pelo número da conta, agência e documento do titular”`, mas estava dizendo lá na descrição das funcionalidades da tela de BackOffice do Front. O problema surgiu quando eu me concentrei demais nos requisitos e na regra de negócio do Backend enquanto estava desenvolvendo o Backend e coloquei o Frontend em segundo plano, desejando “fazer as coisas por partes”. Isso me impediu de olhar o escopo completo do projeto, acarretando nesse erro de analise.
  Já no caso da **api/transasion/filter**, ele existia, mas possuía outro nome **“api/transasion/by-acoontId”**, esse filtro representava o requisito `“RF18: O sistema deve permitir buscar todas as transações de uma conta bancária, ordenadas da mais recente para a mais antiga, filtrando pele período e/ou pelo tipo da transação.”`, mas esse filtro estava incompleto, faltava dois valores na filtragem, como solicitado no requisito do frontend `“DSH03: Consultar as transações dessa conta bancária com os filtros: Id, Período, Documento da contraparte, Tipo da transação”`. Eu deveria ter notado essa inconsistência durante a fase de analise de requisitos, mas novamente, eu foquei nos requisitos do backend, deixando os requisitos do front para depois, levando a esse problema.
- **Solução**: Parei brevemente o desenvolvimento do front para corrigir esses erros de leitura de requisito. A rota do **api/bankaccount/filter** foi implementada junto com seus métodos necessários e o api/transasion/by-acoontId foi totalmente reestruturada, corrigido e renomeada para **api/transasion/filter**.
- **Resultados**: Todos os filtros implementados corretamente e operantes.
  Aprendi que quando você estiver analisando os requisitos, você não deve limitar seu olhar para uma parte do projeto, você deve olhar para o todo, para entender o escopo completo da aplicação. Com essa experiencia, esse acontecimento míope não irá se repetir.

---

## 4. Observações

- **Erro CORS policy**: Erro caudado devido ao frontend tentando fazer chamadas ao backend hospedado em outro domínio. Mas foi simples de resolver. Apenas colocando um pequeno trecho de código no Program.js.
