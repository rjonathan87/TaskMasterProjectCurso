#!/bin/bash
set -e

# Wait for the database to be ready
./wait-for-db.sh db "$SA_PASSWORD" -- echo "SQL Server is ready"

# Apply database migrations


# Start the application
>&2 echo "Starting application..."
dotnet TaskMaster.Presentation.dll
