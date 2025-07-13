#!/bin/bash

# MAPP Health Check Script
# Usage: ./health-check.sh [domain] [environment]

set -e

# Configuration
DOMAIN=${1:-"all"}
ENVIRONMENT=${2:-"development"}

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
    echo -e "${GREEN}[✓]${NC} $1"
}

log_warning() {
    echo -e "${YELLOW}[⚠]${NC} $1"
}

log_error() {
    echo -e "${RED}[✗]${NC} $1"
}

# Health check endpoints based on environment
get_base_url() {
    case $ENVIRONMENT in
        development)
            echo "http://localhost"
            ;;
        staging)
            echo "https://mapp-staging.azurewebsites.net"
            ;;
        production)
            echo "https://mapp.company.com"
            ;;
    esac
}

# Check single service health
check_service_health() {
    local service_name=$1
    local url=$2
    local timeout=${3:-10}
    
    log_info "Checking $service_name..."
    
    if response=$(curl -s -f --max-time $timeout "$url/health" 2>/dev/null); then
        # Parse JSON response
        status=$(echo "$response" | jq -r '.status // "unknown"' 2>/dev/null || echo "unknown")
        
        case $status in
            "healthy"|"Healthy")
                log_success "$service_name is healthy"
                return 0
                ;;
            "degraded"|"Degraded")
                log_warning "$service_name is degraded"
                echo "$response" | jq '.' 2>/dev/null || echo "$response"
                return 1
                ;;
            "unhealthy"|"Unhealthy")
                log_error "$service_name is unhealthy"
                echo "$response" | jq '.' 2>/dev/null || echo "$response"
                return 2
                ;;
            *)
                log_warning "$service_name returned unknown status: $status"
                return 1
                ;;
        esac
    else
        log_error "$service_name is not responding"
        return 2
    fi
}

# Check domain health
check_domain_health() {
    local domain=$1
    local base_url=$(get_base_url)
    local api_port
    local ai_port
    local mfe_port
    local overall_status=0
    
    case $domain in
        planning)
            api_port=5001
            ai_port=8001
            mfe_port=4201
            ;;
        observations)
            api_port=5002
            ai_port=8002
            mfe_port=4202
            ;;
        usermanagement)
            api_port=5003
            ai_port=8003
            mfe_port=4203
            ;;
        reports)
            api_port=5004
            ai_port=8004
            mfe_port=4204
            ;;
        *)
            log_error "Unknown domain: $domain"
            return 1
            ;;
    esac
    
    log_info "=== Checking $domain Domain ==="
    
    # Check API
    if [ "$ENVIRONMENT" = "development" ]; then
        api_url="$base_url:$api_port"
    else
        api_url="$base_url/api/$domain"
    fi
    
    if ! check_service_health "${domain^} API" "$api_url"; then
        overall_status=1
    fi
    
    # Check AI Service
    if [ "$ENVIRONMENT" = "development" ]; then
        ai_url="$base_url:$ai_port"
    else
        ai_url="$base_url/ai/$domain"
    fi
    
    if ! check_service_health "${domain^} AI" "$ai_url"; then
        overall_status=1
    fi
    
    # Check MFE (basic connectivity)
    if [ "$ENVIRONMENT" = "development" ]; then
        mfe_url="$base_url:$mfe_port"
        if curl -s -f --max-time 5 "$mfe_url" > /dev/null 2>&1; then
            log_success "${domain^} MFE is accessible"
        else
            log_warning "${domain^} MFE is not accessible"
            overall_status=1
        fi
    fi
    
    # Check database connectivity through API
    if response=$(curl -s -f --max-time 10 "$api_url/health" 2>/dev/null); then
        db_status=$(echo "$response" | jq -r '.checks[] | select(.name | contains("database")) | .status' 2>/dev/null || echo "unknown")
        if [ "$db_status" = "Healthy" ] || [ "$db_status" = "healthy" ]; then
            log_success "${domain^} Database is healthy"
        else
            log_warning "${domain^} Database status: $db_status"
            overall_status=1
        fi
    fi
    
    echo ""
    return $overall_status
}

# Generate health report
generate_health_report() {
    local timestamp=$(date -u +"%Y-%m-%dT%H:%M:%SZ")
    local report_file="health-report-$timestamp.json"
    
    log_info "Generating detailed health report..."
    
    cat > "$report_file" << EOF
{
  "timestamp": "$timestamp",
  "environment": "$ENVIRONMENT",
  "overall_status": "checking",
  "domains": {
EOF

    local first=true
    for domain in planning observations usermanagement reports; do
        if [ "$first" = true ]; then
            first=false
        else
            echo "," >> "$report_file"
        fi
        
        echo "    \"$domain\": {" >> "$report_file"
        
        # Get detailed health info for each service
        local base_url=$(get_base_url)
        local api_port
        case $domain in
            planning) api_port=5001 ;;
            observations) api_port=5002 ;;
            usermanagement) api_port=5003 ;;
            reports) api_port=5004 ;;
        esac
        
        if [ "$ENVIRONMENT" = "development" ]; then
            api_url="$base_url:$api_port"
        else
            api_url="$base_url/api/$domain"
        fi
        
        if health_data=$(curl -s -f --max-time 10 "$api_url/health" 2>/dev/null); then
            echo "      \"api\": $health_data" >> "$report_file"
        else
            echo "      \"api\": {\"status\": \"unreachable\"}" >> "$report_file"
        fi
        
        echo "    }" >> "$report_file"
    done

    cat >> "$report_file" << EOF
  }
}
EOF

    log_success "Health report generated: $report_file"
}

# Main function
main() {
    log_info "MAPP Health Check"
    log_info "Domain: $DOMAIN"
    log_info "Environment: $ENVIRONMENT"
    log_info "Timestamp: $(date)"
    echo ""
    
    local overall_status=0
    
    if [ "$DOMAIN" = "all" ]; then
        for domain in planning observations usermanagement reports; do
            if ! check_domain_health $domain; then
                overall_status=1
            fi
        done
    else
        if ! check_domain_health $DOMAIN; then
            overall_status=1
        fi
    fi
    
    echo "=== Summary ==="
    if [ $overall_status -eq 0 ]; then
        log_success "All services are healthy!"
    else
        log_warning "Some services have issues. Check the details above."
    fi
    
    # Generate detailed report if requested
    if [ "${GENERATE_REPORT:-false}" = "true" ]; then
        generate_health_report
    fi
    
    echo ""
    log_info "For real-time monitoring, visit:"
    log_info "  - Health Dashboard: http://localhost:8080"
    log_info "  - Grafana: http://localhost:3000"
    log_info "  - Seq Logs: http://localhost:5341"
    
    exit $overall_status
}

# Check dependencies
if ! command -v curl &> /dev/null; then
    log_error "curl is required but not installed"
    exit 1
fi

if ! command -v jq &> /dev/null; then
    log_warning "jq is not installed. JSON parsing will be limited."
fi

# Run main function
main
