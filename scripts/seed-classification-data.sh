#!/bin/bash

# Script to seed classification data into the Observations service
# Usage: ./scripts/seed-classification-data.sh

set -e

echo "🌱 Seeding Classification Data..."

# Get the absolute path to the JSON file
JSON_FILE_PATH="/Users/krishnajammula/Development/MAPPV2/domainsattributesprogressionpoints.json"

# Check if JSON file exists
if [ ! -f "$JSON_FILE_PATH" ]; then
    echo "❌ Error: JSON file not found at $JSON_FILE_PATH"
    exit 1
fi

echo "📁 Using JSON file: $JSON_FILE_PATH"

# Default service URL (can be overridden)
SERVICE_URL="${OBSERVATIONS_SERVICE_URL:-http://localhost:5000}"

echo "🌐 Targeting service: $SERVICE_URL"

# Make the API call to seed the data
echo "📡 Calling seed endpoint..."

curl -X POST "$SERVICE_URL/api/api/observations/classifications/seed" \
  -H "Content-Type: application/json" \
  -d "{
    \"jsonFilePath\": \"$JSON_FILE_PATH\",
    \"overwriteExisting\": true
  }" \
  -w "\n%{http_code}\n" \
  -s

echo "✅ Seeding request completed!"
echo ""
echo "💡 To verify the data was seeded, you can call:"
echo "   curl $SERVICE_URL/api/api/observations/classifications"
