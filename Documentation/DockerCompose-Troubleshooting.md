# Docker Compose Troubleshooting Guide

## Common Issues and Solutions

### Issue 1: PostgreSQL Initialization Error

**Error Message:**
```
postgres-container-1  | initdb: error: directory "/var/lib/postgresql/data" exists but is not empty
postgres-container-1  | initdb: hint: If you want to create a new database system, either remove or empty the directory "/var/lib/postgresql/data" or run initdb with an argument other than "/var/lib/postgresql/data".
postgres-container-1 exited with code 1
```

**Root Cause:**
The PostgreSQL container is trying to initialize a database in a directory that already contains data from a previous run. This happens when you use a local volume mount (`./postgres-init:/var/lib/postgresql/data`) that persists between container restarts.

**Solutions:**

#### Solution 1: Remove the Local Data Directory (Recommended for Development)
```powershell
# Stop all running containers
docker-compose -f .\docker.compose.yaml down

# Remove the postgres data directory
Remove-Item -Recurse -Force .\postgres-init

# Remove the mysql data directory (if needed)
Remove-Item -Recurse -Force .\mysql-init

# Start containers again
docker-compose -f .\docker.compose.yaml up
```

#### Solution 2: Use Docker Named Volumes (Recommended for Production)
Update your `docker.compose.yaml` to use named volumes instead of bind mounts:

```yaml
version: '3.8'

services:
  mysql-container:
    image: mysql:8.3.0
    environment:
      - MYSQL_ROOT_PASSWORD=admin
    ports:
      - "3307:3306"
    volumes:
      - mysql-data:/var/lib/mysql  # Changed to named volume
    networks:
      - ecommerce-network
    hostname: mysql-host-productsmicroservice

  products-microservice:
    image: 900325302/ecommerce-products-microservice:v1.0
    environment:
      - MYSQL_HOST=mysql-host-productsmicroservice
      - MYSQL_PORT=3306
      - MYSQL_PASSWORD=admin
    ports:
      - "8082:8080"
    networks:
      - ecommerce-network
    depends_on:
      - mysql-container
  
  postgres-container:
    image: postgres:16.1
    environment:
      - POSTGRES_USER=postgres
      - POSTGRES_PASSWORD=admin
      - POSTGRES_DB=ecommerceUsers
    ports:
      - "5433:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data  # Changed to named volume
    networks:
      - ecommerce-network
    hostname: postgres-host-usersmicroservice

  users-microservice:
    image: 900325302/ecommerce-users-microservice:v1
    environment:
      - POSTGRES_HOST=postgres-container
      - POSTGRES_PASSWORD=admin
      - POSTGRES_DB=ecommerceUsers
      - POSTGRES_USER=postgres
    ports:
      - "9090:9090"
    networks:
      - ecommerce-network
    depends_on:
      - postgres-container

volumes:
  mysql-data:      # Named volume for MySQL
  postgres-data:   # Named volume for PostgreSQL

networks:
  ecommerce-network:
    driver: bridge
```

**Key Changes:**
1. Removed `./postgres-init:/var/lib/postgresql/data` bind mount
2. Changed to named volume `postgres-data:/var/lib/postgresql/data`
3. Added `depends_on` to ensure databases start before microservices
4. Fixed hostname for postgres from `postgres-host-productsmicroservice` to `postgres-host-usersmicroservice`
5. Removed space after `=` in `MYSQL_ROOT_PASSWORD=admin`

#### Solution 3: Clean Start with Volume Removal
```powershell
# Stop and remove all containers, networks, and volumes
docker-compose -f .\docker.compose.yaml down -v

# Start fresh
docker-compose -f .\docker.compose.yaml up
```

---

### Issue 2: Version Attribute Warning

**Warning Message:**
```
level=warning msg="C:\\eCommerce\\docker.compose.yaml: the attribute `version` is obsolete"
```

**Solution:**
Remove the `version: '3.8'` line from your docker-compose.yaml. It's no longer needed in newer versions of Docker Compose.

**Updated File:**
```yaml
services:  # Start directly with services
  mysql-container:
    image: mysql:8.3.0
    # ... rest of configuration
```

---

### Issue 3: Database Connection Issues

**Symptoms:**
- Microservices can't connect to databases
- Connection timeout errors
- "Unknown host" errors

**Solutions:**

#### Check Container Networking
```powershell
# List networks
docker network ls

# Inspect the network
docker network inspect ecommerce_ecommerce-network

# Check if containers are in the same network
docker inspect postgres-container-1 | findstr NetworkMode
docker inspect users-microservice-1 | findstr NetworkMode
```

#### Verify Database is Ready
```powershell
# Check PostgreSQL logs
docker logs postgres-container-1

# Check MySQL logs
docker logs mysql-container-1

# Test database connection from microservice
docker exec -it users-microservice-1 ping postgres-container
```

#### Fix Hostname Configuration
Make sure your microservice environment variables match the container names:

```yaml
users-microservice:
  environment:
    - POSTGRES_HOST=postgres-container  # Must match service name
    # NOT postgres-host-usersmicroservice (that's just the hostname inside the container)
```

---

