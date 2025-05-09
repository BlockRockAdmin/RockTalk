# RockTalk

RockTalk è un'applicazione di chat in tempo reale costruita con .NET 8 e Supabase. Utilizza Windows Forms per l'interfaccia utente e Supabase per la gestione di messaggi, broadcast e presence.

## Prerequisiti

- .NET 8 SDK  
- Supabase account  
- Visual Studio Code o altro editor  

## Installazione

1. Clona il repository:

   ```bash
   git clone https://github.com/your-username/RockTalk.git
   cd RockTalk
Configura le credenziali Supabase:

Crea un file appsettings.json con Supabase.Url e Supabase.Key.

Compila ed esegui:

bash
Copy code
dotnet restore
dotnet run
Oppure crea un eseguibile:

bash
Copy code
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
.\publish\RockTalk.exe
Funzionalità
Messaggistica in tempo reale con Supabase Realtime
Supporto per broadcast e presence (in sviluppo)
Interfaccia Windows Forms
Licenza
MIT License
