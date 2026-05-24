<br />

<div align="center">
  <h1>🐾 PawShelter Management System</h1>
  <img src="https://img.shields.io/badge/License-MIT-green" alt="License MIT" />
  <img src="https://img.shields.io/badge/Status-Phase%201%20Complete-blue" alt="Status" />
  <img src="https://img.shields.io/badge/.NET-10.0-512bd4?logo=dotnet" alt=".NET 10" />
  <img src="https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white" alt="Postgres" />
  <img src="https://img.shields.io/badge/Architecture-N--Tier-orange" alt="Architecture" />
  <img src="https://img.shields.io/badge/Security-AES--256-red" alt="Security" />

  <br />
  <br />

  <p align="center">
    <b>A Animal Shelter Management System built with C# and PostgreSQL.</b>
    <br />
    <i>Developed as part of the SGBD Bachelor Curriculum.</i>
  </p>

  <p align="center">
    <a href="#-project-overview">Overview</a> •
    <a href="#-technical-features">Features</a> •
    <a href="#-setup--installation">Setup</a> •
    <a href="#-tech-stack">Stack</a>
  </p>
</div>

---

## Project Overview
PawShelter is a Animal Shelter Management System developed as part of the SGBD (Database Management Systems) curriculum. 
The system provides a robust architecture to manage animals, contacts (volunteers, adopters, candidates), health records, foster care movements, and adoption processes.

**Current Status:** Phase 1 - Console Application with Direct SQL (ADO.NET Pur).

---

## Architecture (N-Tier / Clean Architecture)
The project is structured into four distinct layers to ensure separation of concerns, maintainability, and scalability:

1.  **AnimalShelter.Core**: Domain models, Enums, Interfaces (Contracts), and custom Exceptions. 
2.  **AnimalShelter.DAL (Data Access Layer)**: Low-level database interaction using **ADO.NET Pur (Npgsql)**. Includes custom Mappers, Query constants, and a generic `DbHelper` (Micro-ORM style).
3.  **AnimalShelter.BLL (Business Logic Layer)**: Business rules validation, security (encryption/hashing), and service orchestration.
4.  **AnimalShelter.ConsoleApp**: Presentation layer featuring a rich CLI with ASCII art, table rendering, and robust input handling.

---

## Technical Features

### Infrastructure & Database
- **Dockerized Environment**: Fully containerized PostgreSQL database using `docker-compose`.
- **Custom ID Generation**: Animals are identified by a unique `yymmdd99999` format generated via a PostgreSQL Sequence and Function.
- **Data Integrity**: 
    - **Transactions**: Atomic operations for complex inserts (e.g., Contact + Address).
    - **Soft Delete**: Logical deletion across all modules using `deleted_at` timestamps.
    - **Audit Logs**: Automatic `updated_at` triggers on every table.
    - **Enum Mapping**: Native mapping between C# Enums and PostgreSQL Types.

### Security & Privacy (RGPD Ready)
- **AES-256 Encryption**: Sensitive data (National Register) is encrypted at the BLL level before storage.
- **Blind Indexing (SHA-256)**: Deterministic hashing of encrypted fields to allow unique constraints without compromising privacy.
- **Input Validation**: Centralized validators for all business objects.

### ser Interface
- **Robust CLI**: `ConsoleHelper` ensures the app never crashes on invalid user input.
- **Visual Feedback**: Themed UI with color-coded messages (Success, Warning, Error), custom boxes, and loading animations.
- **Table Rendering**: Dynamic formatting for listing animals, contacts, and histories.

---

## Implemented Functional Requirements (PDF Compliance)
- [x] **Animals**: Add, Consult, List (Active), Soft Delete.
- [x] **Information**: Manage Descriptions, Particularities, and Compatibilities (OK Cat, OK Dog, etc.).
- [x] **Contacts**: Manage Volunteers, Adopters, and Candidates with full Address support.
- [x] **Health**: Complete Vaccination history tracking per animal.
- [x] **Foster Care**: Track history of foster families for animals and current animals per family.
- [x] **Adoptions**: Manage the full lifecycle (Request -> Approval/Rejection -> Status Update).

---

## Setup & Installation

### Prerequisites
- **.NET 10 SDK** (Preview or Latest)
- **Docker & Docker Compose**
- **Git**

### 1. Clone the Repository
First, clone the project to your local machine:
```bash
git clone https://github.com/votre-username/Gregory_Colard_SGDB_2026.git
cd Gregory_Colard_ShelterManagement
```

### 2. Restore Dependencies
Restore all NuGet packages for the entire solution:
```bash
dotnet restore
```

### 3. Configuration (.env)
Ensure you have a `.env` file in the `docker/` folder. You can use the `.env.example` as a template:
```bash
cp docker/.env.example docker/.env
```
**Required Variables:**
- `DB_HOST=localhost`
- `DB_PORT=5432`
- `POSTGRES_USER=your_user`
- `POSTGRES_PASSWORD=your_password`
- `POSTGRES_DB=animal_shelter_db`
- `ENCRYPTION_KEY=your_32_chars_secret_key_here`

### ⚠️ Windows Specific Notes
- **File Encoding**: If you are creating the `.env` file on Windows, ensure it is saved with **UTF-8 encoding (without BOM)**. Some editors (like Notepad) might add hidden characters that prevent Docker or the .NET application from reading the variables correctly.
- **Line Endings (LF vs CRLF)**: Ensure that the SQL scripts in `docker/init-db/` use **LF** (Linux) line endings. If they use CRLF (Windows), the PostgreSQL container might fail to execute them during initialization.
- **Validation**: You can verify that your environment variables are correctly loaded by Docker by running the following command in the `docker/` folder:
```bash
docker-compose config
```

### 4. Database Infrastructure
Navigate to the `docker/` folder and start the PostgreSQL container:
```bash
cd docker
docker compose up -d
cd ..
```

<span style="color:purple; font-weight:bold">The database will automatically initialize its schema and types using the `init-db/script.sql` file`.</span>


### 5. Running the Application
Launch the Console application from the root directory:
```bash
dotnet run --project src/AnimalShelter.ConsoleApp
```
---

## Tech Stack
- **Language**: C# 13 (.NET 10)
- **Database**: PostgreSQL 16+
- **Driver**: Npgsql
- **Environment**: dotenv.net
- **DevOps**: Docker, Husky (Git Hooks)

---
**Developer:** Gregory Colard  
**Project Year:** 2025-2026

---
