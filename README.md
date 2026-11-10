# PrepHub — Placement Training Revision Platform

A comprehensive web app for placement training batches to revise curriculum topics, practice MCQs, review interview questions, and track progress toward hiring milestones.

## Tech Stack

| Layer | Technology |
|-------|-----------|
| **Frontend** | React 18 + TypeScript, Vite, React Router v6, TanStack Query, Tailwind CSS, Monaco Editor |
| **Backend** | ASP.NET Core 8 Web API, Entity Framework Core, ASP.NET Identity + JWT |
| **Database** | PostgreSQL 16 |
| **Containerization** | Docker + Docker Compose |

## Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [PostgreSQL 16](https://www.postgresql.org/) (or use Docker)
- [Docker](https://www.docker.com/) (optional, for containerized setup)

### Option 1: Docker Compose (Recommended)

```bash
docker-compose up --build
```

- Frontend: http://localhost:5173
- Backend API: http://localhost:5000
- Swagger: http://localhost:5000/swagger
- PostgreSQL: localhost:5432

### Option 2: Manual Setup

#### 1. Start PostgreSQL
Make sure PostgreSQL is running on `localhost:5432` with a database named `prephub`.

#### 2. Backend
```bash
cd backend
dotnet restore
dotnet ef database update --project PrepHub.Infrastructure --startup-project PrepHub.API
dotnet run --project PrepHub.API
```
The API starts at http://localhost:5000. Swagger UI at http://localhost:5000/swagger.

#### 3. Frontend
```bash
cd frontend
npm install
npm run dev
```
The app starts at http://localhost:5173.

## Environment Variables

### Backend (`backend/PrepHub.API/appsettings.json`)

| Variable | Description | Default |
|----------|-------------|---------|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string | `Host=localhost;Port=5432;Database=prephub;Username=postgres;Password=postgres` |
| `Jwt__Key` | JWT signing key (min 32 chars) | `YourSuperSecretKeyThatIsAtLeast32CharactersLong!` |
| `Jwt__Issuer` | JWT issuer | `PrepHub` |
| `Jwt__Audience` | JWT audience | `PrepHub` |
| `Jwt__AccessTokenExpirationMinutes` | Access token TTL | `60` |
| `Jwt__RefreshTokenExpirationDays` | Refresh token TTL | `7` |

> [!IMPORTANT]
> Change `Jwt__Key` to a unique secret before deploying to production!

## Default Admin Account

| Email | Password |
|-------|----------|
| `admin@prephub.com` | `Admin@123` |

## Seed Data

The database is seeded on first run with:
- 12 courses matching the training curriculum
- Topics and chapters for each course
- Sample MCQs, Interview Questions, and Notes for key chapters
- Hiring stages with countdown dates
- Default admin account

## Bulk Import

Import content via JSON file. Use `sample-import.json` in the project root as a template.

**Via API:**
```bash
curl -X POST http://localhost:5000/api/admin/import \
  -H "Authorization: Bearer <admin-token>" \
  -H "Content-Type: application/json" \
  -d @sample-import.json
```

**Via UI:** Go to Admin > Import tab, upload a JSON file.

### Import JSON Format
```json
{
  "mcqs": [
    {
      "chapterId": 1,
      "question": "Your question here?",
      "optionA": "Option A",
      "optionB": "Option B",
      "optionC": "Option C",
      "optionD": "Option D",
      "correctOption": "A",
      "explanation": "Why A is correct",
      "difficulty": "Easy|Medium|Hard",
      "tags": ["tag1", "tag2"]
    }
  ],
  "interviewQuestions": [
    {
      "chapterId": 1,
      "question": "What is...?",
      "modelAnswer": "The answer is...",
      "difficulty": "Easy|Medium|Hard",
      "frequency": "Common|Occasional|Rare",
      "tags": ["tag1"]
    }
  ],
  "notes": [
    {
      "chapterId": 1,
      "title": "Topic Revision Sheet",
      "markdownBody": "# Markdown content here..."
    }
  ]
}
```

## Project Structure

```
PrepHub/
├── backend/
│   ├── PrepHub.API/            # Web API controllers, middleware, Program.cs
│   ├── PrepHub.Core/           # Entities, DTOs, interfaces, enums
│   ├── PrepHub.Infrastructure/ # EF Core, repositories, services
│   └── PrepHub.Tests/          # xUnit tests
├── frontend/
│   └── src/
│       ├── components/         # Reusable UI components
│       ├── pages/              # Route pages
│       ├── services/           # API client layer
│       ├── context/            # React context (auth)
│       ├── hooks/              # Custom hooks
│       └── types/              # TypeScript interfaces
├── docker-compose.yml
├── sample-import.json
└── README.md
```

## Build Phases

### ✅ Phase 1 (MVP) — Current
- Auth (register, login, JWT)
- Course/Topic/Chapter browsing with seed data
- Notes, Interview Q&A, MCQs per chapter
- MCQ practice + quiz + attempt tracking
- Progress tracking (per chapter, per course)
- Contribution system + admin approval
- Bulk import (JSON)
- Hiring stage countdown
- Responsive mobile-friendly UI with dark mode

### 🔜 Phase 2
- Coding practice with Judge0 sandbox
- SQL practice
- Mock test mode
- AI features (draft generation, answer feedback, hints)
- Bookmarks, search, weak areas

### 📋 Phase 3
- Flashcards
- Streaks & leaderboard
- Doubt chat
- PWA / offline support
- Google login

## API Documentation

Full API documentation is available via Swagger at http://localhost:5000/swagger when running the backend.

## Testing

```bash
# Backend tests
cd backend
dotnet test

# Frontend (if tests are set up)
cd frontend
npm test
```

## License

Private — for internal batch training use.
