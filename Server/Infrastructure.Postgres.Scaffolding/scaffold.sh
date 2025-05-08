#!/bin/bash
# Load the environment variables from the .env file
set -a
source ../Startup/.env
set +a

# Scaffold for the deployed database
echo "Scaffolding for the deployed database..."
dotnet ef dbcontext scaffold "$DEPLOYED_CONN_STR_LOCAL" Npgsql.EntityFrameworkCore.PostgreSQL \
  --output-dir ../Core.Domain/Entities \
  --context-dir . \
  --context MyDbContext \
  --no-onconfiguring \
  --namespace Core.Domain.Entities \
  --context-namespace Infrastructure.Postgres.Scaffolding \
  --schema public \
  --no-pluralize \
  --force