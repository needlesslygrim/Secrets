# Secrets
A project I did for school. It isn't very good, as I've not done any web development in the past.
## Architecture
It's built using Blazor Webassembly on the frontend, since I don't know JS but do know C#. The backend is using ASP.NET Core, and is a minimal API. It connects to a MySQL database, I'm using MariaDB since it's the default on Arch.
## Running
In its current configuration this project would not work with on other machines unless they were set up identically to mine. You would have to change the file path in Secrets.API/Program.cs to a different place, and put in the correct contents. Other than that it should just be:
- ```git clone "https://github.com/DivineBicycle/Secrets.git"```
- Open in JetBrains Rider
- Run normally

Or:
- ```git clone "https://github.com/DivineBicycle/Secrets.git"```
- ```dotnet run``` in both the Secrets.Web and Secrets.API directories

