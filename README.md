<!-- 🌟 PROJECT BANNER -->
<p align="center">
  <img src=".github/images/banner.png" alt="Ecommerce Pharmacy Banner" width="100%" />
</p>

<h1 align="center">💊 Ecommerce_Pharmacy</h1>

<p align="center">
  A modern Pharmacy E-commerce platform built with <b>ASP.NET Core</b> and <b>Clean Architecture</b>.  
  It offers product browsing, shopping cart, order management, secure authentication, and role-based access.  
  Designed for scalability, performance, and real-world pharmacy workflows.
</p>

<p align="center">
  <a href="https://github.com/YOUR_USERNAME/Ecommerce_Pharmacy/stargazers"><img src="https://img.shields.io/github/stars/YOUR_USERNAME/Ecommerce_Pharmacy?color=yellow" alt="Stars Badge"/></a>
  <a href="https://github.com/YOUR_USERNAME/Ecommerce_Pharmacy/network/members"><img src="https://img.shields.io/github/forks/YOUR_USERNAME/Ecommerce_Pharmacy?color=blue" alt="Forks Badge"/></a>
  <a href="https://github.com/YOUR_USERNAME/Ecommerce_Pharmacy/blob/main/LICENSE"><img src="https://img.shields.io/github/license/YOUR_USERNAME/Ecommerce_Pharmacy?color=green" alt="License Badge"/></a>
  <a href="#"><img src="https://img.shields.io/badge/.NET-9.0-blueviolet" alt=".NET Version"/></a>
</p>

---

## 🏗️ Architecture Overview

The project follows the **Clean Architecture** principle to ensure clear separation of concerns, scalability, and maintainability.

📦 Pharmacy
┣ 📂 Pharmacy.Api → Presentation Layer (Controllers, Middlewares, Swagger)

┣ 📂 Pharmacy.Application → Application Layer (CQRS, Commands, Queries, Validators)

┣ 📂 Pharmacy.Domain → Core Domain Layer (Entities, ValueObjects, Enums)

┣ 📂 Pharmacy.Infrastructure → Infrastructure Layer (EF Core, Repositories, Configurations)

┣ 📂 Pharmacy.Design → Design Patterns Layer (Strategies, Specifications, etc.)

┣ 📂 Pharmacy.Shared → Shared Utilities & DTOs

---

## 🚀 Key Features

- 🧠 **Clean Architecture** with CQRS pattern  
- 🧩 **Entity Framework Core** for ORM and SQL Server  
- 🔐 **JWT Authentication** & Role-Based Authorization (Admin, Customer, Delivery)  
- 🛒 **Shopping Cart & Order Management**  
- 💳 **Product & Price Management** with Supplier Discount Logic  
- 🏷️ **Category & Inventory Management**  
- 🌍 **Localization & Globalization Support**  
- 🧾 **ProblemDetails** + **FluentValidation** for standardized errors  
- 🧱 **Unit of Work & Repository Patterns**  
- ⚙️ **Specification Pattern** for complex queries  
- 🧠 **Strategy Pattern** for discount and pricing logic  
- 📦 **API Documentation** with Swagger UI  

---

## 🧩 Tech Stack

| Layer | Technology |
|-------|-------------|
| Backend Framework | ASP.NET Core 9 |
| ORM | Entity Framework Core |
| Database | SQL Server |
| Architecture | Clean Architecture + CQRS |
| Patterns | Repository, Unit of Work, Strategy, Specification |
| Validation | FluentValidation |
| Documentation | Swagger / OpenAPI |
| Authentication | JWT (JSON Web Tokens) |

---


🧠 Design Patterns Used

Repository Pattern → Abstracts data access logic

Unit of Work Pattern → Ensures transactional consistency

Strategy Pattern → Handles discount calculation per product/supplier

Specification Pattern → Simplifies dynamic filtering and pagination

CQRS Pattern → Separates read/write responsibilities

Factory Pattern → Used for DbContext creation at design-time

🧪 Future Enhancements

🛍️ Frontend Integration (Blazor or Angular)

📱 Mobile App Gateway (for mobile clients)

🧠 AI-driven Product Recommendation Engine

🧾 Payment Gateway Integration

🚚 Delivery Tracking & Real-Time Notifications

👨‍💻 Author

Youssef Elmesedy
.NET Developer | Clean Architecture Enthusiast
📧 Email

🌐 GitHub Profile

🪪 License

This project is licensed under the MIT License – see the LICENSE
 file for details.

<p align="center"> ⭐ If you like this project, don’t forget to <b>star</b> the repository! </p> ```

