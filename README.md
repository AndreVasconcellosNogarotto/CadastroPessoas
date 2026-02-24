# Cadastro de Pessoas — Banco PAN | Case Técnico Sr. .NET

API REST + Frontend para cadastro de **Pessoas Físicas** e **Pessoas Jurídicas** com integração ViaCEP.

---

## Tecnologias

### Backend
| Camada | Tecnologia |
|---|---|
| Framework | .NET 8 / ASP.NET Core |
| ORM | Entity Framework Core 8 + SQL Server |
| Arquitetura | **Hexagonal (Ports & Adapters)** |
| Testes | xUnit + Moq + FluentAssertions |
| Docs | Swagger / OpenAPI |
| Container | Docker + Docker Compose |

### Frontend
| Item | Tecnologia |
|---|---|
| Framework | Angular 18 (Standalone Components) |
| UI | PrimeNG 17 + PrimeFlex |
| Testes | Karma + Jasmine |

---

## Arquitetura Hexagonal

```
src/
├── CadastroPessoas.Domain/          ← Núcleo da aplicação
│   ├── Entities/                    ← Entidades de domínio com regras de negócio
│   │   ├── Pessoa.cs               ← Classe base abstrata
│   │   ├── PessoaFisica.cs         ← Validação de CPF no domínio
│   │   ├── PessoaJuridica.cs       ← Validação de CNPJ no domínio
│   │   └── Endereco.cs             ← Validação de CEP/UF no domínio
│   ├── Ports/
│   │   ├── Inbound/                ← Primary Ports (interfaces de casos de uso)
│   │   └── Outbound/               ← Secondary Ports (interfaces de repos e serviços ext.)
│   └── Exceptions/                 ← Exceções de domínio tipadas
│
├── CadastroPessoas.Application/     ← Casos de uso (orquestração)
│   ├── UseCases/
│   │   ├── PessoaFisica/           ← PessoaFisicaUseCase
│   │   ├── PessoaJuridica/         ← PessoaJuridicaUseCase
│   │   └── Endereco/               ← EnderecoUseCase (consulta ViaCEP)
│   └── DTOs/
│       ├── Request/                ← Contratos de entrada
│       └── Response/               ← Contratos de saída
│
├── CadastroPessoas.Infrastructure/  ← Adaptadores de saída (Secondary Adapters)
│   ├── Persistence/EF/             ← Repositórios EF Core + SQL Server
│   │   ├── Context/AppDbContext
│   │   ├── Mappings/               ← Fluent API mapeamentos
│   │   └── Repositories/           ← Implementações dos Secondary Ports
│   └── ExternalServices/ViaCep/    ← Adaptador do serviço externo ViaCEP
│
└── CadastroPessoas.API/             ← Adaptadores de entrada (Primary Adapters)
    ├── Controllers/                 ← PessoaFisicaController, PessoaJuridicaController, EnderecoController
    ├── Middlewares/                 ← Tratamento global de exceções
    └── Extensions/                 ← Configuração de DI
```

### Fluxo Hexagonal
```
[HTTP Request]
    ↓
[Controller — Primary Adapter]
    ↓
[IUseCase Port — Primary Port]
    ↓
[UseCase — Application Core]
    ↓
[IRepository Port — Secondary Port] → [EF Core Repository — Secondary Adapter] → [SQL Server]
[IViaCepService Port — Secondary Port] → [ViaCepAdapter — Secondary Adapter] → [ViaCEP API]
```

---

## Endpoints

### Pessoa Física
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/pessoas-fisicas` | Lista com paginação |
| `GET` | `/api/v1/pessoas-fisicas/{id}` | Obtém por ID |
| `POST` | `/api/v1/pessoas-fisicas` | Cria |
| `PUT` | `/api/v1/pessoas-fisicas/{id}` | Atualiza |
| `DELETE` | `/api/v1/pessoas-fisicas/{id}` | Remove |

### Pessoa Jurídica
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/pessoas-juridicas` | Lista com paginação |
| `GET` | `/api/v1/pessoas-juridicas/{id}` | Obtém por ID |
| `POST` | `/api/v1/pessoas-juridicas` | Cria |
| `PUT` | `/api/v1/pessoas-juridicas/{id}` | Atualiza |
| `DELETE` | `/api/v1/pessoas-juridicas/{id}` | Remove |

### Endereço (ViaCEP)
| Método | Rota | Descrição |
|---|---|---|
| `GET` | `/api/v1/enderecos/cep/{cep}` | Consulta endereço pelo CEP |

---

## Como Executar

### Com Docker Compose (recomendado)
```bash
# Na raiz do projeto
docker-compose up --build
```
- API: http://localhost:5000 (Swagger na raiz)
- Frontend: http://localhost:4200

### Sem Docker

#### Backend
```bash
cd backend
# Garanta que o SQL Server está rodando
dotnet restore
dotnet ef database update --project src/CadastroPessoas.Infrastructure --startup-project src/CadastroPessoas.API
dotnet run --project src/CadastroPessoas.API
```

#### Frontend
```bash
cd frontend
npm install
npm start
```

### Rodar Testes
```bash
# Backend — Unit Tests
cd backend
dotnet test --collect:"XPlat Code Coverage"

# Frontend
cd frontend
npm run test:coverage
```

---

## Design Patterns Utilizados

- **Repository Pattern** — abstração do acesso a dados via interfaces (Secondary Ports)
- **Use Case / Service Layer** — encapsulamento da lógica de negócio por caso de uso
- **Adapter Pattern** — ViaCepAdapter implementa IViaCepService sem acoplar o domínio à HTTP
- **Factory Method** (implícito) — construtores das entidades como únicos pontos de criação válida
- **Middleware Pipeline** — tratamento centralizado de exceções (ExceptionHandlingMiddleware)

### O que faria diferente em produção
- Adicionar **CQRS com MediatR** para separar leituras de escritas
- Implementar **MongoDB** como segunda fonte de dados (NoSQL) para consultas analíticas
- Adicionar **Redis** para cache de consultas ViaCEP
- Implementar **AWS Cognito** para autenticação e autorização
- Usar **AWS SES** para notificações por e-mail no cadastro

---

## AWS — Cenário Técnico

Para um ambiente produtivo, este projeto utilizaria:

| Serviço | Uso |
|---|---|
| **ECS Fargate** | Hospedagem dos containers da API e Frontend |
| **RDS SQL Server** | Banco de dados gerenciado |
| **Cognito** | Autenticação JWT + gerenciamento de usuários |
| **S3** | Armazenamento de arquivos e documentos |
| **ElastiCache (Redis)** | Cache de CEPs consultados |
| **CloudWatch** | Logs, métricas e alertas |
| **API Gateway** | Rate limiting e roteamento |
