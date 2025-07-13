# Azure DevOps Pipeline Setup for MAPP Observations Service

## 🎯 Overview
This guide will help you set up the Azure DevOps pipeline to deploy the MAPP Observations service to Google Cloud Run.

## 📋 Prerequisites
- ✅ Code pushed to Azure DevOps repository
- ✅ GCP project `mapp-dev-457512` with required APIs enabled
- ✅ Artifact Registry repository `mapp-docker-repo` created
- ✅ PostgreSQL instance running in GCP
- ✅ Service account key file `mapp-dev-457512-6bfcf231ed81.json`

## 🚀 Step-by-Step Setup

### Step 1: Upload Service Account Key to Azure DevOps

1. Go to your Azure DevOps project: `https://dev.azure.com/BHADO/Full%20Service`
2. Navigate to **Pipelines** → **Library**
3. Click **+ Secure files**
4. Upload the file `mapp-dev-457512-6bfcf231ed81.json`
5. Rename it to: `mapp-dev-457512-service-account.json`
6. Set permissions to allow pipeline access

### Step 2: Create Pipeline Variables

1. Go to **Pipelines** → **Library** → **Variable groups**
2. Create a new variable group: `MAPP-Production-Variables`
3. Add the following variables:

| Variable Name | Value | Secret |
|---------------|-------|--------|
| `DATABASE_CONNECTION_STRING` | `Host=YOUR_GCP_POSTGRES_HOST;Database=worldplanning;Username=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;SSL Mode=Require` | ✅ Yes |
| `GCP_PROJECT_ID` | `mapp-dev-457512` | ❌ No |
| `GCP_REGION` | `us-central1` | ❌ No |

### Step 3: Create the Pipeline

1. Go to **Pipelines** → **Pipelines**
2. Click **New pipeline**
3. Select **Azure Repos Git**
4. Choose your repository: `MAPPV2`
5. Select **Existing Azure Pipelines YAML file**
6. Choose the path: `.azure/pipelines/observations-simple-deploy.yml`
7. Click **Continue**

### Step 4: Configure Pipeline Settings

1. Before running, click **Variables** in the pipeline editor
2. Link the variable group: `MAPP-Production-Variables`
3. Save the pipeline

### Step 5: Run the Pipeline

1. Click **Run pipeline**
2. Monitor the deployment progress
3. The pipeline will:
   - ✅ Build the .NET application
   - ✅ Create Docker image
   - ✅ Push to Artifact Registry
   - ✅ Deploy to Cloud Run service `cr-observations`
   - ✅ Test the deployed service

## 🔍 Expected Results

### Successful Deployment Output:
```
✅ GCP authentication completed
✅ Dependencies restored
✅ Application built successfully
✅ Docker image built: us-central1-docker.pkg.dev/mapp-dev-457512/mapp-docker-repo/observations-api:BUILD_NUMBER
✅ Image pushed to Artifact Registry
✅ Deployed to Cloud Run service: cr-observations
✅ Service URL: https://cr-observations-HASH-uc.a.run.app
✅ Health check passed
✅ Swagger endpoint accessible
```

### Service Endpoints:
- **API Base URL**: `https://cr-observations-HASH-uc.a.run.app`
- **Health Check**: `https://cr-observations-HASH-uc.a.run.app/health`
- **Swagger UI**: `https://cr-observations-HASH-uc.a.run.app/swagger`
- **Create Observation**: `POST https://cr-observations-HASH-uc.a.run.app/api/observations`

## 🧪 Testing the Deployed Service

### Test 1: Health Check
```bash
curl https://cr-observations-HASH-uc.a.run.app/health
```

### Test 2: Create Observation
```bash
curl -X POST https://cr-observations-HASH-uc.a.run.app/api/observations \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Cloud Run Test Observation",
    "description": "Testing the deployed service on Google Cloud Run",
    "priority": 1,
    "location": "Google Cloud"
  }'
```

### Test 3: Verify Pub/Sub Integration
Check GCP Console → Pub/Sub → Topics → `observationcreated` for published messages.

## 🔧 Troubleshooting

### Common Issues:

1. **Authentication Error**: Ensure service account key is uploaded correctly
2. **Database Connection**: Verify PostgreSQL connection string and firewall rules
3. **Docker Build Fails**: Check Dockerfile and dependencies
4. **Cloud Run Deployment Fails**: Verify GCP permissions and quotas

### Debug Commands:
```bash
# Check Cloud Run service status
gcloud run services describe cr-observations --region=us-central1

# View Cloud Run logs
gcloud logging read "resource.type=cloud_run_revision AND resource.labels.service_name=cr-observations" --limit=50

# Test local Docker build
docker build -f src/Services/MAPP.Services.Observations/Dockerfile -t test-image .
```

## 🎉 Success Criteria

✅ Pipeline runs without errors
✅ Docker image pushed to Artifact Registry
✅ Cloud Run service `cr-observations` deployed
✅ Service responds to health checks
✅ API endpoints accessible via HTTPS
✅ Database connectivity working
✅ Pub/Sub events publishing successfully

## 📞 Next Steps

After successful deployment:
1. Set up monitoring and alerting
2. Configure custom domain
3. Implement authentication
4. Set up staging environment
5. Create additional domain services