### Issue 4: Port Conflicts

**Error Message:**
```
Error response from daemon: Ports are not available: exposing port TCP 0.0.0.0:5432 -> 0.0.0.0:0: listen tcp 0.0.0.0:5432: bind: An attempt was made to access a socket in a way forbidden by its access permissions.
```

**Solution:**
```powershell
# Check what's using the port
netstat -ano | findstr :5432

# Kill the process (if safe to do so)
taskkill /PID <process_id> /F

# Or change the port in docker-compose.yaml
ports:
  - "5434:5432"  # Changed from 5433 to 5434
```

---

### Issue 5: Container Exits Immediately

**Symptoms:**
- Container starts but exits with code 1
- No logs or minimal logs

**Diagnosis:**
```powershell
# Check container status
docker-compose -f .\docker.compose.yaml ps

# View container logs
docker-compose -f .\docker.compose.yaml logs postgres-container

# Check last 50 lines
docker-compose -f .\docker.compose.yaml logs --tail=50 postgres-container
```

**Common Causes:**
1. Invalid environment variables
2. Missing required environment variables
3. Permission issues with volumes
4. Corrupted data in volumes

**Solution:**
```powershell
# Remove containers and volumes
docker-compose -f .\docker.compose.yaml down -v

# Remove local directories
Remove-Item -Recurse -Force .\postgres-init, .\mysql-init

# Start fresh
docker-compose -f .\docker.compose.yaml up
```

---

## Complete Fixed Docker Compose File

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
      - mysql-data:/var/lib/mysql
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
      MYSQL_DATABASE: ecommerceProducts
    ports:
      - "8082:8080"
    networks:
      - ecommerce-network
    depends_on:
      mysql-container:
        condition: service_healthy
    restart: on-failure

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
      - postgres-data:/var/lib/postgresql/data
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
    restart: on-failure

volumes:
  mysql-data:
    driver: local
  postgres-data:
    driver: local

networks:
  ecommerce-network:
    driver: bridge
```

**Key Improvements:**
1. ✅ Named volumes instead of bind mounts
2. ✅ Health checks for databases
3. ✅ `depends_on` with health check conditions
4. ✅ Container names for easier management
5. ✅ Restart policies
6. ✅ No version attribute
7. ✅ Proper environment variable format (no spaces)

---

## Useful Docker Compose Commands

```powershell
# Start services in detached mode
docker-compose -f .\docker.compose.yaml up -d

# Start and rebuild images
docker-compose -f .\docker.compose.yaml up -d --build

# Stop services
docker-compose -f .\docker.compose.yaml stop

# Stop and remove containers
docker-compose -f .\docker.compose.yaml down

# Stop and remove containers + volumes
docker-compose -f .\docker.compose.yaml down -v

# Stop and remove everything including images
docker-compose -f .\docker.compose.yaml down -v --rmi all

# View logs
docker-compose -f .\docker.compose.yaml logs -f

# View logs for specific service
docker-compose -f .\docker.compose.yaml logs -f postgres-container

# List running services
docker-compose -f .\docker.compose.yaml ps

# Restart a specific service
docker-compose -f .\docker.compose.yaml restart postgres-container

# Execute command in running container
docker-compose -f .\docker.compose.yaml exec postgres-container psql -U postgres -d ecommerceUsers

# Scale a service
docker-compose -f .\docker.compose.yaml up -d --scale products-microservice=3

# View resource usage
docker-compose -f .\docker.compose.yaml top
```

---

## Testing Database Connections

### Test PostgreSQL Connection
```powershell
# From host machine
docker exec -it postgres-container psql -U postgres -d ecommerceUsers

# From users-microservice container
docker exec -it users-microservice curl postgres-container:5432

# Test with pg_isready
docker exec -it postgres-container pg_isready -U postgres
```

### Test MySQL Connection
```powershell
# From host machine
docker exec -it mysql-container mysql -uroot -padmin

# List databases
docker exec -it mysql-container mysql -uroot -padmin -e "SHOW DATABASES;"

# Test connection
docker exec -it mysql-container mysqladmin ping -h localhost -uroot -padmin
```

---

## Quick Fix Commands

```powershell
# Complete clean start
docker-compose -f .\docker.compose.yaml down -v
Remove-Item -Recurse -Force .\postgres-init, .\mysql-init -ErrorAction SilentlyContinue
docker-compose -f .\docker.compose.yaml up -d

# View all logs
docker-compose -f .\docker.compose.yaml logs -f

# Check service health
docker-compose -f .\docker.compose.yaml ps

# Restart specific service
docker-compose -f .\docker.compose.yaml restart postgres-container
```

---

## Best Practices

1. ✅ Always use named volumes for databases in production
2. ✅ Add health checks for databases
3. ✅ Use `depends_on` with health check conditions
4. ✅ Use container names for easier management
5. ✅ Add restart policies for production
6. ✅ Never commit database volumes to git (add to `.gitignore`)
7. ✅ Use environment files (`.env`) for sensitive data
8. ✅ Remove the `version` attribute in docker-compose files
9. ✅ Use specific image tags, avoid `latest`
10. ✅ Monitor logs regularly during development
