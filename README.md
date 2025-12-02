# 🏢 Gerenciamento Clientes Beneficiarios

Sistema de gerenciamento de clientes e beneficiários desenvolvido em ASP.NET MVC com .NET Framework 4.8.

[![.NET Framework](https://img.shields.io/badge/.NET%20Framework-4.8-blue.svg)](https://dotnet.microsoft.com/download/dotnet-framework/net48)
[![ASP.NET MVC](https://img.shields.io/badge/ASP.NET-MVC%205-green.svg)](https://dotnet.microsoft.com/apps/aspnet/mvc)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2019-red.svg)](https://www.microsoft.com/sql-server)

---

## 📋 **Sobre o Projeto**

Sistema web para cadastro e gerenciamento de clientes e seus beneficiários, com validação completa de CPF e interface responsiva.

### Funcionalidades Principais

✅ Gerenciamento de Clientes
- Cadastro e edição de clientes
- Validação de CPF com algoritmo de dígito verificador
- Verificação de CPF duplicado
- Paginação e ordenação de resultados

✅ Gerenciamento de Beneficiários
- Adicionar múltiplos beneficiários por cliente
- Edição e exclusão de beneficiários
- Validação de CPF duplicado por cliente
- Modal interativo para gerenciamento

✅ Validações
- CPF com máscara automática (000.000.000-00)
- Algoritmo completo de validação de CPF
- Normalização de CPF para evitar duplicatas


## 🛠️ Tecnologias Utilizadas

### Backend
- **ASP.NET MVC 5** (.NET Framework 4.8)
- **C# 7.3**
- **ADO.NET**
- **SQL Server 2019**

### Frontend
- **HTML5 / CSS3**
- **Bootstrap 3.x** (interface responsiva)
- **jQuery 3.x**
- **jTable** (grid de dados)
- **JavaScript ES5**

### Arquitetura
- **Padrão MVC** (Model-View-Controller)
- **Camadas BLL, DAL e DML** (Business Logic Layer, Data Access Layer, Data Model Layer)
- **Repository Pattern**
