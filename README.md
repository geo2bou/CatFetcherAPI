# Cat Fetcher API

Cat Fetcher is an ASP.NET Core Web API that retrieves cat images and metadata from [The Cat API](https://thecatapi.com), stores them in a database, and allows filtering by tags. It's designed to demonstrate RESTful API development with Entity Framework Core and integration with third-party APIs.

---

## Features

- Fetch and store cats from The Cat API
- Filter cats by tags
- Basic data validation
- Swagger documentation
- Unit and integration tests
- Docker support

---

## Technologies Used

- ASP.NET Core 8
- Entity Framework Core
- SQL Server (or SQLite for development)
- xUnit & FluentAssertions for testing
- Swagger/OpenAPI

---

## Setup Instructions

### 1. Clone the repository

git clone https://github.com/geo2bοu/CatFetcherAPI.git
cd CatFetcherAPI

### 2. Set your Cat API Key:

"CatApi": {
  "BaseUrl": "https://api.thecatapi.com/v1",
  "ApiKey": "YOUR_API_KEY"
}

### 3. Set up the database:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CatFetcherDb;User Id=your_id;Password=your_password;Trusted_Connection=True;TrustServerCertificate=True;"
}

### 4. Run the migrations:

dotnet ef database update

### 5. Run the app:

dotnet run
