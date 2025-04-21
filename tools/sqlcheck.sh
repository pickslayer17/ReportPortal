#!/bin/bash

for i in {1..60}; do
  echo "[$i] Checking SQL readiness..."
  /opt/mssql-tools/bin/sqlcmd -S sql -U sa -P StrongPassword123! -Q "SELECT 1" &>/dev/null && exit 0
  sleep 2
done

echo "SQL not ready in time"
exit 1