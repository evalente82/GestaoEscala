# 🚀 Gestão de Escalas e Permutas

> API RESTful para gerenciamento de escalas e permutas de trabalho — desenvolvida em **C# .NET (ASP.NET Core 7)** com suporte ao frontend em **JavaScript + HTML**.

![Tecnologia - C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
![JavaScript](https://img.shields.io/badge/JavaScript-F7DF1E?style=for-the-badge&logo=javascript&logoColor=black)
![HTML5](https://img.shields.io/badge/HTML5-E34F26?style=for-the-badge&logo=html5&logoColor=white)

---

## 📌 Visão Geral

Este repositório contém a **API RESTful** central para o sistema de **Gestão de Escalas e Permutas**, desenvolvida em **C# .NET (ASP.NET Core 7)**. A aplicação é responsável por:
- Gerenciar escalas de trabalho
- Registrar e organizar permutas entre funcionários
- Prover endpoints seguros e eficientes para interação com sistemas cliente (mobile, web, integradores)

O backend é robusto, escalável e segue padrões modernos de desenvolvimento, enquanto o frontend utiliza **JavaScript + HTML** para interfaces simples e funcionais.

---

## 🚀 Funcionalidades Principais

### 🔹 Backend (C# .NET)
- ✅ API RESTful para gerenciamento de usuários, escalas e permutas
- ✅ Autenticação segura com **JWT** e middleware de proteção
- ✅ Persistência com **SQL Server** ou **PostgreSQL** (configurável via variáveis de ambiente)
- ✅ Validação de dados com **Fluent Validation**
- ✅ Configuração de variáveis de ambiente (`.env`)
- ✅ Suporte a CORS para comunicação com frontend e mobile
- ✅ Estrutura **Clean Architecture** organizada:
  -✅ `Controllers` – manipulação das requisições HTTP
  - ✅`Services` – lógica de negócios
  - ✅`Repositories` – acesso ao banco de dados
  - ✅`Models` – definição de entidades
---


---

## 🚀 Funcionalidades do Frontend

### 🔹 Frontend (React.js + CSS)
- ✅ Interface web moderna para visualização de escalas e permutas
- ✅ Registro de novas permutas e solicitações
- ✅ Dashboard interativo com filtros e relatórios
- ✅ Login seguro com autenticação via API
---
## 🛠️ Tecnologias Utilizadas

| Camada | Tecnologia | Função |
|-------|-----------|------|
| **Backend** | ASP.NET Core 7 | Servidor API REST |
| **Banco de Dados** | SQL Server / PostgreSQL | Armazenamento de dados estruturado |
| **Autenticação** | JWT | Tokens seguros para sessão de usuário |
| **Frontend** | React.js + CSS | Interface de usuário moderna |
| **Validação** | Fluent Validation | Sanitização e validação de entradas |
| **Segurança** | CORS, HTTPS, dotenv | Proteção básica de headers e variáveis |
| **Logs** | Serilog (opcional) | Monitoramento de requisições e erros |

---

## 🗂️ Estrutura de Pastas
.
├── Back/
│ ├── Controllers/ # Controladores HTTP
│ ├── Services/ # Lógica de negócios
│ ├── Repositories/ # Acesso ao banco de dados
│ ├── Models/ # Entidades do domínio
│ ├── Startup.cs # Configuração do aplicativo
│ └── appsettings.json # Configurações do ambiente
│
├── Front/
│ ├── index.html # Página principal
│ ├── scripts.js # Lógica do frontend
│ └── styles.css # Estilização da interface
│
├── .dockerignore
├── Dockerfile # Containerização do backend
├── docker-compose.yml # Orquestração: backend, banco de dados
└── README.md

---

## 🌐 API REST (Principais Endpoints)

| Método | Rota | Descrição |
|-------|------|----------|
| `POST` | `/api/auth/login` | Autentica usuário e retorna token JWT |
| `GET` | `/api/users` | Lista todos os usuários (requer autenticação) |
| `POST` | `/api/escalas` | Cria uma nova escala de trabalho |
| `GET` | `/api/escalas` | Retorna todas as escalas (com filtro opcional) |
| `POST` | `/api/permutas` | Registra uma nova permuta |
| `GET` | `/api/permutas/:id` | Consulta histórico de permutas de um usuário |

> 🔐 Todas as rotas sensíveis são protegidas por middleware de autenticação JWT.

---

## 🐳 Deploy & Containerização

O projeto utiliza **Docker** para orquestrar os serviços:

```yaml
services:
  api:
    build: ./Back
docker-compose up --build
```
Ideal para desenvolvimento local, CI/CD e deploy em produção (VPS, cloud, etc).

🎯 Objetivo do Projeto
Criar uma solução robusta e modular para gerenciar escalas e permutas de trabalho, unificando:

App Mobile (via Flutter ou React Native)
Painel Web Admin (via JavaScript + HTML)
Integrações Externas (API aberta para terceiros)
Com foco em:

Simplicidade
Segurança
Escalabilidade
🤝 Contribuição
Contribuições são bem-vindas! Este é um ótimo projeto para quem deseja aprender:

Arquitetura Clean em ASP.NET Core
Autenticação JWT
Integração com bancos de dados SQL
Docker em aplicações fullstack
Como contribuir:
🍴 Faça um fork
🌿 Crie uma branch (git checkout -b feature/graficos)
💾 Commit suas alterações
🚀 Envie para o GitHub
📥 Abra um Pull Request
📄 Licença
Este projeto está licenciado sob a MIT License. Veja o arquivo LICENSE para mais detalhes.

📬 Contato
Desenvolvido por Evalente82
🔧 Conectando pessoas, dados e plataformas com simplicidade.

<a href="https://github.com/evalente82">
<img src="https://img.shields.io/badge/Ver%20Perfil%20no%20GitHub-181717?style=for-the-badge&logo=github" alt="GitHub Profile">
</a>

"💼 Backend sólido. Frontend claro. Gestão eficiente. "
