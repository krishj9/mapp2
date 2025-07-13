#!/bin/bash

# MAPP Domain Deployment Script
# Usage: ./deploy-domain.sh <domain> <environment> [options]

set -e

# Configuration
DOMAIN=$1
ENVIRONMENT=$2
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Logging functions
log_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

log_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Validate inputs
if [ -z "$DOMAIN" ] || [ -z "$ENVIRONMENT" ]; then
    log_error "Usage: $0 <domain> <environment>"
    log_info "Available domains: planning, observations, usermanagement, reports, all"
    log_info "Available environments: development, staging, production"
    exit 1
fi

# Validate domain
case $DOMAIN in
    planning|observations|usermanagement|reports|all)
        ;;
    *)
        log_error "Invalid domain: $DOMAIN"
        log_info "Available domains: planning, observations, usermanagement, reports, all"
        exit 1
        ;;
esac

# Validate environment
case $ENVIRONMENT in
    development|staging|production)
        ;;
    *)
        log_error "Invalid environment: $ENVIRONMENT"
        log_info "Available environments: development, staging, production"
        exit 1
        ;;
esac

# Function to deploy a single domain
deploy_single_domain() {
    local domain=$1
    local env=$2
    
    log_info "Deploying $domain domain to $env environment..."
    
    # Build .NET API
    log_info "Building .NET API for $domain..."
    cd "$PROJECT_ROOT/src/Services/MAPP.Services.${domain^}"
    dotnet build --configuration Release
    
    # Build Python AI service (if exists)
    if [ -d "$PROJECT_ROOT/src/Services/genai/${domain}-ai" ]; then
        log_info "Building Python AI service for $domain..."
        cd "$PROJECT_ROOT/src/Services/genai/${domain}-ai"
        python -m pip install -r requirements.txt
        python -m py_compile main.py
    fi
    
    # Build Angular MFE (if exists)
    if [ -d "$PROJECT_ROOT/src/Web/${domain}-mfe" ]; then
        log_info "Building Angular MFE for $domain..."
        cd "$PROJECT_ROOT/src/Web/${domain}-mfe"
        npm ci
        npm run build --prod
    fi
    
    # Deploy based on environment
    case $env in
        development)
            deploy_to_development $domain
            ;;
        staging)
            deploy_to_staging $domain
            ;;
        production)
            deploy_to_production $domain
            ;;
    esac
    
    log_success "$domain domain deployed to $env successfully!"
}

# Development deployment (local)
deploy_to_development() {
    local domain=$1
    log_info "Starting $domain domain in development mode..."
    
    cd "$PROJECT_ROOT/orchestration/MAPP.${domain^}.AppHost"
    dotnet run &
    
    log_info "$domain domain started in development mode"
}

# Staging deployment (Azure)
deploy_to_staging() {
    local domain=$1
    log_info "Deploying $domain domain to Azure staging..."
    
    # Deploy API to Azure App Service
    az webapp deploy \
        --resource-group "mapp-staging-rg" \
        --name "mapp-${domain}-api-staging" \
        --src-path "$PROJECT_ROOT/src/Services/MAPP.Services.${domain^}/bin/Release/net9.0/publish" \
        --type zip
    
    # Deploy AI service to Azure Functions (if exists)
    if [ -d "$PROJECT_ROOT/src/Services/genai/${domain}-ai" ]; then
        az functionapp deployment source config-zip \
            --resource-group "mapp-staging-rg" \
            --name "mapp-${domain}-ai-staging" \
            --src "$PROJECT_ROOT/src/Services/genai/${domain}-ai.zip"
    fi
    
    # Deploy MFE to Azure Storage (if exists)
    if [ -d "$PROJECT_ROOT/src/Web/${domain}-mfe/dist" ]; then
        az storage blob upload-batch \
            --account-name "mappstaging" \
            --destination '$web' \
            --source "$PROJECT_ROOT/src/Web/${domain}-mfe/dist"
    fi
}

# Production deployment (Azure)
deploy_to_production() {
    local domain=$1
    log_info "Deploying $domain domain to Azure production..."
    
    # Add production deployment logic here
    log_warning "Production deployment not yet implemented"
}

# Health check function
check_domain_health() {
    local domain=$1
    local port
    
    case $domain in
        planning) port=5001 ;;
        observations) port=5002 ;;
        usermanagement) port=5003 ;;
        reports) port=5004 ;;
    esac
    
    log_info "Checking health of $domain domain..."
    
    # Wait for service to start
    sleep 10
    
    # Check API health
    if curl -f "http://localhost:$port/health" > /dev/null 2>&1; then
        log_success "$domain API is healthy"
    else
        log_error "$domain API health check failed"
        return 1
    fi
    
    # Check AI service health (if exists)
    local ai_port=$((8000 + port - 5000))
    if curl -f "http://localhost:$ai_port/health" > /dev/null 2>&1; then
        log_success "$domain AI service is healthy"
    else
        log_warning "$domain AI service health check failed or not available"
    fi
}

# Main deployment logic
main() {
    log_info "Starting MAPP domain deployment..."
    log_info "Domain: $DOMAIN"
    log_info "Environment: $ENVIRONMENT"
    
    if [ "$DOMAIN" = "all" ]; then
        for domain in planning observations usermanagement reports; do
            deploy_single_domain $domain $ENVIRONMENT
            check_domain_health $domain
        done
    else
        deploy_single_domain $DOMAIN $ENVIRONMENT
        check_domain_health $DOMAIN
    fi
    
    log_success "Deployment completed successfully!"
    log_info "Access your application:"
    log_info "  - Health Dashboard: http://localhost:8080"
    log_info "  - Monitoring: http://localhost:3000 (Grafana)"
    log_info "  - Logs: http://localhost:5341 (Seq)"
}

# Run main function
main
