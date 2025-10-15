#!/bin/bash
# entrypoint.sh

set -e

# Run the wait-for-db script
/app/wait-for-db.sh db 5432 -- echo "Database is ready"

# Apply EF Core migrations
>&2 echo "Applying database migrations..."
/usr/bin/dotnet TaskMaster.Presentation.dll --migrate || true

# Start the application
>&2 echo "Starting application..."
exec /usr/bin/dotnet TaskMaster.Presentation.dll