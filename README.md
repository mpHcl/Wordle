# Wordle

## Description 
Wordle is a simple word game inspired by NY Times game https://www.nytimes.com/games/wordle/index.html
Goal of this game is to guess a 5 letter word in max 5 attempts. 

// Add screenshot

### Functionalities
Each game consist of 
- Guessing 5 letter word in max 6 tries
- Colorful indicators: 
	- <b style="color: green;">GREEN</b> - letter is guessed in correct place 
	- <b style="color: orange;">ORANGE</b> - letter is guessed, but in other placement 
	- <b style="color: gray;">GRAY</b> - letter is not in the word to guess

This project have two types of games: 
- Daily challenge 
- Free play

## Technologies
### Architecure
Project consist of 3 modules 
- Client - web assembly client 
- Server - API for database integration with game
- Shared - DTOs for communication 

### Technologies
- Blazor Web Assemlby (HTML + C# + CSS) 
- ASP.NET web API
- LINQ 
- EntityFramework 
- SQLite
- Identity



## Installation
### Requirements
- .NET 9.0 SDK or later

### Setup:
- Clone the repository
- Open .SLN file in Visual Studio or your preferred IDE
- Restore NuGet packages
- Build the solution
- Update database (project uses SQLite by deafult, data will be stored in file /Server/wordle.db)
```cmd
# run in Server directory
dotnet ef database update 
```

### Running the application:

