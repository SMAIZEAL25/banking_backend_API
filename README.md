📦 ProjectRoot
┣ 📂 Application # CQRS Commands/Queries, DTOs, Validators
┣ 📂 Domain # Entities, Enums, Value Objects
┣ 📂 Infrastructure# EF Core, Repositories, Unit of Work, Caching, Paystack Integration
┣ 📂 API # Controllers, Middleware, Startup
┗ 📂 Tests # Unit & Integration Tests



---

## 🔗 Entity Relationships
- **User → Accounts**: One user can have multiple accounts.
- **Account → Transactions**: One account can have multiple transactions.
- **Transaction → Account**: Each transaction is tied to exactly one account.

---

## 🔒 Security
- **JWT-based authentication**.
- **Role-based authorization** (Admin, Customer, etc.).
- **CORS configuration** for frontend integration.
- **Overdraft prevention** with strict validation.

---

## 🧩 Tech Stack
- **Language:** C# (.NET 8)
- **Framework:** ASP.NET Core Web API
- **ORM:** Entity Framework Core
- **Database:** SQL Server
- **Caching:** Redis
- **Validation:** FluentValidation
- **Mediator:** MediatR
- **Logging:** Serilog
- **Testing:** xUnit
- **Auth:** JWT
- **External Payment:** Paystack API

---

## ⚙️ Setup Instructions

### 1️⃣ Clone Repository
```bash
git clone https://github.com/your-username/banking-system.git
cd banking-system

📖 API Documentation
Open Swagger UI at:
https://localhost:5001/swagger

Endpoints include:

POST /api/accounts – Create account

GET /api/accounts/{id} – View account details

POST /api/transactions/deposit – Deposit funds

POST /api/transactions/withdraw – Withdraw funds

POST /api/transactions/transfer – Transfer between accounts

GET /api/transactions/history – Paginated transaction history

GET /api/transactions/monthly-statement – Monthly statement


📊 Health Checks
API Health: /health

Database Health: /health/db

🏅 Evaluation Criteria Alignment
✔ Code Quality – Clean & maintainable
✔ Scalable Design – CQRS, caching, rate limiting
✔ Performance – Optimized DB queries & indexes
✔ Security – JWT, RBAC, encryption
✔ Integration – Paystack support
✔ Testing – xUnit coverage
✔ Documentation – This README + Swagger


