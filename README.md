# CatFetcherAPI

Web API σε ASP.NET Core 8 που κατεβάζει 25 εικόνες γατών από το [TheCatAPI](https://thecatapi.com/) και τις αποθηκεύει σε βάση SQL Server με tags και metadata.

## Προαπαιτούμενα

- .NET 8 SDK
- Visual Studio 2022
- SQL Server
- API Key από [TheCatAPI](https://thecatapi.com/)

## Οδηγίες Εκκίνησης

1. Κλωνοποίησε το repo:

```bash
git clone https://github.com/yourusername/CatFetcherAPI.git
cd CatFetcherAPI

2. Πρόσθεσε το API key σου στο appsettings.json:

"CatApi": {
  "BaseUrl": "https://api.thecatapi.com/v1",
  "ApiKey": "YOUR_API_KEY"
}

3. Όρισε connection string για SQL Server:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=CatFetcherDb;User Id=your_id;Password=your_password;Trusted_Connection=True;TrustServerCertificate=True;"
}

4. Κάνε migration:

dotnet ef database update

5. Τρέξε το project:

dotnet run
