# Imperial Backend & Frontend

## Overview
This project is a full-stack application for managing and displaying Imperial guild member data, built with:
- **Backend:** ASP.NET Core 9, Entity Framework Core, OData
- **Frontend:** React 19, TypeScript, Material UI, rsbuild, pnpm

## Features
- Guild member list, leaderboard, and profile pages
- Material UI DataGrid with sorting, searching, and custom theming
- API endpoints for guild members, ranks, games, medals, and stats
- Proxy setup for frontend to backend API calls
- Modern Imperial color palette and UI

## Project Structure
```
Imperial Backend/
├── ImperialBackend/         # .NET backend
│   ├── Controllers/         # API controllers
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

### Backend (.NET)
1. Install [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
2. Navigate to `ImperialBackend` folder
3. Run database migrations:
   ```sh
   dotnet ef database update
   ```
4. Start the backend server:
   ```sh
   dotnet run
   ```

### Frontend (React)
1. Install [pnpm](https://pnpm.io/)
2. Navigate to `ImperialFrontend` folder
3. Install dependencies:
   ```sh
   pnpm install
   ```
4. Start the frontend dev server:
   ```sh
   pnpm run dev
   ```

## API Endpoints
- `/odata/GuildMembers` - List all guild members
- `/odata/GuildMembers/Leaderboard` - Leaderboard stats
- `/odata/GuildMembers/Profile/{id}` - Member profile
- `/odata/Ranks` - List ranks
- `/odata/Games` - List games
- `/odata/Medals` - List medals

## Customization
- Colors and theme: Edit `ImperialFrontend/src/theme.ts`
- Proxy/API: Edit `ImperialFrontend/rsbuild.config.ts`
- Table columns: Edit `ImperialFrontend/src/pages/GuildMemberList.tsx`

## Contributing
1. Fork the repo
2. Create a feature branch
3. Commit and push your changes
4. Open a pull request

## License
MIT
