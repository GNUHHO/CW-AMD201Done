# T-Shortener | High-Performance URL Shortener

T-Shortener is a modern, fast, and robust URL shortening service built with **ASP.NET Core MVC** and **Web API**. It provides both a user-friendly web interface with glassmorphism design and a fully documented RESTful API for developers. The entire system is containerized using **Docker** for seamless deployment.

---

## Key Features

* **Blazing Fast Redirection:** Instantly redirects users from a short code to the original long URL.
* **Modern UI/UX:** Responsive web interface built with Bootstrap 5, featuring a clean, glassmorphism-inspired aesthetic.
* **Auto QR Code Generation:** Automatically generates downloadable QR codes for every shortened URL using `qrcode.js`.
* **History Management:** View recent shortened links and easily clear the entire history with a single click.
* **Full REST API:** Includes a complete CRUD API for programmatic access, documented interactively with Swagger UI.
* **Docker Ready:** Fully containerized backend and database (SQL Server) using `docker-compose` for a "zero-config" setup.

---

## Tech Stack

### Backend
* **Framework:** .NET (ASP.NET Core MVC & Web API)
* **Database:** Microsoft SQL Server 2022
* **ORM:** Entity Framework Core
* **Documentation:** Swashbuckle (Swagger)

### Frontend
* **Design:** HTML5, CSS3 (Custom Glassmorphism)
* **Framework:** Bootstrap 5
* **Libraries:** QRCode.js

### DevOps & Architecture
* **Containerization:** Docker & Docker Compose
* **Pattern:** MVC (Model-View-Controller) & Repository Pattern

---

## Getting Started

Follow these instructions to get a copy of the project up and running on your local machine.

### Prerequisites
* [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.
* *(Optional)* .NET SDK if you want to run it outside of Docker.

### Installation & Running

1.  **Clone the repository:**
    ```bash
    git clone [https://github.com/your-username/T-Shortener.git](https://github.com/your-username/T-Shortener.git)
    cd T-Shortener
    ```

2.  **Build and spin up the containers:**
    ```bash
    docker-compose up -d --build
    ```

3.  **Access the application:**
    * **Web Interface:** Open your browser and navigate to `http://localhost:5078`
    * **API Documentation (Swagger):** Navigate to `http://localhost:5078/swagger`
    * **Database Access (SSMS):** Connect to `127.0.0.1,1435` with User `sa` and your configured password.

---

## Project Structure

```text
T-Shortener/
├── docker-compose.yml         # Docker multi-container orchestration
└── UrlShortener.API/          # Main ASP.NET Core Application
    ├── Controllers/           # Handles Web (Home) and API routing
    ├── Data/                  # Entity Framework DbContext & Migrations
    ├── Services/              # Core business logic (URL generation)
    ├── Views/                 # Razor pages for the UI
    ├── wwwroot/               # Static files (CSS, JS, Images)
    └── Program.cs             # App configuration & middleware pipeline