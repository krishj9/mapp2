"""
Planning AI Service - FastAPI application for Gen-AI features
Integrated with MAPP Planning domain
"""

from fastapi import FastAPI, HTTPException, Depends
from fastapi.middleware.cors import CORSMiddleware
from pydantic import BaseModel
from typing import List, Optional
import uvicorn
import os
import time
from datetime import datetime

# Initialize FastAPI app
app = FastAPI(
    title="MAPP Planning AI Service",
    description="Gen-AI features for Planning domain",
    version="1.0.0"
)

# Add CORS middleware
app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],  # Configure appropriately for production
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Pydantic models
class PlanSuggestionRequest(BaseModel):
    title: str
    description: Optional[str] = None
    context: Optional[str] = None

class PlanSuggestionResponse(BaseModel):
    suggestions: List[str]
    estimated_duration: Optional[str] = None
    priority_recommendation: int
    confidence_score: float

class PlanOptimizationRequest(BaseModel):
    plan_id: int
    current_items: List[str]
    constraints: Optional[List[str]] = None

class PlanOptimizationResponse(BaseModel):
    optimized_items: List[str]
    recommendations: List[str]
    efficiency_score: float

# Health check endpoints
@app.get("/health")
async def health_check():
    """Comprehensive health check for Planning AI service"""
    try:
        health_data = {
            "status": "healthy",
            "service": "planning-ai",
            "domain": "Planning",
            "timestamp": datetime.utcnow().isoformat(),
            "version": "1.0.0",
            "environment": os.getenv("ENVIRONMENT", "development"),
            "checks": {
                "api_responsive": True,
                "memory_usage_mb": get_memory_usage(),
                "uptime_seconds": get_uptime(),
                "dependencies": await check_dependencies()
            }
        }

        # Check if any critical issues
        has_issues = any(
            check_name.endswith("_error") or check_name.endswith("_failed")
            for check_name in health_data["checks"].keys()
        )

        if has_issues:
            health_data["status"] = "degraded"

        return health_data
    except Exception as e:
        return {
            "status": "unhealthy",
            "service": "planning-ai",
            "timestamp": datetime.utcnow().isoformat(),
            "error": str(e)
        }

@app.get("/health/ready")
async def readiness_check():
    """Readiness probe for Kubernetes/container orchestration"""
    return {"status": "ready", "service": "planning-ai"}

@app.get("/health/live")
async def liveness_check():
    """Liveness probe for Kubernetes/container orchestration"""
    return {"status": "alive", "service": "planning-ai"}

def get_memory_usage():
    """Get current memory usage in MB"""
    import psutil
    try:
        process = psutil.Process()
        return round(process.memory_info().rss / 1024 / 1024, 2)
    except:
        return "unknown"

def get_uptime():
    """Get service uptime in seconds"""
    try:
        import psutil
        process = psutil.Process()
        return round(time.time() - process.create_time(), 2)
    except:
        return "unknown"

async def check_dependencies():
    """Check external dependencies"""
    dependencies = {}

    # Check Planning API connectivity
    planning_api_url = os.getenv("PLANNING_API_URL")
    if planning_api_url:
        try:
            import httpx
            async with httpx.AsyncClient(timeout=5.0) as client:
                response = await client.get(f"{planning_api_url}/health")
                dependencies["planning_api"] = "healthy" if response.status_code == 200 else "degraded"
        except Exception as e:
            dependencies["planning_api"] = f"error: {str(e)}"
    else:
        dependencies["planning_api"] = "not_configured"

    # Check database connectivity if configured
    db_url = os.getenv("DATABASE_URL")
    if db_url:
        try:
            # Add database connectivity check here if needed
            dependencies["database"] = "not_implemented"
        except Exception as e:
            dependencies["database"] = f"error: {str(e)}"
    else:
        dependencies["database"] = "not_configured"

    return dependencies

# Planning AI endpoints
@app.post("/api/suggestions", response_model=PlanSuggestionResponse)
async def generate_plan_suggestions(request: PlanSuggestionRequest):
    """
    Generate AI-powered plan suggestions based on title and context
    """
    try:
        # Mock AI logic - replace with actual AI implementation
        suggestions = [
            f"Break down '{request.title}' into smaller, manageable tasks",
            "Set clear milestones and deadlines",
            "Identify potential risks and mitigation strategies",
            "Allocate resources and assign responsibilities"
        ]
        
        if request.context:
            suggestions.append(f"Consider context: {request.context}")
        
        return PlanSuggestionResponse(
            suggestions=suggestions,
            estimated_duration="2-4 weeks",
            priority_recommendation=2,  # Medium priority
            confidence_score=0.85
        )
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating suggestions: {str(e)}")

@app.post("/api/optimize", response_model=PlanOptimizationResponse)
async def optimize_plan(request: PlanOptimizationRequest):
    """
    Optimize existing plan items using AI
    """
    try:
        # Mock optimization logic - replace with actual AI implementation
        optimized_items = []
        for item in request.current_items:
            optimized_items.append(f"Optimized: {item}")
        
        recommendations = [
            "Consider parallel execution of independent tasks",
            "Identify critical path dependencies",
            "Optimize resource allocation"
        ]
        
        if request.constraints:
            recommendations.append(f"Applied constraints: {', '.join(request.constraints)}")
        
        return PlanOptimizationResponse(
            optimized_items=optimized_items,
            recommendations=recommendations,
            efficiency_score=0.92
        )
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error optimizing plan: {str(e)}")

@app.get("/api/analytics/{plan_id}")
async def get_plan_analytics(plan_id: int):
    """
    Get AI-powered analytics for a specific plan
    """
    try:
        # Mock analytics - replace with actual AI implementation
        return {
            "plan_id": plan_id,
            "completion_probability": 0.78,
            "risk_factors": ["Resource constraints", "Timeline pressure"],
            "success_indicators": ["Clear milestones", "Engaged team"],
            "recommendations": [
                "Add buffer time for critical tasks",
                "Increase communication frequency"
            ]
        }
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error generating analytics: {str(e)}")

if __name__ == "__main__":
    port = int(os.getenv("PORT", 8001))
    uvicorn.run(app, host="0.0.0.0", port=port)
