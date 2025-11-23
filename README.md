# 🌐 WorkConnect – Comunidade Colaborativa de Aprendizagem (API .NET + PostgreSQL)

Esta solução implementa uma **API RESTful em .NET 8** para o tema:

> **WorkConnect – Comunidade Colaborativa de Aprendizagem entre Trabalhadores**

A API permite cadastrar usuários e dicas de aprendizagem, com:
- CRUD completo
- Paginação + HATEOAS no feed de dicas
- Versionamento de API (`/api/v1/...`)
- Integração com **PostgreSQL** usando **Entity Framework Core + Migrations**
- Health Check, Logging básico
- Testes de integração com **xUnit** e `WebApplicationFactory`

A solução dialoga com os ODS:
- **ODS 4 – Educação de Qualidade** (compartilhamento de conhecimento, dicas, trilhas)
- **ODS 10 – Redução das Desigualdades** (comunidade global de apoio e aprendizado entre trabalhadores)

---

## 🧱 Estrutura da Solução

```text
WorkConnect.sln
├─ src/
│  ├─ WorkConnect.Api
│  ├─ WorkConnect.Domain
│  └─ WorkConnect.Infrastructure
└─ tests/
   └─ WorkConnect.Tests
```

---

## 🛠️ Tecnologias Principais

- .NET 8 (ASP.NET Core Web API)
- PostgreSQL
- Entity Framework Core + Migrations
- API Versioning
- HealthChecks
- xUnit + WebApplicationFactory (testes de integração)

---

## ▶️ Como Rodar o Projeto

### 1. Pré-requisitos

- .NET 8 SDK instalado
- PostgreSQL rodando em `localhost:5432`
- Usuário padrão: `postgres`
- Senha padrão: `1234`

Você pode ajustar usuário/senha em:

```bash
src/WorkConnect.Api/appsettings.json
```

Trecho da connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Host=localhost;Port=5432;Database=WorkConnectDb;Username=postgres;Password=postgres"
}
```

---

### 2. Restaurar pacotes

Na pasta raiz onde está o `WorkConnect.sln`:

```bash
dotnet restore
```

---

### 3. Aplicar Migrations no PostgreSQL

> **Obs.:** A solução já contém uma Migration inicial (`InitialCreate`), dentro de  
> `src/WorkConnect.Infrastructure/Migrations`.

Você pode deixar que a própria API aplique as migrations na primeira execução (o código chama `Database.Migrate()` no startup).  
OU, se preferir, rodar manualmente:

```bash
cd src/WorkConnect.Api

# Caso ainda não tenha o dotnet-ef:
dotnet tool install --global dotnet-ef

# Aplicar migrations no banco
dotnet ef database update -p ../WorkConnect.Infrastructure -s .
```

---

### 4. Executar a API

Ainda em:

```bash
cd src/WorkConnect.Api
dotnet run
```

Endpoints principais:

- Swagger UI: `https://localhost:5001/swagger` (ou porta apresentada no console)
- Health Check: `https://localhost:5001/health`
- API base (v1):
  - `https://localhost:5001/api/v1/users`
  - `https://localhost:5001/api/v1/tips`

---

### 5. Testar a API

Na raiz do projeto:

```bash
dotnet test
```

Os testes usam um banco **InMemory** para não mexer no seu PostgreSQL.

---

## 📚 Versionamento da API

A API utiliza **API Versioning** com o padrão de rota:

```http
/api/v{version}/[controller]
```

Hoje existe a versão:

- `v1` – estável (Users, Tips)

Exemplos:

- `GET /api/v1/users`
- `GET /api/v1/tips?pageNumber=1&pageSize=10`

O versionamento é configurado em `Program.cs`:

- Versão padrão: **1.0**
- Header de resposta inclui as versões suportadas.

---

## 📄 Paginação + HATEOAS no Feed de Dicas

Endpoint:

```http
GET /api/v1/tips?pageNumber=1&pageSize=10
```

Resposta exemplo (resumida):

```json
{
  "items": [
    {
      "id": 1,
      "title": "Dica de estudo focado",
      "content": "Use a técnica Pomodoro...",
      "authorName": "João",
      "links": [
        { "rel": "self", "href": "/api/v1/tips/1", "method": "GET" },
        { "rel": "update", "href": "/api/v1/tips/1", "method": "PUT" },
        { "rel": "delete", "href": "/api/v1/tips/1", "method": "DELETE" },
        { "rel": "author", "href": "/api/v1/users/1", "method": "GET" }
      ]
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 1,
  "links": [
    { "rel": "self", "href": "/api/v1/tips?pageNumber=1&pageSize=10", "method": "GET" },
    { "rel": "nextPage", "href": "/api/v1/tips?pageNumber=2&pageSize=10", "method": "GET" }
  ]
}
```

---

## ✅ Itens do Enunciado Atendidos

1. **Boas Práticas REST**
   - Verbos HTTP corretos
   - Status codes bem definidos (200, 201, 204, 400, 404)
   - Paginação e HATEOAS no endpoint de dicas

2. **Monitoramento e Observabilidade**
   - Health Check em `/health`
   - Logging básico com `UseHttpLogging()`

3. **Versionamento**
   - Rotas em `/api/v1/...`
   - Configuração com `AddApiVersioning` explicada neste README

4. **Integração & Persistência**
   - PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`
   - `WorkConnectContext` com `DbSet<User>` e `DbSet<Tip>`
   - Migrations incluídas

5. **Testes Integrados**
   - Projeto `WorkConnect.Tests` com xUnit
   - Testes de integração básicos para Users e Tips usando `WebApplicationFactory`

---

## 📌 Observação Final

Se a sua instância de PostgreSQL tiver usuário/senha diferentes de `postgres/postgres`,  
basta ajustar a connection string em `appsettings.json`.

Bom estudo e boa sprint! 🚀
