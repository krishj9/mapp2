"""
Shared configuration for MAPP GenAI services
"""

import os
from typing import Optional

class Settings:
    """Application settings"""
    
    # API Configuration
    API_HOST: str = os.getenv("API_HOST", "0.0.0.0")
    API_PORT: int = int(os.getenv("PORT", 8000))
    
    # Database Configuration
    DATABASE_URL: Optional[str] = os.getenv("DATABASE_URL")
    
    # AI/ML Configuration
    OPENAI_API_KEY: Optional[str] = os.getenv("OPENAI_API_KEY")
    MODEL_NAME: str = os.getenv("MODEL_NAME", "gpt-3.5-turbo")
    
    # MAPP API Configuration
    PLANNING_API_URL: str = os.getenv("PLANNING_API_URL", "http://localhost:5001")
    OBSERVATIONS_API_URL: str = os.getenv("OBSERVATIONS_API_URL", "http://localhost:5002")
    USERMANAGEMENT_API_URL: str = os.getenv("USERMANAGEMENT_API_URL", "http://localhost:5003")
    REPORTS_API_URL: str = os.getenv("REPORTS_API_URL", "http://localhost:5004")
    
    # Logging Configuration
    LOG_LEVEL: str = os.getenv("LOG_LEVEL", "INFO")
    
    # Security Configuration
    SECRET_KEY: str = os.getenv("SECRET_KEY", "your-secret-key-here")
    
    # Performance Configuration
    MAX_WORKERS: int = int(os.getenv("MAX_WORKERS", 4))
    TIMEOUT_SECONDS: int = int(os.getenv("TIMEOUT_SECONDS", 30))

settings = Settings()
