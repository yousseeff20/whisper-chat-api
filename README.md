# 💬 Whisper Chat API

A real-time private messaging API built with **ASP.NET Core 9**, **SignalR**, and **Clean Architecture**. Whisper provides secure one-on-one chat functionality with JWT authentication, file sharing via Supabase Storage, and a CQRS pattern powered by MediatR.

---

## ✨ Features

- **Real-time messaging** via SignalR WebSocket hub
- **JWT authentication** with access & refresh token flow
- **Private conversations** with one-on-one chat support
- **File uploads** (images, documents, audio, video) via Supabase Storage
- **Message types** — text, image, document, audio, and video
- **User presence** — online status and last seen tracking
- **Rate limiting** — global request throttling
- **Security headers** — XSS, clickjacking, and MIME-sniffing protection
- **Swagger UI** — interactive API documentation in development
- **Database seeding** — predefined test users for quick setup

---

## 🏗️ Architecture

The project follows **Clean Architecture** with **CQRS** (Command Query Responsibility Segregation) using MediatR.

```
┌──────────────────────────────────────────────────┐
│                  Whisper.Api                      │
│         (Controllers, Hubs, Middleware)           │
├──────────────────────────────────────────────────┤
│              Whisper.Application                  │
│       (Commands, Queries, Validators)             │
├──────────────────────────────────────────────────┤
│              Whisper.Infrastructure               │
│    (EF Core, JWT, Supabase, Persistence)          │
├──────────────────────────────────────────────────┤
│                Whisper.Domain                     │
│          (Entities, Enums, Common)                │
└──────────────────────────────────────────────────┘
```

**Dependency flow:** Api → Application → Domain ← Infrastructure

---

## 🛠️ Tech Stack

| Layer           | Technology                                      |
| --------------- | ----------------------------------------------- |
| Framework       | ASP.NET Core 9                                  |
| Language        | C# 13                                           |
| Database        | PostgreSQL (via Supabase)                        |
| ORM             | Entity Framework Core 9                          |
| Authentication  | JWT Bearer Tokens                                |
| Identity        | ASP.NET Core Identity                            |
| Real-time       | SignalR                                          |
| CQRS/Mediator   | MediatR                                          |
| Validation      | FluentValidation                                 |
| File Storage    | Supabase Storage                                 |
| API Docs        | Swashbuckle (Swagger UI)                         |
| Logging         | Serilog                                          |
| Testing         | xUnit, Moq                                       |

---

## 📁 Folder Structure

```
Whisper.sln
├── Whisper.Api/                        # Presentation layer
│   ├── Controllers/
│   │   ├── AuthController.cs           # Login endpoint
│   │   ├── ConversationsController.cs  # Conversation CRUD
│   │   ├── MessagesController.cs       # Message send/query
│   │   └── UploadsController.cs        # File upload endpoint
│   ├── Hubs/
│   │   └── ChatHub.cs                  # SignalR real-time hub
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs      # Global error handling
│   ├── Services/
│   │   └── RealtimeNotifier.cs         # SignalR notification service
│   ├── Program.cs                      # App entry point & config
│   └── appsettings.json                # Configuration (placeholders)
│
├── Whisper.Application/                # Business logic layer
│   ├── Common/
│   │   └── Interfaces/                 # Abstractions (IApplicationDbContext, etc.)
│   ├── Features/
│   │   ├── Auth/Commands/              # Login command + handler
│   │   ├── Conversations/              # Conversation commands & queries
│   │   └── Messages/                   # Message commands & queries
│   └── DependencyInjection.cs
│
├── Whisper.Domain/                     # Core domain layer
│   ├── Common/
│   │   ├── Error.cs                    # Domain error type
│   │   └── Result.cs                   # Result pattern
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Conversation.cs
│   │   ├── ConversationParticipant.cs
│   │   └── Message.cs
│   └── Enums/
│       └── MessageType.cs
│
├── Whisper.Infrastructure/             # Data & external services
│   ├── Authentication/
│   │   ├── JwtSettings.cs
│   │   └── JwtTokenGenerator.cs
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   └── DbInitializer.cs
│   ├── Migrations/                     # EF Core migrations
│   ├── Storage/
│   │   └── StorageService.cs           # Supabase file storage
│   └── DependencyInjection.cs
│
└── Whisper.Tests/                      # Unit tests
    └── SendMessageCommandHandlerTests.cs
```

