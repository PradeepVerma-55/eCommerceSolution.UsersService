# Docker Commands Guide

This document contains all the essential Docker commands for building, running, and managing the eCommerce User Microservice.

## Table of Contents
- [Essential Commands (Quick Start)](#essential-commands-quick-start)
- [Building Docker Images](#building-docker-images)
- [Tagging Images](#tagging-images)
- [Running Containers](#running-containers)
- [Managing Containers](#managing-containers)
- [Docker Compose](#docker-compose)
- [Cleanup Commands](#cleanup-commands)
- [Troubleshooting](#troubleshooting)

---

## Essential Commands (Quick Start)

These are the three most important commands for this project:

### 1. Build the Docker Image
```powershell
docker build -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile .
```
Builds the Docker image from the Dockerfile with tag `user-microservice:1.0`.

### 2. Tag for Docker Hub
```powershell
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:v1
```
Tags the local image for pushing to Docker Hub repository `900325302/ecommerce-users-microservice` with version `v1`.

### 3. Push to Docker Hub
```powershell
docker push 900325302/ecommerce-users-microservice:v1
```
Pushes the tagged image to Docker Hub (requires `docker login` first).

**Complete Workflow:**
```powershell
# Step 1: Build the image
docker build -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile .

# Step 2: Tag for Docker Hub
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:v1

# Step 3: Login to Docker Hub (if not already logged in)
docker login

# Step 4: Push to Docker Hub
docker push 900325302/ecommerce-users-microservice:v1
```

---

## Building Docker Images

### Build the User Microservice Image
```powershell
docker build -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile .
```

**Parameters Explanation:**
- `-t user-microservice:1.0` - Tags the image with name and version
- `-f .\eeCommerce.API\Dockerfile` - Specifies the Dockerfile location
- `.` - Sets the build context to the current directory

### Build with Different Tags
```powershell
# Build with latest tag
docker build -t user-microservice:latest -f .\eeCommerce.API\Dockerfile .

# Build with specific version
docker build -t user-microservice:1.1.0 -f .\eeCommerce.API\Dockerfile .

# Build without cache (clean build)
docker build --no-cache -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile .
```

### Build with Build Arguments
```powershell
docker build -t user-microservice:1.0 `
  --build-arg BUILD_CONFIGURATION=Release `
  -f .\eeCommerce.API\Dockerfile .
```

---

## Tagging Images

### Tag for Docker Hub
```powershell
# Tag the local image for Docker Hub
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:v1
```

**Tag Format:** `docker tag <source-image>:<tag> <registry-username>/<repository-name>:<version>`

### Multiple Tags for Same Image
```powershell
# Tag with different versions
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:v1
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:latest
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:1.0.0

# Tag for different environments
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:dev
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:staging
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:production
```

### Tag for Private Registry
```powershell
# Tag for private registry
docker tag user-microservice:1.0 myregistry.azurecr.io/ecommerce-users-microservice:v1

# Tag for AWS ECR
docker tag user-microservice:1.0 123456789012.dkr.ecr.us-east-1.amazonaws.com/ecommerce-users:v1
```

### List Image Tags
```powershell
# View all local images and their tags
docker images

# Filter by repository name
docker images 900325302/ecommerce-users-microservice

# View image with specific tag
docker images user-microservice:1.0
```

### Push Tagged Image to Docker Hub
```powershell
# Login to Docker Hub (required before first push)
docker login
# Username: 900325302
# Password: <your-docker-hub-password>

# Push the image
docker push 900325302/ecommerce-users-microservice:v1

# Push all tags of the repository
docker push --all-tags 900325302/ecommerce-users-microservice
```

### Pull Tagged Image from Docker Hub
```powershell
# Pull specific version
docker pull 900325302/ecommerce-users-microservice:v1

# Pull latest version
docker pull 900325302/ecommerce-users-microservice:latest
```

### Remove Tags
```powershell
# Remove specific tag (doesn't delete the image if other tags exist)
docker rmi 900325302/ecommerce-users-microservice:v1

# Remove all tags of an image
docker rmi user-microservice:1.0 900325302/ecommerce-users-microservice:v1
```

---

## Running Containers

### Run the User Microservice Container
```powershell
# Basic run
docker run -d -p 9090:9090 -p 9091:9091 --name user-service user-microservice:1.0

# Run with environment variables
docker run -d -p 9090:9090 -p 9091:9091 `
  --name user-service `
  -e POSTGRES_HOST=host.docker.internal `
  -e POSTGRES_PASSWORD=your_password `
  user-microservice:1.0

# Run with custom network
docker run -d -p 9090:9090 -p 9091:9091 `
  --name user-service `
  --network ecommerce-network `
  user-microservice:1.0
```

**Parameters Explanation:**
- `-d` - Run in detached mode (background)
- `-p 9090:9090` - Port mapping (host:container)
- `--name user-service` - Container name
- `-e` - Environment variable

### Run PostgreSQL Database Container
```powershell
docker run -d `
  --name postgres-db `
  -e POSTGRES_USER=admin `
  -e POSTGRES_PASSWORD=admin `
  -e POSTGRES_DB=ecommerce_users `
  -p 5432:5432 `
  -v postgres-data:/var/lib/postgresql/data `
  postgres:15
```

### Run with Interactive Mode (for debugging)
```powershell
# Run with interactive shell
docker run -it --rm -p 9090:9090 user-microservice:1.0 /bin/bash

# View logs in real-time
docker run -p 9090:9090 --name user-service user-microservice:1.0
```

---

## Managing Containers

### List Containers
```powershell
# List running containers
docker ps

# List all containers (including stopped)
docker ps -a

# List with formatting
docker ps --format "table {{.ID}}\t{{.Names}}\t{{.Status}}\t{{.Ports}}"
```

### Start/Stop Containers
```powershell
# Start container
docker start user-service

# Stop container
docker stop user-service

# Restart container
docker restart user-service

# Pause/Unpause container
docker pause user-service
docker unpause user-service
```

### View Logs
```powershell
# View logs
docker logs user-service

# Follow logs (real-time)
docker logs -f user-service

# View last 100 lines
docker logs --tail 100 user-service

# View logs with timestamps
docker logs -t user-service
```

### Execute Commands in Running Container
```powershell
# Open bash shell in container
docker exec -it user-service /bin/bash

# Run a specific command
docker exec user-service dotnet --version

# Check environment variables
docker exec user-service env
```

### Inspect Container
```powershell
# View detailed container information
docker inspect user-service

# View specific information (IP address)
docker inspect -f '{{range .NetworkSettings.Networks}}{{.IPAddress}}{{end}}' user-service

# View environment variables
docker inspect -f '{{.Config.Env}}' user-service
```

---

## Docker Compose

### Docker Compose File Example
Create a `docker-compose.yml` file in the project root:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:15
    container_name: postgres-db
    environment:
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: admin
      POSTGRES_DB: ecommerce_users
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data
    networks:
      - ecommerce-network

  user-service:
    build:
      context: .
      dockerfile: ./eeCommerce.API/Dockerfile
    container_name: user-service
    ports:
      - "9090:9090"
      - "9091:9091"
    environment:
      POSTGRES_HOST: postgres
      POSTGRES_PASSWORD: admin
    depends_on:
      - postgres
    networks:
      - ecommerce-network

volumes:
  postgres-data:

networks:
  ecommerce-network:
    driver: bridge
```

### Docker Compose Commands
```powershell
# Start all services
docker-compose up -d

# Start and rebuild images
docker-compose up -d --build

# Stop all services
docker-compose down

# Stop and remove volumes
docker-compose down -v

# View logs
docker-compose logs -f

# View logs for specific service
docker-compose logs -f user-service

# Scale services
docker-compose up -d --scale user-service=3
```

---

## Cleanup Commands

### Remove Containers
```powershell
# Remove stopped container
docker rm user-service

# Force remove running container
docker rm -f user-service

# Remove all stopped containers
docker container prune

# Remove all containers
docker rm -f $(docker ps -aq)
```

### Remove Images
```powershell
# Remove specific image
docker rmi user-microservice:1.0

# Force remove image
docker rmi -f user-microservice:1.0

# Remove all unused images
docker image prune

# Remove all images
docker rmi $(docker images -q)
```

### Remove Volumes
```powershell
# Remove specific volume
docker volume rm postgres-data

# Remove all unused volumes
docker volume prune

# List volumes
docker volume ls
```

### Remove Networks
```powershell
# Remove specific network
docker network rm ecommerce-network

# Remove all unused networks
docker network prune

# List networks
docker network ls
```

### Complete Cleanup
```powershell
# Remove all stopped containers, unused networks, dangling images, and build cache
docker system prune

# Remove everything (including volumes)
docker system prune -a --volumes
```

---

## Troubleshooting

### Check Docker Status
```powershell
# Check Docker version
docker --version
docker version

# Check Docker info
docker info

# Check running processes
docker ps
```

### Debugging Build Issues
```powershell
# Build with verbose output
docker build -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile . --progress=plain

# Build specific stage
docker build --target build -t user-microservice:build -f .\eeCommerce.API\Dockerfile .

# Check build history
docker history user-microservice:1.0
```

### Network Issues
```powershell
# List networks
docker network ls

# Inspect network
docker network inspect ecommerce-network

# Create network
docker network create ecommerce-network

# Connect container to network
docker network connect ecommerce-network user-service
```

### Port Conflicts
```powershell
# Check port usage (Windows)
netstat -ano | findstr :9090

# Kill process using port (Windows)
taskkill /PID <process_id> /F
```

### Container Health Check
```powershell
# Check container stats
docker stats user-service

# Check container processes
docker top user-service

# Check disk usage
docker system df
```

### Common Issues

#### Issue: "Port already in use"
```powershell
# Solution 1: Use different port
docker run -d -p 9095:9090 --name user-service user-microservice:1.0

# Solution 2: Stop conflicting container
docker stop $(docker ps -q --filter "publish=9090")
```

#### Issue: "Cannot connect to database"
```powershell
# Check if database container is running
docker ps | grep postgres

# Check database logs
docker logs postgres-db

# Verify network connectivity
docker exec user-service ping postgres
```

#### Issue: "File not found during build"
```powershell
# Check .dockerignore file
cat .dockerignore

# Build with no cache
docker build --no-cache -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile .
```

---

## Additional Resources

### View Resource Usage
```powershell
# Container resource usage
docker stats

# Disk usage by Docker
docker system df

# Detailed disk usage
docker system df -v
```

### Export/Import Images
```powershell
# Save image to tar file
docker save user-microservice:1.0 -o user-microservice.tar

# Load image from tar file
docker load -i user-microservice.tar

# Export container to tar
docker export user-service -o user-service.tar

# Import container
docker import user-service.tar
```

### Push to Docker Registry
```powershell
# Tag for registry
docker tag user-microservice:1.0 username/user-microservice:1.0

# Login to Docker Hub
docker login

# Push to registry
docker push username/user-microservice:1.0

# Pull from registry
docker pull username/user-microservice:1.0
```

---

## Quick Reference

### Most Common Commands
```powershell
# Build
docker build -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile .

# Tag for Docker Hub
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:v1

# Push to Docker Hub
docker push 900325302/ecommerce-users-microservice:v1

# Run
docker run -d -p 9090:9090 -p 9091:9091 --name user-service user-microservice:1.0

# Stop
docker stop user-service

# Start
docker start user-service

# Logs
docker logs -f user-service

# Remove
docker rm -f user-service

# Clean up
docker system prune -a
```

### Complete Build and Push Workflow
```powershell
# 1. Build the image
docker build -t user-microservice:1.0 -f .\eeCommerce.API\Dockerfile .

# 2. Tag for Docker Hub
docker tag user-microservice:1.0 900325302/ecommerce-users-microservice:v1

# 3. Login to Docker Hub (first time only)
docker login

# 4. Push to Docker Hub
docker push 900325302/ecommerce-users-microservice:v1

# 5. Pull from Docker Hub (on another machine)
docker pull 900325302/ecommerce-users-microservice:v1

# 6. Run the pulled image
docker run -d -p 9090:9090 -p 9091:9091 --name user-service 900325302/ecommerce-users-microservice:v1
```

---

## Environment Variables Reference

The following environment variables can be configured:

| Variable | Description | Default |
|----------|-------------|---------|
| `POSTGRES_HOST` | PostgreSQL host address | `localhost` |
| `POSTGRES_PASSWORD` | PostgreSQL password | `admin` |
| `ASPNETCORE_ENVIRONMENT` | Application environment | `Production` |
| `ASPNETCORE_URLS` | URLs the app listens on | `http://+:9090` |

### Setting Environment Variables
```powershell
# Using -e flag
docker run -d -p 9090:9090 `
  -e POSTGRES_HOST=db.example.com `
  -e POSTGRES_PASSWORD=securepass `
  -e ASPNETCORE_ENVIRONMENT=Development `
  --name user-service user-microservice:1.0

# Using --env-file
docker run -d -p 9090:9090 --env-file .env --name user-service user-microservice:1.0
```

---

## Notes
- Always use specific version tags in production (avoid `latest`)
- Use volumes for persistent data
- Use networks for container communication
- Monitor resource usage regularly
- Keep Docker and images updated
- Use `.dockerignore` to exclude unnecessary files from build context
