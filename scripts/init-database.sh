#!/bin/bash

# Database initialization script for MAPP Observations service
# This script creates the database schema using Entity Framework migrations

echo "🗄️ MAPP Database Initialization"
echo "==============================="

export PATH="/Users/krishnajammula/Development/gcloud/google-cloud-sdk/bin:$PATH"

# Database connection details
DB_HOST="35.223.100.132"
DB_NAME="worldplanning"
DB_USER="mappdbuser"
DB_PASSWORD="gu3st123"
CONNECTION_STRING="Host=$DB_HOST;Database=$DB_NAME;Username=$DB_USER;Password=$DB_PASSWORD;SSL Mode=Require"

echo "📋 Database Details:"
echo "Host: $DB_HOST"
echo "Database: $DB_NAME"
echo "User: $DB_USER"
echo ""

echo "🔧 Step 1: Build the application locally"
cd src/Services/MAPP.Services.Observations

# Set the connection string for migrations
export ConnectionStrings__DefaultConnection="$CONNECTION_STRING"

echo "📦 Restoring packages..."
dotnet restore

echo "🏗️ Building application..."
dotnet build --configuration Release

echo "🗄️ Step 2: Create and apply migrations"

# Check if migrations exist
if [ ! -d "Migrations" ]; then
    echo "📝 Creating initial migration..."
    dotnet ef migrations add InitialCreate --context ObservationsDbContext
else
    echo "✅ Migrations folder exists"
fi

echo "🚀 Step 3: Apply migrations to database"
echo "Applying migrations to: $DB_HOST/$DB_NAME"

# Apply migrations
dotnet ef database update --context ObservationsDbContext --connection "$CONNECTION_STRING"

if [ $? -eq 0 ]; then
    echo "✅ Database schema created successfully!"
    echo ""
    echo "📋 Tables created:"
    echo "- Observations"
    echo "- OutboxEvents"
    echo "- Any other domain tables"
    echo ""
    echo "🚀 Ready to deploy to Cloud Run!"
else
    echo "❌ Migration failed. Check the error messages above."
    echo ""
    echo "💡 Troubleshooting tips:"
    echo "1. Verify database connection string"
    echo "2. Ensure database user has CREATE TABLE permissions"
    echo "3. Check if database 'worldplanning' exists"
    echo "4. Verify SSL connection settings"
fi

cd ../../..

echo ""
echo "🎯 Next steps:"
echo "1. If successful, redeploy the Cloud Run service"
echo "2. Test the API endpoints"
echo "3. Verify observations can be created and retrieved"