---

## 🚀 Installation

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [PostgreSQL](https://www.postgresql.org/) (or a [Supabase](https://supabase.com/) project)
- Git

### Clone the Repository

```bash
git clone https://github.com/yousseeff20/whisper-chat-api.git
cd whisper-chat-api
```

### Restore Dependencies

```bash
dotnet restore
```

---

## 🔐 Environment Variables

The application reads configuration from `appsettings.json`. For local development, create an `appsettings.Development.json` (git-ignored) with your real values:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-host.pooler.supabase.com;Port=5432;Database=postgres;Username=your-username;Password=your-real-password;SSL Mode=Require;Trust Server Certificate=true"
  },
  "JwtSettings": {
    "Secret": "your-secret-key-at-least-32-characters-long"
  },
  "Supabase": {
    "Url": "https://your-project-ref.supabase.co",
    "Key": "your-supabase-service-role-key"
  },
  "PredefinedUsers": {
    "User1": {
      "Username": "user1",
      "Password": "YourPassword1"
    },
    "User2": {
      "Username": "user2",
      "Password": "YourPassword2"
    }
  }
}
```

| Variable                              | Description                              |
| ------------------------------------- | ---------------------------------------- |
| `ConnectionStrings:DefaultConnection` | PostgreSQL connection string             |
| `JwtSettings:Secret`                  | JWT signing key (≥ 32 characters)        |
| `JwtSettings:Issuer`                  | Token issuer (`WhisperApi`)              |
| `JwtSettings:Audience`                | Token audience (`WhisperClient`)         |
| `Supabase:Url`                        | Supabase project URL                     |
| `Supabase:Key`                        | Supabase service role key                |
| `Supabase:BucketName`                 | Storage bucket name (`whisper-bucket`)   |
| `PredefinedUsers:User1:Password`      | Seed user 1 password                     |
| `PredefinedUsers:User2:Password`      | Seed user 2 password                     |

---

## ▶️ Running the API

```bash
cd Whisper.Api
dotnet run
```

The API will start at:
- **HTTPS:** `https://localhost:7247`
- **HTTP:** `http://localhost:5108`

---

## 🗄️ Database Migration

Migrations run automatically on startup via `DbInitializer`. To manually manage migrations:

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project Whisper.Infrastructure --startup-project Whisper.Api

# Update the database
dotnet ef database update --project Whisper.Infrastructure --startup-project Whisper.Api
```

---

## ☁️ Supabase Configuration

1. Create a [Supabase](https://supabase.com/) project
2. Navigate to **Settings → Database** and copy your connection string
3. Navigate to **Settings → API** and copy your **service_role** key
4. Create a storage bucket named `whisper-bucket` (or configure a different name)
5. Set the bucket to **public** for file access
6. Add the values to your `appsettings.Development.json`

---

## 📡 SignalR

The real-time hub is available at:

```
wss://localhost:7247/chathub?access_token={jwt_token}
```

### Hub Events

| Event              | Direction       | Description                        |
| ------------------ | --------------- | ---------------------------------- |
| `ReceiveMessage`   | Server → Client | New message received               |
| `UserConnected`    | Server → Client | A user came online                 |
| `UserDisconnected` | Server → Client | A user went offline                |

### Authentication

Pass the JWT token as a query parameter `access_token` when connecting to the hub.

---

## 📖 API Documentation

Swagger UI is available in development mode:

```
https://localhost:7247/swagger
```

### Endpoints Overview

| Method | Endpoint                           | Description                  |
| ------ | ---------------------------------- | ---------------------------- |
| POST   | `/api/auth/login`                  | Authenticate & get JWT       |
| GET    | `/api/conversations`               | List user conversations      |
| POST   | `/api/conversations`               | Create a new conversation    |
| GET    | `/api/messages/{conversationId}`   | Get messages in conversation |
| POST   | `/api/messages`                    | Send a message               |
| POST   | `/api/uploads`                     | Upload a file                |

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request
