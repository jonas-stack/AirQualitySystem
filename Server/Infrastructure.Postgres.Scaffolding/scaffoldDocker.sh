#!/bin/bash

# Load environment variables from .env file
set -a
source ../Startup/.env
set +a

# Scaffold for the Docker database
echo "Scaffolding for the Docker database..."
dotnet ef dbcontext scaffold "${DOCKER_CONN_STR}" Npgsql.EntityFrameworkCore.PostgreSQL \
  --output-dir ../Core.Domain/Entities \
  --context-dir . \
  --context MyDbContext \
  --no-onconfiguring \
  --no-pluralize \
  --namespace Core.Domain.Entities \
  --context-namespace Infrastructure.Postgres.Scaffolding \
  --schema public \
  --force