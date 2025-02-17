# Digital Wallet Application

A full-stack digital wallet application built with React frontend and .NET backend, allowing users to manage their digital wallet, view transactions, and transfer funds.

## Prerequisites

- Node.js (v16.0 or higher)
- .NET SDK 7.0 or higher
- SQL Server
- Git

## Project Structure

```
DigitalWallet/
├── wallet_frontend/       # React frontend application
└── dee_backend/           # .NET backend application
```

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/SamWaku/DigitalWallet
cd DigitalWallet
```

### Backend Setup

1. Navigate to the backend directory:

```bash
cd dee_backend/api
```

2. Restore dependencies:

```bash
dotnet restore
```

3. Update database connection string:

- Open `appsettings.json`
- Modify the `DefaultConnection` string to match your SQL Server configuration:

```json
"ConnectionStrings": {
    "DefaultConnection": "Data Source=PCUser\\SQLEXPRESS;Initial Catalog=databasename;Integrated Security=True   TrustServerCertificate=True"
}
```

4. Apply database migrations:

```bash
dotnet ef database update
```

5. Run the backend:

```bash
dotnet run
```

The API will be available at `http://localhost:5143`

### Frontend Setup

1. Open a new terminal and navigate to the frontend directory:

```bash
cd wallet_frontend
```

2. Install dependencies:

```bash
npm install
```

3. Create `.env` file and configure API URL:

```
VITE_API_BASE_URL=http://localhost:5143
```

4. Start the frontend application:

```bash
npm run dev
```

The application will be available at `http://localhost:5173/`

## Core Features

### 1. User Management

- **Create Account**

  - Path: `/register`
  - Fields:
    - Username
    - Email
    - Password

- **Login**
  - Path: `/login`
  - Fields:
    - Email
    - Password

### 2. Wallet Management

- **View Wallet**
  - Path: `/wallet`
  - Displays:
    - Username
    - Email
    - Current Balance

### 3. Transactions

- **View Transactions**

  - Path: `/transactions`
  - Displays:
    - Transaction Amount
    - Receiver Details
    - Date/Time (I forgot to add this, but I'll surely do next time)
    - Status

- **Transfer Funds**
  - Path: `/transfer`
  - Fields:
    - Receiver (email/username)
    - Amount

## API Endpoints

### Authentication

```
POST /api/register
POST /api/login
```

### Wallet

```
GET /api/wallet/user/{userId}
GET /wallet-transactions/{userId}
POST /transfer
```

## Environment Variables

### Backend (appsettings.json)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=PCUser\\SQLEXPRESS;Initial Catalog=databasename;Integrated Security=True TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "Your-JWT-Secret-Key",
    "ExpiryHours": 24
  }
}
```

### Frontend (.env)

```
VITE_API_BASE_URL=http://localhost:5143
```

## Development Notes

1. The backend uses:

   - Entity Framework Core for database operations
   - JWT for authentication
   - Mappers(Manual) for object mapping
   - Repository pattern for data access

2. The frontend uses:
   - React Router for navigation
   - Axios for API calls
   - JWT for authentication storage

## Common Issues & Troubleshooting

1. **Database Connection**

   - Ensure SQL Server is running
   - Verify connection string in `appsettings.json`
   - Check SQL Server authentication mode

2. **CORS Issues**

   - Backend CORS policy is configured for `http://localhost:5173`

3. **JWT Token**
   - Tokens expire after 24 hours
   - Ensure correct secret key in backend configuration
