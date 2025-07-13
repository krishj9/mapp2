#!/bin/bash

# Test script for GCP Pub/Sub integration
echo "🚀 Testing MAPP Observations API with GCP Pub/Sub"
echo "================================================"

# Set the Google Cloud credentials
export GOOGLE_APPLICATION_CREDENTIALS="$(pwd)/mapp-dev-457512-6bfcf231ed81.json"

# Verify the service account key file exists
if [ ! -f "$GOOGLE_APPLICATION_CREDENTIALS" ]; then
    echo "❌ Error: Service account key file not found at $GOOGLE_APPLICATION_CREDENTIALS"
    exit 1
fi

echo "✅ Service account key file found: $GOOGLE_APPLICATION_CREDENTIALS"

# Set environment to enable Pub/Sub
export ASPNETCORE_ENVIRONMENT=PubSubTest
export ASPNETCORE_URLS="https://localhost:7001;http://localhost:7000"

echo "✅ Environment set to: $ASPNETCORE_ENVIRONMENT"
echo "✅ URLs configured: $ASPNETCORE_URLS"

# Navigate to the Observations service directory
cd src/Services/MAPP.Services.Observations

echo "🔄 Starting MAPP Observations API with GCP Pub/Sub enabled..."
echo "📍 Project: mapp-dev-457512"
echo "📍 Topic: projects/mapp-dev-457512/topics/observationcreated"
echo ""
echo "Press Ctrl+C to stop the service"
echo "================================================"

# Run the service
dotnet run
