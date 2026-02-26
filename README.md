
# MVCDHProject

## 📌 Project Overview
MVCDHProject is an ASP.NET Core MVC web application built using .NET 10.  
The application demonstrates layered architecture, authentication, authorization, and CRUD operations with SQL Server and XML-based data access.

---

## 🚀 Technologies Used
- ASP.NET Core MVC (.NET 10)
- Entity Framework Core
- SQL Server
- ASP.NET Core Identity
- Google & Facebook External Authentication
- MailKit (Email Confirmation & Password Reset)
- Razor Views

---

## 🏗 Architecture
The project follows a layered architecture using:
- Interface-based Dependency Injection
- Separate Data Access Layer (DAL)
- Identity-based Authentication System

---

## 🔐 Features
- User Registration with Email Confirmation
- Login & Logout
- Forgot Password & Reset Password
- Google & Facebook Login
- Customer CRUD Operations
- Soft Delete Implementation
- Custom Client & Server Error Pages
- Switchable Data Layer (SQL / XML)

---

## 🗄 Database
- SQL Server
- Entity Framework Core (Code First)
- Seed Data using OnModelCreating()

---

## 📧 Email Service
- SMTP-based email sending using MailKit
- Email confirmation token generation
- Password reset token support

---

## ⚙ Setup Instructions

1. Clone the repository
2. Update `appsettings.json` with your SQL Server connection string
3. Update Google & Facebook authentication keys
4. Run:

dotnet restore
dotnet build
dotnet run

---

## 📌 Future Improvements
- Role-based authorization
- Logging integration
- Cloud deployment (Azure / Render)
- API versioning

---

## 👨‍💻 Author
Bishwa Ranjan Das  
.NET Full Stack Developer
