#!/bin/bash

# Script to help get PostgreSQL connection information from GCP

echo "🔍 Getting PostgreSQL Connection Information"
echo "==========================================="

export PATH="/Users/krishnajammula/Development/gcloud/google-cloud-sdk/bin:$PATH"

echo "📋 Listing Cloud SQL instances in project mapp-dev-457512..."
echo ""

# List Cloud SQL instances
gcloud sql instances list --project=mapp-dev-457512

echo ""
echo "🔧 To get detailed connection information for a specific instance:"
echo "Replace INSTANCE_NAME with your actual instance name from the list above"
echo ""
echo "gcloud sql instances describe INSTANCE_NAME --project=mapp-dev-457512"
echo ""

echo "📋 Example connection string format:"
echo "Host=INSTANCE_IP;Database=worldplanning;Username=postgres;Password=YOUR_PASSWORD;SSL Mode=Require"
echo ""

echo "🔐 To get the instance IP address:"
echo "gcloud sql instances describe INSTANCE_NAME --project=mapp-dev-457512 --format='value(ipAddresses[0].ipAddress)'"
echo ""

echo "💡 Tips:"
echo "1. Make sure your Cloud SQL instance allows connections from 0.0.0.0/0"
echo "2. Create a database named 'worldplanning' if it doesn't exist"
echo "3. Ensure you have a user with appropriate permissions"
echo "4. Test the connection string before using it in the pipeline"
echo ""

echo "🚀 Once you have the connection string, add it to Azure DevOps as:"
echo "Variable name: DATABASE_CONNECTION_STRING"
echo "Variable value: Host=YOUR_IP;Database=worldplanning;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require"
echo "Mark as secret: ✅ Yes"
