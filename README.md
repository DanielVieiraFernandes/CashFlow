## Sobre o projeto

Esta **API**, desenvolvida utilizando **.NET 8**, adota os princípios do **Domain-Driven Design (DDD)** para oferecer uma solução estruturada e eficaz no gerenciamento de despesas pessoais. O principal objetivo é permitir que os usuários registrem suas despesas, detalhando informações como título, data e hora, descrição, valor e tipo de pagamento, com os dados sendo armazenados de forma segura em um banco de dados **MySQL**.

A arquitetura da **API** baseia-se em **REST**, utilizando métodos **HTTP** padrão para uma comunicação eficiente e simplificada. Além disso, é complementada por uma documentação **Swagger**, que proporciona uma interface gráfica interativa para que os desenvolvedores possam explorar e testar os endpoints de maneira fácil.

Dentre os pacotes NuGet utilizados, o **AutoMapper** é o responsável pelo mapeamento entre objetos de domínio e requisição/resposta, reduzindo a necessidade de código repetitivo e manual. O **FluentAssertions** é utilizado nos testes de unidade para tornar as verificações mais legíveis, ajudando a escrever testes claros e compreensíveis. Para as validações, o **FluentValidation** é usado para implementar regras de validação de forma simples e intuitiva nas classes de requisições, mantendo o código limpo e fácil de manter. Por fim, o **EntityFramework** atua como um ORM (Object-Relational Mapper) que simplifica as interações com o banco de dados, permitindo o uso de objetos .NET para manipular dados diretamente, sem a necessidade de lidar com consultas SQL.

![hero-image]

### Features

- **Domain-Driven Design (DDD)**: Estrutura modular que facilita o entendimento e a manutenção do domínio da aplicação.
- **Autenticação JWT**: Sistema completo de autenticação baseado em tokens JWT (JSON Web Tokens), garantindo segurança no acesso aos recursos da API.
- **Autorização RBAC (Role-Based Access Control)**: Implementação de controle de acesso baseado em funções (roles), com níveis de permissão diferenciados:
  - **Administrator**: Acesso completo a todos os recursos, incluindo geração de relatórios.
  - **Team Member**: Acesso às funcionalidades básicas de gerenciamento de despesas pessoais.
- **Testes Unitários**: Cobertura de **100%** do código com testes unitários abrangentes utilizando **xUnit** e **FluentAssertions**.
- **Testes de Integração**: Testes end-to-end completos que validam o comportamento da API como um todo, garantindo a integração correta entre todos os componentes.
- **Geração de Relatórios**: Capacidade de exportar relatórios detalhados para **PDF e Excel**, oferecendo uma análise visual e eficaz das despesas (exclusivo para administradores).
- **RESTful API com Documentação Swagger**: Interface documentada que facilita a integração e o teste por parte dos desenvolvedores.
- **Health Checks**: Endpoint de monitoramento da saúde da aplicação e conectividade com o banco de dados.

## Getting Started

Para obter uma cópia local funcionando, siga estes passos simples.

### Requisitos

* Visual Studio versão 2022+ ou Visual Studio Code
* Windows 10+ ou Linux/MacOS com [.NET SDK][dot-net-sdk] instalado
* MySql Server

### Instalação

1. Clone o repositório:
    ```sh
    git clone https://github.com/DanielVieiraFernandes/CashFlow.git
    ```

2. Preencha as informações no arquivo `appsettings.Development.json`.
3. Execute a API e aproveite o seu teste :)

---

## 📚 Documentação da API

### Autenticação

A API utiliza **JWT (JSON Web Tokens)** para autenticação. Após o login bem-sucedido, um token é retornado e deve ser incluído no header `Authorization` das requisições protegidas no formato: `Bearer {token}`.

### 🔐 Endpoints de Autenticação e Usuários

#### **POST /api/login**
Realiza o login do usuário e retorna um token JWT.

