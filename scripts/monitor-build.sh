#!/bin/bash

# Monitor Azure DevOps build and Artifact Registry for new images

echo "🔍 Monitoring Azure DevOps Build and Artifact Registry"
echo "====================================================="

export PATH="/Users/krishnajammula/Development/gcloud/google-cloud-sdk/bin:$PATH"

echo "📋 Latest commit pushed:"
git log --oneline -1

echo ""
echo "🐳 Current images in Artifact Registry:"
gcloud artifacts docker images list us-central1-docker.pkg.dev/mapp-dev-457512/mapp-docker-repo/observations-api --project=mapp-dev-457512 --sort-by="~CREATE_TIME" --limit=3

echo ""
echo "⏰ Waiting for new build to complete..."
echo "Expected: New image should appear in 5-10 minutes"
echo ""

# Function to check for new images
check_for_new_image() {
    local latest_image=$(gcloud artifacts docker images list us-central1-docker.pkg.dev/mapp-dev-457512/mapp-docker-repo/observations-api --project=mapp-dev-457512 --sort-by="~CREATE_TIME" --limit=1 --format="value(digest)")
    echo "Latest image digest: $latest_image"
    
    # Check if it's different from the current one
    local current_digest="sha256:3e016ae1b463fe00d0fc6e047f0b51b2ed56ba98b189f0f10a9316c8a755c010"
    if [ "$latest_image" != "$current_digest" ]; then
        echo "🎉 NEW IMAGE DETECTED!"
        return 0
    else
        echo "⏳ Still waiting for new image..."
        return 1
    fi
}

# Monitor for 15 minutes
for i in {1..15}; do
    echo "🔄 Check $i/15 ($(date))"
    if check_for_new_image; then
        echo ""
        echo "✅ New image available! Ready to deploy to Cloud Run."
        
        # Get the latest image details
        echo "📋 New image details:"
        gcloud artifacts docker images list us-central1-docker.pkg.dev/mapp-dev-457512/mapp-docker-repo/observations-api --project=mapp-dev-457512 --sort-by="~CREATE_TIME" --limit=1
        
        echo ""
        echo "🚀 To deploy the new image to Cloud Run:"
        echo "gcloud run deploy cr-observations \\"
        echo "  --image us-central1-docker.pkg.dev/mapp-dev-457512/mapp-docker-repo/observations-api@$(gcloud artifacts docker images list us-central1-docker.pkg.dev/mapp-dev-457512/mapp-docker-repo/observations-api --project=mapp-dev-457512 --sort-by="~CREATE_TIME" --limit=1 --format="value(digest)") \\"
        echo "  --region us-central1 \\"
        echo "  --project mapp-dev-457512"
        
        break
    fi
    
    if [ $i -lt 15 ]; then
        echo "💤 Waiting 60 seconds before next check..."
        sleep 60
    fi
done

echo ""
echo "🏁 Monitoring complete."
