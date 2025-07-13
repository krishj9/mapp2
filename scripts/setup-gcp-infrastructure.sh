#!/bin/bash

# MAPP GCP Infrastructure Setup Script
# This script sets up the required GCP resources for deploying MAPP Observations service

set -e

# Configuration
PROJECT_ID="mapp-dev-457512"
REGION="us-central1"
ARTIFACT_REGISTRY_REPO="mapp-docker-repo"
CLOUD_RUN_SERVICE="cr-observations"
SERVICE_ACCOUNT_EMAIL="mapp-service-account@${PROJECT_ID}.iam.gserviceaccount.com"

echo "🚀 Setting up GCP infrastructure for MAPP Observations service"
echo "=================================================="
echo "Project ID: $PROJECT_ID"
echo "Region: $REGION"
echo "Artifact Registry: $ARTIFACT_REGISTRY_REPO"
echo "Cloud Run Service: $CLOUD_RUN_SERVICE"
echo ""

# Set the project
echo "📍 Setting GCP project..."
gcloud config set project $PROJECT_ID

# Enable required APIs
echo "🔧 Enabling required GCP APIs..."
gcloud services enable \
    cloudbuild.googleapis.com \
    run.googleapis.com \
    artifactregistry.googleapis.com \
    pubsub.googleapis.com \
    sqladmin.googleapis.com \
    secretmanager.googleapis.com

echo "✅ APIs enabled successfully"

# Create Artifact Registry repository
echo "📦 Creating Artifact Registry repository..."
if gcloud artifacts repositories describe $ARTIFACT_REGISTRY_REPO --location=$REGION >/dev/null 2>&1; then
    echo "ℹ️  Artifact Registry repository '$ARTIFACT_REGISTRY_REPO' already exists"
else
    gcloud artifacts repositories create $ARTIFACT_REGISTRY_REPO \
        --repository-format=docker \
        --location=$REGION \
        --description="MAPP Docker images repository"
    echo "✅ Artifact Registry repository created: $REGION-docker.pkg.dev/$PROJECT_ID/$ARTIFACT_REGISTRY_REPO"
fi

# Create Pub/Sub topics if they don't exist
echo "📢 Setting up Pub/Sub topics..."
TOPICS=("observationcreated" "observationsubmitted" "observationvalidated")

for topic in "${TOPICS[@]}"; do
    if gcloud pubsub topics describe $topic >/dev/null 2>&1; then
        echo "ℹ️  Topic '$topic' already exists"
    else
        gcloud pubsub topics create $topic
        echo "✅ Created topic: $topic"
    fi
done

# Create Pub/Sub subscriptions
echo "📥 Setting up Pub/Sub subscriptions..."
SUBSCRIPTIONS=("observationcreated-subscription")

for subscription in "${SUBSCRIPTIONS[@]}"; do
    if gcloud pubsub subscriptions describe $subscription >/dev/null 2>&1; then
        echo "ℹ️  Subscription '$subscription' already exists"
    else
        gcloud pubsub subscriptions create $subscription \
            --topic=observationcreated \
            --ack-deadline=60
        echo "✅ Created subscription: $subscription"
    fi
done

# Set up IAM permissions for the service account
echo "🔐 Setting up IAM permissions..."
gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" \
    --role="roles/run.developer"

gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" \
    --role="roles/pubsub.publisher"

gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" \
    --role="roles/pubsub.subscriber"

gcloud projects add-iam-policy-binding $PROJECT_ID \
    --member="serviceAccount:$SERVICE_ACCOUNT_EMAIL" \
    --role="roles/artifactregistry.writer"

echo "✅ IAM permissions configured"

# Configure Docker authentication for Artifact Registry
echo "🐳 Configuring Docker authentication..."
gcloud auth configure-docker $REGION-docker.pkg.dev

echo ""
echo "🎉 GCP infrastructure setup completed successfully!"
echo "=================================================="
echo "📦 Artifact Registry: $REGION-docker.pkg.dev/$PROJECT_ID/$ARTIFACT_REGISTRY_REPO"
echo "🚀 Cloud Run Service: $CLOUD_RUN_SERVICE (to be created during deployment)"
echo "📢 Pub/Sub Topics: observationcreated, observationsubmitted, observationvalidated"
echo "📥 Pub/Sub Subscriptions: observationcreated-subscription"
echo ""
echo "Next steps:"
echo "1. Upload service account key to Azure DevOps as secure file"
echo "2. Configure Azure DevOps pipeline variables"
echo "3. Run the deployment pipeline"
echo ""