**Request Body:**
```json
{
  "email": "user@example.com",
  "password": "SuaSenha123"
}
```

**Responses:**
- `200 OK`: Login bem-sucedido, retorna o token JWT e informações do usuário
- `401 Unauthorized`: Credenciais inválidas

---

#### **POST /api/user**
Registra um novo usuário no sistema.

**Request Body:**
```json
{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "SenhaSegura123"
}
```

**Responses:**
- `201 Created`: Usuário criado com sucesso, retorna token JWT
- `400 Bad Request`: Dados inválidos

---

#### **GET /api/user**
Obtém o perfil do usuário autenticado.

**Headers:**
- `Authorization: Bearer {token}`

**Responses:**
- `200 OK`: Retorna informações do perfil do usuário

---

#### **PUT /api/user**
Atualiza as informações do perfil do usuário autenticado.

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "name": "João Silva Atualizado",
  "email": "joao.novo@example.com"
}
```

**Responses:**
- `204 No Content`: Perfil atualizado com sucesso
- `400 Bad Request`: Dados inválidos

---

#### **PUT /api/user/change-password**
Altera a senha do usuário autenticado.

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "currentPassword": "SenhaAtual123",
  "newPassword": "NovaSenha456"
}
```

**Responses:**
- `204 No Content`: Senha alterada com sucesso
- `400 Bad Request`: Senha atual incorreta ou nova senha inválida

---

#### **DELETE /api/user**
Deleta a conta do usuário autenticado.

**Headers:**
- `Authorization: Bearer {token}`

**Responses:**
- `204 No Content`: Conta deletada com sucesso

---

### 💰 Endpoints de Despesas

Todos os endpoints de despesas requerem autenticação.

#### **POST /api/expenses**
Registra uma nova despesa.

**Headers:**
- `Authorization: Bearer {token}`

**Request Body:**
```json
{
  "title": "Almoço",
  "description": "Almoço no restaurante",
  "date": "2024-01-15T12:30:00",
  "amount": 45.50,
  "paymentType": 1,
  "tags": [1, 3]
}
```

**Responses:**
- `201 Created`: Despesa criada com sucesso
- `400 Bad Request`: Dados inválidos

---

#### **GET /api/expenses**
Obtém todas as despesas do usuário autenticado.

**Headers:**
- `Authorization: Bearer {token}`

**Responses:**
- `200 OK`: Retorna lista de despesas
- `204 No Content`: Nenhuma despesa encontrada

---

#### **GET /api/expenses/{id}**
Obtém uma despesa específica por ID.

**Headers:**
- `Authorization: Bearer {token}`

**Parameters:**
- `id` (path, required): ID da despesa

**Responses:**
- `200 OK`: Retorna a despesa
- `404 Not Found`: Despesa não encontrada

---

#### **PUT /api/expenses/{id}**
Atualiza uma despesa existente.

**Headers:**
- `Authorization: Bearer {token}`

**Parameters:**
- `id` (path, required): ID da despesa

**Request Body:**
```json
{
  "title": "Almoço Atualizado",
  "description": "Almoço no novo restaurante",
  "date": "2024-01-15T12:30:00",
  "amount": 50.00,
  "paymentType": 2,
  "tags": [1, 2]
}
```

**Responses:**
- `204 No Content`: Despesa atualizada com sucesso
- `400 Bad Request`: Dados inválidos
- `404 Not Found`: Despesa não encontrada

---

#### **DELETE /api/expenses/{id}**
Deleta uma despesa.

**Headers:**
- `Authorization: Bearer {token}`

**Parameters:**
- `id` (path, required): ID da despesa

**Responses:**
- `204 No Content`: Despesa deletada com sucesso
- `404 Not Found`: Despesa não encontrada

---

### 📊 Endpoints de Relatórios (Apenas Administradores)

Estes endpoints são restritos a usuários com a role **Administrator**.

