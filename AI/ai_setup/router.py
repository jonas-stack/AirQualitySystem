from fastapi import APIRouter, Query, HTTPException
from enum import Enum
from .llm_chain import get_health_chain
from .agents.custom_agent import get_health_agent
from .agents.communicator_agent import user_friendly_advice
from .agents.environment_agent import ask_environment_ai
from .agents.live_agent import get_live_agent
from .tools.live_data_tool import live_environment_tool
from .tools.history_tool import history_tool
from pydantic import BaseModel

# Initialize Router, Health Chain, and Custom Agent
router = APIRouter()
health_chain = get_health_chain()
agent = get_health_agent()

#AI advice mode
class AdviceMode(str, Enum):
    raw = "raw",
    friendly = "friendly",
    both = "both"

#BaseModel
class QuestionInput(BaseModel):
    question: str


@router.get("/analyze_live_data")
def ask_sensor_data():
    """
    Automatically retrieves and analyzes live sensor data from the database.
    """
    try:
        response = live_environment_tool.func()
        return {
            "type": "live_data_analysis",
            "advice": response
        }
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error analyzing live data: {str(ex)}")

@router.get("/planning_advice")
def analyze_air_data(
        prompt: str = Query(..., description="Sensor data like CO₂, temperature."),
        mode: AdviceMode = Query(AdviceMode.friendly, description="Choose between, raw, friendly, or both")):
    """
        UI Endpoint for user-facing air quality analysis and advice.
        :param mode: Option: raw, friendly, or both
        :param prompt: Sensor data (e.g., CO₂, temperature)
        :return: Friendly and actionable recommendations
   """
    try:
        raw_result = agent.run({"input": prompt})

        if mode == AdviceMode.raw:
            return {
                "input": prompt,
                "raw_advise": raw_result
            }
        elif mode == AdviceMode.friendly:
            friendly_result = user_friendly_advice(raw_result)
            return {
                "input": prompt,
                "user_friendly_advise": friendly_result
            }
        elif mode == AdviceMode.both:
            friendly_result = user_friendly_advice(raw_result)
            return {
                "input": prompt,
                "raw_advise": raw_result,
                "user_friendly_advise": friendly_result
            }
        else:
            raise HTTPException(status_code=422, detail="Invalid mode selected.")
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error: str(ex)")


@router.get("/general_user_advice")
def general_user_advice(
    prompt: str = Query(..., description="Ask a general question about indoor air quality.")
):
    """
    Ask a general air quality question. Uses the custom agent with all tools and formats the answer to be user-friendly.
    """
    try:
        raw_response = get_health_agent().run(prompt)
        friendly_response = user_friendly_advice(raw_response)
        return {
            "input": prompt,
            "type": "friendly_general_advice",
            "advice": friendly_response
        }
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error: {str(ex)}")

@router.get("/historical_analysis")
def analyze_historical_data():
    """
    Henter og analyserer historiske data fra databasen og giver anbefalinger
    baseret på identificerede mønstre (f.eks. høj CO₂, lav luftfugtighed).
    """
    try:

        historical_data = history_tool.func()
        raw_result = agent.run({"input": historical_data})
        friendly_result = user_friendly_advice(raw_result)

        return {
            "type": "historical_data_analysis",
            "raw_advise": raw_result,
            "user_friendly_advise": friendly_result,
            "data_used": historical_data
        }
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error analyzing historical data: {str(ex)}")

@router.get("/ask_ai")
def ask_ai(question: str = Query(..., description="Ask any question about air quality or environment.")):
    try:
        answer = ask_environment_ai(question)
        return {"answer": answer}
    except Exception as ex:
        raise HTTPException(status_code=500, detail=f"Internal error: {str(ex)}")
