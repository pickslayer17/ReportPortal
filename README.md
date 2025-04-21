# ReportPortal

Fully containerized backend + frontend project for test reporting.

## 📦 Components

- 🧠 **Backend** — ASP.NET Core + EF Core
- 🖥️ **Frontend** — Vite + React, Nginx
- 🗃️ **Database** — Microsoft SQL Server
- ⚙️ **Migrations** — EF Core migrator
- 🧬 **Git clone** — automatically pulls source code

---

## 🔧 Requirements

- Docker (https://www.docker.com)
- Docker Compose (included in Docker Desktop)

---

## 🚀 Project startup

```bash
docker compose up --build
```

---

## 🌐 Access

- **Frontend**: http://localhost:3000  
- **Backend API**: http://localhost:5000  
  *(could be set through docker-compose.yml)*

---

## 📁 Service structure

| Service     | Purpose                    | Port     |
|-------------|----------------------------|----------|
| `sql`       | MSSQL database             | 1433     |
| `git-clone` | Clones the repository      | -        |
| `builder`   | Builds .NET backend        | -        |
| `migrator`  | Applies EF Core migrations | -        |
| `backend`   | ASP.NET Core API           | 5000:80  |
| `frontend`  | React + Nginx              | 3000:80  |

---

## 📌 Environment variables

- `VITE_API_URL` — API URL used by frontend during build  
- `FrontEndUrl` — frontend URL passed to backend for CORS and SignalR