#### **GET /api/report/excel**
Gera um relatório em Excel das despesas do mês especificado.

**Headers:**
- `Authorization: Bearer {token}`

**Query Parameters:**
- `month` (query, required): Mês no formato YYYY-MM-DD (ex: 2024-01-01)

**Responses:**
- `200 OK`: Retorna o arquivo Excel (report.xlsx)
- `204 No Content`: Nenhuma despesa encontrada para o período
- `403 Forbidden`: Usuário não tem permissão de administrador

---

#### **GET /api/report/pdf**
Gera um relatório em PDF das despesas do mês especificado.

**Headers:**
- `Authorization: Bearer {token}`

**Query Parameters:**
- `month` (query, required): Mês no formato YYYY-MM-DD (ex: 2024-01-01)

**Responses:**
- `200 OK`: Retorna o arquivo PDF (report.pdf)
- `204 No Content`: Nenhuma despesa encontrada para o período
- `403 Forbidden`: Usuário não tem permissão de administrador

---

### 🏥 Endpoint de Health Check

#### **GET /health**
Verifica o status da aplicação e conectividade com o banco de dados.

**Responses:**
- `200 OK`: Aplicação e banco de dados operacionais
- `503 Service Unavailable`: Aplicação ou banco de dados com problemas

---

## 🔒 Sistema de Autorização (RBAC)

A aplicação implementa controle de acesso baseado em roles (funções):

### Roles Disponíveis:

1. **Administrator** (`administrator`)
   - Acesso completo a todos os recursos
   - Pode gerar relatórios em Excel e PDF
   - Gerenciar suas próprias despesas e perfil

2. **Team Member** (`teamMember`)
   - Gerenciar suas próprias despesas (criar, ler, atualizar, deletar)
   - Gerenciar seu próprio perfil
   - Sem acesso aos relatórios

---

## 🧪 Qualidade e Testes

### Cobertura de Testes

O projeto mantém **100% de cobertura de código** através de:

#### **Testes Unitários** (`UseCases.Tests`)
- Testa cada caso de uso individualmente
- Validação de regras de negócio
- Testes de validadores com FluentValidation
- Uso de mocks para isolar dependências
- Framework: **xUnit**
- Assertions: **FluentAssertions**

#### **Testes de Integração** (`WebApi.Test`)
- Testes end-to-end da API
- Validação de autenticação e autorização
- Testes de controllers com banco de dados em memória
- Validação de responses HTTP
- Framework: **xUnit** + **WebApplicationFactory**

#### **Testes de Validadores** (`Validators.Tests`)
- Validação de regras de entrada
- Testes de FluentValidation rules
- Cenários de sucesso e falha

### Executando os Testes

```sh
# Executar todos os testes
dotnet test

# Executar com cobertura
dotnet test /p:CollectCoverage=true
```

---

## 🛠️ Tecnologias e Padrões

### Arquitetura
- **Domain-Driven Design (DDD)**
- **Clean Architecture**
- **Repository Pattern**
- **Dependency Injection**

### Segurança
- **JWT (JSON Web Tokens)** para autenticação
- **RBAC (Role-Based Access Control)** para autorização
- **Password Hashing** com algoritmos seguros
- **HTTPS** obrigatório

### Principais Pacotes NuGet
- **Entity Framework Core** - ORM para acesso a dados
- **AutoMapper** - Mapeamento objeto-objeto
- **FluentValidation** - Validação de requisições
- **FluentAssertions** - Assertions nos testes
- **Swashbuckle (Swagger)** - Documentação da API
- **xUnit** - Framework de testes
- **ClosedXML** - Geração de arquivos Excel
- **PdfSharp** - Geração de arquivos PDF

---



<!-- Links -->
[dot-net-sdk]: https://dotnet.microsoft.com/en-us/download/dotnet/8.0

<!-- Images -->
[hero-image]: images/heroimage.png
