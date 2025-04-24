# Game‑Store (Demo project)

A hobby project that re‑creates the core flow of a digital game store (think Steam / Epic Games Store).

---

## Tech stack

| Layer    | Tech                                                  |
|----------|-------------------------------------------------------|
| Frontend | **Angular 17**, Bootstrap 5                           |
| Backend  | **.NET 9**, MSSQL, RabbitMQ, gRPC                     |
| Runtime  | Docker, Kubernetes (EKS)                              |
| Storage  | Azure Blob Storage (for images)                       |

---

## Architecture highlights

* Polyglot **micro‑services** orchestrated by **Kubernetes**.  
* Services talk via **RabbitMQ (event bus)** and **gRPC** where low‑latency is required.  
* Stateless REST + JWT auth; Google OAuth for social sign‑in.  
* CDN‑ready static site hosted in an S3‑style bucket; images off‑loaded to Azure Blob.  

---

## Features (all fully responsive)

<details>
<summary><strong>📚 Game catalog</strong></summary>

![catalog‑1](https://github.com/user-attachments/assets/8bdecb24-e8aa-44de-9b88-3a5e94ed53e7)  
![catalog‑2](https://github.com/user-attachments/assets/6b3b6b8f-13bf-489b-8c3e-5790825d1502)
</details>

<details>
<summary><strong>🎮 Game page</strong></summary>

![game‑1](https://github.com/user-attachments/assets/57acc348-d82c-4298-b911-3d9a03ecba74)  
![game‑2](https://github.com/user-attachments/assets/e474627f-1f21-42ba-be90-5aff408e771a)
</details>

<details>
<summary><strong>🔍 Search</strong></summary>

![search‑1](https://github.com/user-attachments/assets/ca1dd591-352b-4a9a-9bcf-4b1dda442f60)  
![search‑2](https://github.com/user-attachments/assets/a98ee048-59e9-49a6-9676-ce79cde6dd9e)
</details>

<details>
<summary><strong>🔐 Authentication</strong></summary>

Supports classic accounts + Google OAuth.  
![auth-1](https://github.com/user-attachments/assets/6f510ee1-48f2-4a74-9584-475384d8e82a)
![auth-2](https://github.com/user-attachments/assets/44051a7c-ecc6-4c0c-a654-2ba2e950e826)
</details>

<details>
<summary><strong>🛒 Cart & Checkout</strong></summary>

Checkout is demo‑only; no real payment captured.  
![cart‑1](https://github.com/user-attachments/assets/b7e52e27-882c-433d-a436-3cf545a89fcb)  
![cart‑2](https://github.com/user-attachments/assets/36f2a9e0-34c3-4068-9a22-0c866204f6c6)
![cart‑3](https://github.com/user-attachments/assets/ed4f3ecb-d4f8-410d-8dc0-d0890e268ba6)
</details>

<details>
<summary><strong>📚 Library</strong></summary>
  The Library page is for demonstration only.
  It displays mock data, and game installation is intentionally not supported.

  ![library](https://github.com/user-attachments/assets/1cc231a8-b3ba-44ae-9232-a90f4f203cc5)
</details>

<details>
<summary><strong>🛠 Admin panel (CRUD for games)</strong></summary>

![admin](https://github.com/user-attachments/assets/379580a8-f786-49c0-871a-0c621d59569c)
</details>

---

> **Note**  
> This repository is for educational purposes only—no commercial assets or real transactions are included.
