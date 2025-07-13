#!/bin/bash

# Test script for PostgreSQL database connection
# This script helps verify the database connection string format for GCP PostgreSQL

echo "🔍 MAPP Database Connection Test"
echo "================================"

# Example connection string format for GCP Cloud SQL PostgreSQL
echo "📋 Expected connection string format for GCP Cloud SQL:"
echo "Host=YOUR_INSTANCE_IP;Database=worldplanning;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require"
echo ""

# Common GCP Cloud SQL connection patterns
echo "📋 Common GCP Cloud SQL connection patterns:"
echo ""
echo "1. Public IP connection:"
echo "   Host=34.123.456.789;Database=worldplanning;Username=postgres;Password=your_password;SSL Mode=Require"
echo ""
echo "2. Private IP connection:"
echo "   Host=10.123.456.789;Database=worldplanning;Username=postgres;Password=your_password;SSL Mode=Require"
echo ""
echo "3. Connection via Cloud SQL Proxy:"
echo "   Host=127.0.0.1;Port=5432;Database=worldplanning;Username=postgres;Password=your_password"
echo ""

# Environment variable format for Azure DevOps
echo "📋 Azure DevOps pipeline variable format:"
echo "Variable name: DATABASE_CONNECTION_STRING"
echo "Variable value: Host=YOUR_INSTANCE_IP;Database=worldplanning;Username=YOUR_USER;Password=YOUR_PASSWORD;SSL Mode=Require"
echo "Mark as secret: ✅ Yes"
echo ""

echo "🔧 To set up your database connection:"
echo "1. Get your GCP Cloud SQL instance IP address"
echo "2. Ensure your Cloud SQL instance allows connections from 0.0.0.0/0 (for Cloud Run)"
echo "3. Create a database user with appropriate permissions"
echo "4. Test the connection string locally if possible"
echo "5. Add the connection string to Azure DevOps as a secret variable"
echo ""

echo "✅ Ready for Azure DevOps pipeline deployment!"
echo ""
echo "Next steps:"
echo "1. Go to Azure DevOps: https://dev.azure.com/BHADO/Full%20Service"
echo "2. Upload service account key as secure file"
echo "3. Create variable group with DATABASE_CONNECTION_STRING"
echo "4. Create pipeline from .azure/pipelines/observations-simple-deploy.yml"
echo "5. Run the pipeline to deploy to cr-observations"
