# SomaPraMim - Backend

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=csharp&logoColor=white)  
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)  
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)  
![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)  

SomaPraMim é uma aplicação para facilitar a soma rápida de produtos no mercado ou ambientes de compra de produtos diversos.  

## 🚀 Tecnologias  

- **C#**  

## 📌 Status do Projeto  
Atualmente, o projeto conta com:  
✅ Implementação das regras de negócio para Shopping List, User e Shopping Item  
✅ Construção de rotas e controllers  
✅ Desenvolvimento de services para comunicação com o banco de dados via Entity Framework  
✅ Configuração do PostgreSQL com Docker Compose  
✅ Testes unitários implementados para User utilizando xUnit   

## 📦 Instalação

### 1. Suba os serviços do banco de dados com Docker Compose:

```bash
docker-compose up -d
```

### 2. Restaure as dependências do projeto:

```bash
dotnet restore
```

### 3. Execute as migrações para garantir que o banco de dados esteja atualizado:

```bash
dotnet ef database update
```

### 4. Rodar a aplicação:

```bash
dotnet run
```
