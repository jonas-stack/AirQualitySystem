from fastapi import APIRouter, Query, HTTPException
from enum import Enum
from pydantic import BaseModel

from .agents.live_data_agent import get_live_agent
from .agents.history_data_agent import get_history_agent
from .agents.combined_health_agent import get_combined_health_agent, ask_environment_ai
from .agents.communicator_agent import user_friendly_advice


router = APIRouter()

#AI advice mode
class AdviceMode(str, Enum):
    raw = "raw",
    friendly = "friendly",
    both = "both"

#BaseModel
class QuestionInput(BaseModel):
    question: str

#Endpoints
@router.get("/analyze_live_data")
def analyze_sensor_data():
    """
    Automatically retrieves and analyzes live sensor data from the database.
    """
    try:
        agent = get_live_agent()
        raw_result = agent.run("Analyze current live sensor data and give recommendations.")
        friendly_result = user_friendly_advice(raw_result)
        return {
            "type": "live_data_analysis",
            "user_friendly_advice": friendly_result
        }
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error analyzing live data: {str(ex)}")

@router.get("/historical_analysis")
def analyze_historical_data():
    """
    Gets and analyzes historical data from the database.
    """
    try:
        agent = get_history_agent()
        raw_result = agent.run("Analyze historical air quality data and give recommendations.")
        friendly_result = user_friendly_advice(raw_result)

        return {
            "type": "historical_data_analysis",
            "user_friendly_advise": friendly_result,
        }
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error analyzing historical data: {str(ex)}")

@router.get("/ask_ai")
def ask_ai(question: str = Query(..., description="Ask any question about air quality or environment.")):
    """
    General question answered by AetherAI
    """
    try:
        result = ask_environment_ai(question, AdviceMode.friendly)
        return result
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error: {str(ex)}")