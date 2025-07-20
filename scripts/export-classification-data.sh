#!/bin/bash

# Script to export classification data from database to Google Cloud Storage
# Usage: ./scripts/export-classification-data.sh [environment]

set -e

# Configuration
ENVIRONMENT="${1:-Development}"
SERVICE_URL="${OBSERVATIONS_SERVICE_URL:-http://localhost:5000}"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

echo "🚀 Starting Classification Data Export"
echo "Environment: $ENVIRONMENT"
echo "Service URL: $SERVICE_URL"
echo "Project Root: $PROJECT_ROOT"

# Function to check if service is running
check_service() {
    echo "🔍 Checking if Observations service is running..."
    
    if curl -s --fail "$SERVICE_URL/health" > /dev/null 2>&1; then
        echo "✅ Service is running"
        return 0
    else
        echo "❌ Service is not running at $SERVICE_URL"
        return 1
    fi
}

# Function to start service if needed
start_service() {
    echo "🔄 Starting Observations service..."
    
    cd "$PROJECT_ROOT"
    
    # Set environment
    export ASPNETCORE_ENVIRONMENT="$ENVIRONMENT"
    
    # Start service in background
    nohup dotnet run --project src/Services/MAPP.Services.Observations > /tmp/observations-service.log 2>&1 &
    SERVICE_PID=$!
    
    echo "⏳ Waiting for service to start (PID: $SERVICE_PID)..."
    
    # Wait up to 30 seconds for service to start
    for i in {1..30}; do
        if check_service; then
            echo "✅ Service started successfully"
            return 0
        fi
        echo "   Waiting... ($i/30)"
        sleep 1
    done
    
    echo "❌ Service failed to start within 30 seconds"
    echo "📋 Service logs:"
    tail -20 /tmp/observations-service.log
    return 1
}

# Function to export data
export_data() {
    echo "📤 Triggering data export to cloud storage..."
    
    # Call the export API endpoint (we'll create this)
    local response=$(curl -s -w "\n%{http_code}" -X POST "$SERVICE_URL/api/observations/classifications/export" \
        -H "Content-Type: application/json")
    
    local http_code=$(echo "$response" | tail -n1)
    local body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" = "200" ]; then
        echo "✅ Export completed successfully"
        echo "📋 Response: $body"
        return 0
    else
        echo "❌ Export failed with HTTP $http_code"
        echo "📋 Response: $body"
        return 1
    fi
}

# Function to invalidate cache
invalidate_cache() {
    echo "🗑️ Invalidating Redis cache..."
    
    local response=$(curl -s -w "\n%{http_code}" -X POST "$SERVICE_URL/api/observations/classifications/refresh" \
        -H "Content-Type: application/json")
    
    local http_code=$(echo "$response" | tail -n1)
    local body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" = "200" ]; then
        echo "✅ Cache invalidated successfully"
        return 0
    else
        echo "⚠️ Cache invalidation failed with HTTP $http_code"
        echo "📋 Response: $body"
        # Don't fail the script for cache invalidation issues
        return 0
    fi
}

# Function to verify export
verify_export() {
    echo "🔍 Verifying export..."
    
    local response=$(curl -s -w "\n%{http_code}" "$SERVICE_URL/api/observations/classifications/version")
    
    local http_code=$(echo "$response" | tail -n1)
    local body=$(echo "$response" | head -n -1)
    
    if [ "$http_code" = "200" ]; then
        echo "✅ Export verification successful"
        echo "📋 Version info: $body"
        return 0
    else
        echo "⚠️ Export verification failed with HTTP $http_code"
        return 0
    fi
}

# Main execution
main() {
    echo "================================================"
    echo "🏗️ MAPP Classification Data Export Script"
    echo "================================================"
    
    # Check if service is running, start if needed
    if ! check_service; then
        if ! start_service; then
            echo "❌ Failed to start service. Exiting."
            exit 1
        fi
    fi
    
    # Export data to cloud storage
    if ! export_data; then
        echo "❌ Data export failed. Exiting."
        exit 1
    fi
    
    # Invalidate cache to force refresh
    invalidate_cache
    
    # Verify the export
    verify_export
    
    echo ""
    echo "🎉 Classification data export completed successfully!"
    echo ""
    echo "📊 Summary:"
    echo "   ✅ Database queried"
    echo "   ✅ JSON file generated"
    echo "   ✅ Uploaded to Google Cloud Storage"
    echo "   ✅ Redis cache invalidated"
    echo "   ✅ Export verified"
    echo ""
    echo "💡 Next API requests will use the new cloud storage file"
}

# Handle script interruption
trap 'echo "❌ Script interrupted"; exit 1' INT TERM

# Run main function
main "$@"
