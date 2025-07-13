#!/bin/bash

# Test script to validate PostgreSQL connection string format

echo "🔍 Testing PostgreSQL Connection String Format"
echo "=============================================="

# Test connection string
CONNECTION_STRING="Host=35.223.100.132;Database=worldplanning;Username=mappdbuser;Password=gu3st123;SSL Mode=Require"

echo "📋 Connection String:"
echo "$CONNECTION_STRING"
echo ""

echo "🔧 Breakdown:"
echo "Host: 35.223.100.132"
echo "Database: worldplanning"
echo "Username: mappdbuser"
echo "Password: gu3st123"
echo "SSL Mode: Require"
echo ""

echo "📝 For Azure DevOps pipeline variable:"
echo "Variable Name: DATABASE_CONNECTION_STRING"
echo "Variable Value: Host=35.223.100.132;Database=worldplanning;Username=mappdbuser;Password=gu3st123;SSL Mode=Require"
echo "Keep this value secret: ✅ Yes"
echo ""

echo "🚀 To update Cloud Run service manually:"
echo 'gcloud run services update cr-observations \'
echo '  --region=us-central1 \'
echo '  --update-env-vars="DATABASE_CONNECTION_STRING=Host=35.223.100.132;Database=worldplanning;Username=mappdbuser;Password=gu3st123;SSL Mode=Require" \'
echo '  --project=mapp-dev-457512'
echo ""

echo "🧪 To test the connection string locally (if you have psql):"
echo "psql 'host=35.223.100.132 dbname=worldplanning user=mappdbuser password=gu3st123 sslmode=require'"
