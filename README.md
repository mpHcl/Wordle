# Wordle

## 1. Requirements: 
- .NET 9.0 SDK or later

## 2. Setup:
- Clone the repository
- Open .SLN file in Visual Studio or your preferred IDE
- Restore NuGet packages
- Build the solution
- Update database (project uses SQLite by deafult, data will be stored in file /Server/wordle.db)
```cmd
# run in Server directory
dotnet ef database update 
```

## 3. Running the Application:

