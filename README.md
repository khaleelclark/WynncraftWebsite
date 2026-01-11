# Imperial Backend & Frontend

## Overview

This project is a full-stack application for managing and displaying Imperial guild member data, built with:

- **Database:** Microsoft SQL Server
- **Backend:** ASP.NET Core 9, Entity Framework Core, REST API
- **Frontend:** React 19, TypeScript, Material UI, rsbuild, pnpm

## Features

- Guild member list, leaderboard, and profile pages
- Material UI DataGrid with sorting, searching, and custom theming
- REST API endpoints for guild members, ranks, games, medals, stats, and raids completed
- All endpoints return snake_case fields for frontend compatibility
- Raids completed count included in guild member and profile views
- Proxy setup for frontend to backend API calls
- Modern Imperial color palette and UI

## Project Structure

From the project root run the docker container using the following

```
docker compose -f docker-compose.dev.yml up --build
```

To copy secret files and .env file, do the following and edit them with the correct values

Initial Setup (Secrets & Environment Files)

Before running the project, you need to copy the example secret files and .env file, then edit them with the correct values.

🐧 Linux / macOS (Terminal)

```
cp .env.example .env

cp RabbitMQ/secrets/rabbitmq_admin_password.example.txt \
   RabbitMQ/secrets/rabbitmq_admin_password.txt

cp RabbitMQ/secrets/rabbitmq_website_password.example.txt \
   RabbitMQ/secrets/rabbitmq_website_password.txt

cp RabbitMQ/secrets/rabbitmq_raidbot_password.example.txt \
   RabbitMQ/secrets/rabbitmq_raidbot_password.txt

cp secrets/rabbitmq_website_password.example.txt \
   secrets/rabbitmq_website_password.txt

cp secrets/sqlserver_connection_string.example.txt \
   secrets/sqlserver_connection_string.txt
```

🪟 Windows (PowerShell — recommended)

Run these commands from the project root:

```
Copy-Item ".env.example" ".env"

Copy-Item "RabbitMQ/secrets/rabbitmq_admin_password.example.txt"   "RabbitMQ/secrets/rabbitmq_admin_password.txt"
Copy-Item "RabbitMQ/secrets/rabbitmq_website_password.example.txt" "RabbitMQ/secrets/rabbitmq_website_password.txt"
Copy-Item "RabbitMQ/secrets/rabbitmq_raidbot_password.example.txt" "RabbitMQ/secrets/rabbitmq_raidbot_password.txt"

Copy-Item "secrets/rabbitmq_website_password.example.txt"          "secrets/rabbitmq_website_password.txt"
Copy-Item "secrets/sqlserver_connection_string.example.txt"        "secrets/sqlserver_connection_string.txt"
```

💡 If files already exist and you want to overwrite them, add -Force to Copy-Item.

```
Imperial Backend/
├── ImperialBackend/         # .NET backend
│   ├── Controllers/         # API controllers
│   ├── DTOs/                # Request/response contracts
│   ├── Models/              # Entity models
│   ├── Migrations/          # EF Core migrations
│   ├── Services/            # Business logic
│   ├── appsettings.json     # Configuration
│   └── ...
├── ImperialFrontend/        # React frontend
│   ├── src/pages/           # Main pages (GuildMemberList, Leaderboard, Profile)
│   ├── theme.ts             # Custom MUI theme
│   ├── rsbuild.config.ts    # Proxy config
│   ├── package.json         # Frontend dependencies
│   └── ...
├── .gitignore               # Ignore build, env, and IDE files
└── README.md                # Project documentation
```

## Getting Started

## API Endpoints

- `/api/guildmembers` - List all guild members (snake_case fields)
- `/api/guildmembers/leaderboard?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD` - Leaderboard stats (diffs, raids completed)
- `/api/guildmembers/{id}` - Member profile (all fields, raids completed)
- `/api/ranks` - List ranks
- `/api/games` - List games
- `/api/medals` - List medals
- `/api/raidscompleted` - Raids completed table

## Customization

- Colors and theme: Edit `ImperialFrontend/src/theme.ts`
- Proxy/API: Edit `ImperialFrontend/rsbuild.config.ts`
- Table columns: Edit `ImperialFrontend/src/pages/GuildMemberList.tsx`
- Profile fields: Edit `ImperialFrontend/src/pages/Profile.tsx`
- Leaderboard logic: Edit `ImperialFrontend/src/pages/Leaderboard.tsx` and backend controller

## Contributing

1. Fork the repo
2. Create a feature branch
3. Commit and push your changes
4. Open a pull request

## License

MIT
