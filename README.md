# Wynncraft Website Production Handoff Checklist

This checklist is for handing the project to someone else to host and run in production with their own domain.

## 1) Prereqs on the host

- Docker + Docker Compose installed

- Ports 80 (and 443 if adding TLS) open to the server

- A public domain with DNS control

- SSMS (or equivalent to import via .bacpac) OR sqlpackage installed.

- If using HTTPS, ensure certs are provisioned and 443 is open.

## 2) DNS setup

Create DNS records pointing to the server IP:

- A/AAAA: `yourdomain.com`

- A/AAAA: `auth.yourdomain.com`

## 3) Create secrets and env files

From the repo root, copy example files and set real values:

MAC OS/Linux

```bash

cp .env.example .env

cp RabbitMQ/secrets/rabbitmq_admin_password.example.txt RabbitMQ/secrets/rabbitmq_admin_password.txt
cp RabbitMQ/secrets/rabbitmq_website_password.example.txt RabbitMQ/secrets/rabbitmq_website_password.txt
cp RabbitMQ/secrets/rabbitmq_raidbot_password.example.txt RabbitMQ/secrets/rabbitmq_raidbot_password.txt
cp secrets/rabbitmq_website_password.example.txt secrets/rabbitmq_website_password.txt
cp secrets/sqlserver_connection_string.example.txt secrets/sqlserver_connection_string.txt

```

Windows:

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

Populate values in `.env`:

- `FRONTEND_URL` = `http://yourdomain.com`

- `BACKEND_URL` = `http://yourdomain.com` or `http://api.yourdomain.com` if you add a separate API domain

- `AUTHENTIK_URL` = `http://auth.yourdomain.com`

- `AUTHENTIK_ISSUER_PATH` = `http://auth.yourdomain.com/application/o/wynncraft-web/`

- `AUTHENTIK_INTERNAL_URL` = `http://authentik-server:9000`

- `BACKEND_INTERNAL_URL` = `http://backend:5032`

- `CLIENT_ID` and `CLIENT_SECRET` will be created in Authentik later

- SQL Server: set `MSSQL_SA_PASSWORD` (this will be your `sa` password, not what the website will use)

- RabbitMQ: set `RABBITMQ_HOST`, `RABBITMQ_VHOST`, `RABBITMQ_USERNAME` if deviating from defaults

#### Note:

https://yourdomain.com and https://auth.yourdomain.com as the expected endpoints when TLS is on.

Populate secrets:

- `secrets/sqlserver_connection_string.txt` (SQL Server connection string)

- `secrets/rabbitmq_website_password.txt` (RabbitMQ password used by the backend)

- RabbitMQ admin/raidbot secrets in `RabbitMQ/secrets/`

## 4) Run RabbitMQ first (creates the network)

RabbitMQ compose creates the `wynncraft-guild-net` network used by the rest of the stack.

```bash

docker compose -f RabbitMQ/docker-compose.rabbitmq.yml up

```

Confirm the network exists:

```bash

docker network ls | rg wynncraft-guild-net

```

## 5) Import Database (BACPAC)

### 5a — Import the BACPAC (using the admin credentials you set in the `.env` file)

You **must** use an admin-level login to import a bacpac.

Examples (pick one):

- **Azure Data Studio**
- **SSMS**
- **sqlpackage**
- **sqlcmd wrapper**

Example with `sqlpackage`:

```
sqlpackage \
  /Action:Import \
  /SourceFile:/home/<path-to-your-file>/your-export.bacpac \
  /TargetServerName:localhost,1433 \
  /TargetDatabaseName:WynncraftDB \
  /TargetUser:sa \
  /TargetPassword:'YOUR_SA_PASSWORD' \
  /TargetTrustServerCertificate:True
```

After this step:

- Database exists
- Schema and data imported
- Application login does not exist yet (this is expected)

---

### 5b — Create the application database user (one-time)

After importing the BACPAC, you must create the application login used by the website.

This is done by running one SQL script using administrator credentials (`sa`).

This script:

- creates the server login
- updates its password if re-run
- maps it to the database
- grants least-privilege access

🔐 Credentials used

Still `sa`
(This is the last time `sa` is used unless something breaks.)

📄 Script to run

Open SSMS / Azure Data Studio / VSC SQL Server (mssql) extension, connect as `sa`, then run: \*need way via cmd

