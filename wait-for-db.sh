#!/bin/bash
# wait-for-db.sh

set -e

host="$1"
password="$2"
shift 2
cmd="$@"

until echo "select 1" | /opt/mssql-tools/bin/sqlcmd -S $host -U sa -P $password; do
  >&2 echo "SQL Server is unavailable - sleeping"
  sleep 1
done

>&2 echo "SQL Server is up - executing command"
exec $cmd
