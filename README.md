# ✈️ Airport Check-in API

API Minimal desenvolvida em **.NET 9** para o gerenciamento de **check-ins de aeroporto**, permitindo o controle de **administradores, voos e passageiros (clientes)**.  
Inclui autenticação JWT, CRUD completo e integração com **MySQL** via **Entity Framework Core**.

---

## 🚀 Tecnologias Utilizadas

- **.NET 9 Minimal API**
- **Entity Framework Core (Pomelo MySQL)**
- **JWT (JSON Web Token)** para autenticação
- **BCrypt** para hashing de senhas
- **Swagger UI** para documentação automática
- **MySQL** como banco de dados

---

## 🧩 Funcionalidades

### 👨‍✈️ Administradores
- Registro de novos administradores (`/admin/register`)
- Login com geração de token JWT (`/admin/login`)
- Rota de *seed* para criar admin padrão e voos de teste (`/admin/seed`)

### ✈️ Voos
- CRUD completo (`/voos`)
- Campos: número do voo, origem, destino, data de partida e chegada

### 👤 Clientes (Passageiros)
- CRUD completo (`/clientes`)
- Campos: nome, CPF, voo, valor da passagem
- Associação com o voo do cliente

---

## 🗃️ Estrutura do Banco de Dados

**Tabelas principais:**
- `Administradores` → controla o login e permissões
- `Voos` → informações dos voos
- `Clientes` → passageiros vinculados a voos

**Relacionamentos:**
- Cada cliente pertence a um voo (`Cliente.VooId → Voo.Id`)

---

## ⚙️ Configuração e Execução

### 1️⃣ Clonar o repositório
```bash
git clone https://github.com/SEU-USUARIO/airport-checkin-api.git
cd airport-checkin-api
