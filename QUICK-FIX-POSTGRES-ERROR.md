# Quick Fix for PostgreSQL Initialization Error

## The Problem
```
postgres-container-1  | initdb: error: directory "/var/lib/postgresql/data" exists but is not empty
postgres-container-1 exited with code 1
```

## Immediate Fix (Choose One)

### Option 1: Clean Start (Quickest)
```powershell
# Stop all containers
docker-compose -f .\docker.compose.yaml down -v

# Remove local data directories
Remove-Item -Recurse -Force .\postgres-init -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force .\mysql-init -ErrorAction SilentlyContinue

# Start again
docker-compose -f .\docker.compose.yaml up -d
```

### Option 2: Use the Fixed Docker Compose File
I've created a fixed version at `docker-compose.fixed.yaml`. Use it:

```powershell
# Stop current containers
docker-compose -f .\docker.compose.yaml down -v

# Use the fixed file
docker-compose -f .\docker-compose.fixed.yaml up -d

# View logs
docker-compose -f .\docker-compose.fixed.yaml logs -f
```

### Option 3: Update Your Current File
Replace your `docker.compose.yaml` with this content:

```yaml
services:
  mysql-container:
    image: mysql:8.3.0
    container_name: mysql-container
    environment:
      MYSQL_ROOT_PASSWORD: admin
      MYSQL_DATABASE: ecommerceProducts
    ports:
      - "3307:3306"
    volumes:
      - mysql-data:/var/lib/mysql  # Changed from ./mysql-init
    networks:
      - ecommerce-network
    healthcheck:
      test: ["CMD", "mysqladmin", "ping", "-h", "localhost"]
      timeout: 5s
      retries: 10

  products-microservice:
    image: 900325302/ecommerce-products-microservice:v1.0
    container_name: products-microservice
    environment:
      MYSQL_HOST: mysql-container
      MYSQL_PORT: 3306
      MYSQL_PASSWORD: admin
    ports:
      - "8082:8080"
    networks:
      - ecommerce-network
    depends_on:
      mysql-container:
        condition: service_healthy

  postgres-container:
    image: postgres:16.1
    container_name: postgres-container
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: admin
      POSTGRES_DB: ecommerceUsers
    ports:
      - "5433:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data  # Changed from ./postgres-init
    networks:
      - ecommerce-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U postgres"]
      interval: 5s
      timeout: 5s
      retries: 10

  users-microservice:
    image: 900325302/ecommerce-users-microservice:v1
    container_name: users-microservice
    environment:
      POSTGRES_HOST: postgres-container
      POSTGRES_PORT: 5432
      POSTGRES_PASSWORD: admin
      POSTGRES_DB: ecommerceUsers
      POSTGRES_USER: postgres
    ports:
      - "9090:9090"
    networks:
      - ecommerce-network
    depends_on:
      postgres-container:
        condition: service_healthy

volumes:
  mysql-data:
    driver: local
  postgres-data:
    driver: local

networks:
  ecommerce-network:
    driver: bridge
```

Then run:
```powershell
docker-compose -f .\docker.compose.yaml up -d
```

## Key Changes Made

1. ✅ **Removed `version: '3.8'`** - No longer needed
2. ✅ **Changed from bind mounts to named volumes**:
   - `./postgres-init:/var/lib/postgresql/data` → `postgres-data:/var/lib/postgresql/data`
   - `./mysql-init:/docker-entrypoint-initdb.d` → `mysql-data:/var/lib/mysql`
3. ✅ **Added health checks** - Ensures databases are ready before microservices start
4. ✅ **Added `depends_on` with health conditions** - Proper startup order
5. ✅ **Fixed environment variables** - Removed spaces after `=`
6. ✅ **Added container names** - Easier management

## Verify It's Working

```powershell
# Check all containers are running
docker-compose -f .\docker.compose.yaml ps

# View logs
docker-compose -f .\docker.compose.yaml logs -f

# Test PostgreSQL
docker exec -it postgres-container pg_isready -U postgres

# Test MySQL
docker exec -it mysql-container mysqladmin ping -h localhost -uroot -padmin
```

## Still Having Issues?

```powershell
# Complete nuclear option - removes everything
docker-compose -f .\docker.compose.yaml down -v --rmi all
docker system prune -a --volumes -f

# Remove local directories
Remove-Item -Recurse -Force .\postgres-init -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force .\mysql-init -ErrorAction SilentlyContinue

# Start fresh
docker-compose -f .\docker.compose.yaml up -d
```

## Access Your Services

- **Users Microservice**: http://localhost:9090
- **Products Microservice**: http://localhost:8082
- **PostgreSQL**: localhost:5433 (username: postgres, password: admin)
- **MySQL**: localhost:3307 (username: root, password: admin)
