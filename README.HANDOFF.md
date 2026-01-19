# Imperial Website Production Handoff Checklist

This checklist is for handing the project to someone else to host and run in production with their own domain.

## 1) Prereqs on the host

- Docker + Docker Compose installed

- Ports 80 (and 443 if adding TLS) open to the server

- A public domain with DNS control

- SSMS (or equivalent to import via .bacpac) OR sqlpackage installed.

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

If there is an issue with the frontend crashing due to not installing node modules try adding the following to your frontend volues:

```
- /app/node_modules
```

Populate values in `.env`:

- `FRONTEND_URL` = `http://yourdomain.com`

- `BACKEND_URL` = `http://yourdomain.com` or `http://api.yourdomain.com` if you add a separate API domain

- `AUTHENTIK_URL` = `http://auth.yourdomain.com`

- `AUTHENTIK_ISSUER_PATH` = `http://auth.yourdomain.com/application/o/imperial-web/`

- `AUTHENTIK_INTERNAL_URL` = `http://authentik-server:9000`

- `BACKEND_INTERNAL_URL` = `http://imperial_backend:5032`

- `CLIENT_ID` and `CLIENT_SECRET` will be created in Authentik later

- SQL Server: set `MSSQL_SA_PASSWORD` (this will be your sa password, not what the website will use)

- RabbitMQ: set `RABBITMQ_HOST`, `RABBITMQ_VHOST`, `RABBITMQ_USERNAME` if deviating from defaults

Populate secrets:

- `secrets/sqlserver_connection_string.txt` (SQL Server connection string)

- `secrets/rabbitmq_website_password.txt` (RabbitMQ password used by the backend)

- RabbitMQ admin/raidbot secrets in `RabbitMQ/secrets/`

## 4) Run RabbitMQ first (creates the network)

RabbitMQ compose creates the `imperial-net` network used by the rest of the stack.

```bash

docker compose -f RabbitMQ/docker-compose.rabbitmq.yml up

```

Confirm the network exists:

```bash

docker network ls | rg imperial-net

```

## 4a) Import Database (BACPAC)

# STEP-BY-STEP: After you export the BACPAC

## Step 1 — Import the BACPAC (using the admin credentials you set in the `.env` file)

You **must** use an admin-level login to import a bacpac. T

import it “like normal”\*\*.

Examples (pick one):

- **Azure Data Studio**
- **SSMS**
- **sqlpackage**
- **sqlcmd wrapper**

Example with `sqlpackage`:

```
sqlpackage \
  /Action:Import \
  /SourceFile:/home/<path-to-your-file>/Imperial_Db_New.bacpac \
  /TargetServerName:localhost,1433 \
  /TargetDatabaseName:ImperialDb_New \
  /TargetUser:sa \
  /TargetPassword:'YOUR_SA_PASSWORD' \
  /TargetTrustServerCertificate:True
```

---

## Step 2 — Create the app user (still admin-only, one-time)

Now you immediately run **one SQL script** using **admin credentials** (`sa`).

This script:

- creates a **server login**
- maps it to a **database user**
- grants **least privilege**

### 🔐 Credentials used

Still **`sa`**  
(This is the _last time_ you’ll use it unless something breaks.)

### 📄 Script to run (copy/paste safe)

```
-- Create server login (run at server scope)
IF NOT EXISTS (
    SELECT 1 FROM sys.server_principals WHERE name = N'imperial_app'
)
BEGIN
    CREATE LOGIN [imperial_app]
    WITH PASSWORD = 'STRONG_RANDOM_PASSWORD_HERE';
END
GO

USE [ImperialDb_New];
GO

-- Create database user (if missing)
IF NOT EXISTS (
    SELECT 1 FROM sys.database_principals WHERE name = N'imperial_app'
)
BEGIN
    CREATE USER [imperial_app]
    FOR LOGIN [imperial_app];
END
GO

-- Grant least-privilege access
ALTER ROLE db_datareader ADD MEMBER [imperial_app];
ALTER ROLE db_datawriter ADD MEMBER [imperial_app];
GO
```

### What this user can do

✅ Read/write data  
❌ Cannot change schema  
❌ Cannot drop tables  
❌ Cannot touch other databases  
❌ Cannot administer SQL Server

This is what your **website will use**.

---

## Step 3 — Lock in the runtime credentials (no admin from here on)

Now you create the **runtime connection string** for the backend:

`Server=db,1433; Database=ImperialDb_New; User Id=imperial_app; Password=STRONG_RANDOM_PASSWORD_HERE; Encrypt=False; TrustServerCertificate=True;`

Put this in:

- `sqlserver_connection_string.txt`
- Docker secret
- Whatever your backend already expects

### 🔐 Credentials used from now on

❌ NOT `sa`  
✅ ONLY `imperial_app`

`docker compose -f docker-compose.prod.yml up --build`

The app connects using `imperial_app`.

## 5) Update frontend Nginx hostnames

Edit `ImperialFrontend/nginx.conf` and replace:

- `server_name imperial.local;` with your real domain

- `server_name auth.imperial.local;` with `auth.yourdomain.com`

- `set $authentik_backend ...` if the container hostname differs (default `authentik-server:9000` works with the prod compose)

## 6) Start the production stack

```bash

docker compose -f docker-compose.prod.yml up --build

```

Services included:

- SQL Server (app data)

- Postgres + Redis (Authentik)

- Authentik server/worker

- Backend API

- Frontend (Nginx + static assets)

## 7) Authentik initial setup (Refer to Authentik Setup Packet)

1. Copy `CLIENT_ID` / `CLIENT_SECRET` into `.env` and restart the stack:

```bash

docker compose -f docker-compose.prod.yml up --build

```

## 8) Database migrations

There is no automatic EF Core migration on boot. Apply migrations before use.

Options:

- Run migrations from a dev machine using the same connection string.

- Or exec into the backend container and run `dotnet ef database update` (requires SDK/tools in image).

## 9) Verify

- Frontend: `http://yourdomain.com`

- Authentik: `http://auth.yourdomain.com`

- `docker-compose.prod.yml` expects the `imperial-net` network created by the RabbitMQ compose file.

- The backend requires RabbitMQ at runtime for the raids consumer.

- Update `.env` URLs any time the domain or hostnames change.