```sql
-- ============================================================
-- Wynncraft Website - Application Login Setup
-- Run once as SQL Server administrator (`sa`)
-- Safe to re-run
-- ============================================================

-- Create or update server login
IF NOT EXISTS (
    SELECT 1 FROM sys.server_principals WHERE name = N'wynncraft_app'
)
BEGIN
    CREATE LOGIN [wynncraft_app]
        WITH PASSWORD = 'STRONG_RANDOM_PASSWORD_HERE';
END
ELSE
BEGIN
    ALTER LOGIN [wynncraft_app]
        WITH PASSWORD = 'STRONG_RANDOM_PASSWORD_HERE';
END
GO

USE [WynncraftDB];
GO

-- Create database user
IF NOT EXISTS (
    SELECT 1 FROM sys.database_principals WHERE name = N'wynncraft_app'
)
BEGIN
    CREATE USER [wynncraft_app]
        FOR LOGIN [wynncraft_app];
END
GO

-- Grant least-privilege access
ALTER ROLE db_datareader ADD MEMBER [wynncraft_app];
ALTER ROLE db_datawriter ADD MEMBER [wynncraft_app];
GO
```

### What this user can do

| Capability             | Allowed |
| ---------------------- | ------- |
| Read data              | ✅      |
| Write data             | ✅      |
| Alter schema           | ❌      |
| Drop tables            | ❌      |
| Access other databases | ❌      |
| Administer SQL Server  | ❌      |

This is the account used by the website at runtime.

### 5c — Configure runtime database access

Create the backend SQL connection string using the wynncraft_app credentials:

`Server=db,1433; Database=WynncraftDB; User Id=wynncraft_app; Password=STRONG_RANDOM_PASSWORD_HERE; Encrypt=False; TrustServerCertificate=True;`

Put this into:

secrets/sqlserver_connection_string.txt

This file is mounted as a Docker secret and read by the backend at startup.

Credentials used from this point forward:

✅ wynncraft_app

⚠️ Important note
If you change the wynncraft_app password in SQL Server later, you must also update the SQL connection string secret to match.

Next run the databse container:

```bash
docker compose -f docker-compose.prod.yml up --build db
```

The app connects using `wynncraft_app`.
The backend container will fail to start if the password does not match.

## 6) Update frontend Nginx hostnames

Edit `Frontend/nginx.conf` and replace:

- `server_name wynncraft.local;` with your real domain

- `server_name auth.wynncraft.local;` with `auth.yourdomain.com`

- `set $authentik_backend ...` if the container hostname differs (default `authentik-server:9000` works with the prod compose)

## 7) Authentik initial setup (Refer to Authentik Setup Packet)

See `Authentik-Setup.md` for the full Authentik configuration steps.

1. Copy `CLIENT_ID` / `CLIENT_SECRET` into `.env` and start the stack:

```bash

docker compose -f docker-compose.prod.yml up --build

```

## 8) Database migrations

There is no automatic EF Core migration on boot. Apply migrations before use.

Options:

- Run migrations from a dev machine using the same connection string.

- Or exec into the backend container and run `dotnet ef database update` (requires SDK/tools in image).

## 9) Verify

- Frontend: `https://yourdomain.com`

- Authentik: `https://auth.yourdomain.com`

- `docker-compose.prod.yml` expects the `wynncraft-guild-net` network created by the RabbitMQ compose file.

- The backend requires RabbitMQ at runtime for the raids consumer.

- Update `.env` URLs any time the domain or hostnames change.

## Dev troubleshooting

### If there is an issue with the frontend crashing in production due to not installing node modules try adding the following to your frontend volumes:

```
- /app/node_modules
```

### If Authentik reports `Role "authentik" does not exist`, the Postgres data directory was initialized with different credentials. You must delete `./pgdata` so Postgres can re-initialize:

Mac OS/Linux:

```bash
docker compose -f docker-compose.dev.yml down
sudo rm -rf pgdata
docker compose -f docker-compose.dev.yml up -d
```

Windows (PowerShell):

```powershell
docker compose -f docker-compose.dev.yml down
Remove-Item -Recurse -Force pgdata
docker compose -f docker-compose.dev.yml up -d
```

Populate values in `.env`:
