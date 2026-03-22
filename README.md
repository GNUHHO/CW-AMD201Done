\# High-Performance URL Shortener



!\[.NET 10](https://img.shields.io/badge/.NET%2010-5C2D91?style=for-the-badge\&logo=.net\&logoColor=white)

!\[Vue.js](https://img.shields.io/badge/Vue.js%203-%2335495e.svg?style=for-the-badge\&logo=vuedotjs\&logoColor=%234FC08D)

!\[Redis](https://img.shields.io/badge/Redis-%23DD0031.svg?style=for-the-badge\&logo=redis\&logoColor=white)

!\[SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge\&logo=microsoft%20sql%20server\&logoColor=white)

!\[Docker](https://img.shields.io/badge/Docker-%230db7ed.svg?style=for-the-badge\&logo=docker\&logoColor=white)



A modern, full-stack web application designed to convert long, complex URLs into short, manageable links. This project focuses heavily on \*\*high-performance data retrieval\*\* by implementing a Distributed Cache architecture and is fully containerized for a seamless "1-click" deployment.



\---



\##  Key Features



\* \*\* Blazing Fast Redirects with Redis:\*\* Implements the \*Cache-Aside\* pattern to maximize redirect speed (HTTP 302), reducing database query loads by up to 90%.

\* \*\* Smart Cache Invalidation:\*\* The system automatically evicts stale cache data during `PUT` (Update) and `DELETE` operations, guaranteeing absolute Data Consistency.

\* \*\* Client-Side QR Code Generation:\*\* QR codes for original URLs are rendered dynamically on the browser via Vue.js, offloading image processing from the backend server.

\* \*\* Clean Architecture:\*\* The C# Backend strictly adheres to the `Service Pattern` and `Dependency Injection`, completely decoupling the API Controllers from the Data Access Layer.

\* \*\* Seamless Deployment:\*\* The Frontend, Backend API, Database, and Redis cache are fully Dockerized and connected via a private Docker network. No local SDKs required!



\---



\## Tech Stack



| Component | Technology |

| :--- | :--- |

| \*\*Frontend\*\* | Vue.js 3, Vite, Bootstrap 5, `qrcode.vue` |

| \*\*Backend API\*\* | ASP.NET Core 10.0 Web API, Entity Framework Core 10 |

| \*\*Database\*\* | Microsoft SQL Server (Containerized) |

| \*\*Caching Layer\*\*| Redis (Alpine Image) |

| \*\*DevOps\*\* | Docker, Docker Compose |



\---



\## System Architecture \& Data Flow



1\. \*\*Redirect Flow (GET `/{code}`):\*\* Client -> `API Controller` -> Check `Redis Cache`.

&#x20;  \* \*\*Cache Hit:\*\* Immediately return the cached Original URL -> Redirect (302).

&#x20;  \* \*\*Cache Miss:\*\* Query `SQL Server` -> Store result in `Redis` (with TTL) -> Redirect (302).

2\. \*\*Update/Delete Flow (PUT/DELETE):\*\*

&#x20;  Update or Remove the record in `SQL Server` -> Simultaneously \*\*Delete the corresponding Key in `Redis`\*\* to prevent stale data routing.



\---



\## Getting Started



The system is completely environment-agnostic. You \*\*DO NOT\*\* need to install Node.js, .NET SDK, or SQL Server locally. 



\*\*Prerequisite:\*\* Ensure you have \[Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running.



\### Step 1: Clone the Repository

```bash

git clone \[https://github.com/GNUHHO/CW-AMD201Done.git](https://github.com/GNUHHO/CW-AMD201Done.git)

cd CW-AMD201Done

