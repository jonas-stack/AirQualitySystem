from fastapi import APIRouter, Query
from .llm_chain import get_health_chain
from .agents.custom_agent import get_health_agent

#Setup
router = APIRouter()
health_chain = get_health_chain()
agent = get_health_agent()


@router.get("/get_advice")
def ask_sensor_data(query: str = Query(..., description="Sensor data like CO₂, temperature, etc.")):
    try:
        response = health_chain.invoke({"prompt": query})
        return {"input": query, "advise": response.content if hasattr(response, "content") else str(response)}
    except Exception as ex:
        return {"error": str(ex)}

@router.get("/analyze")
def analyze_air_data(prompt: str = Query(..., description="Sensor data like CO₂, temperature.")):
    """
        Takes a prompt with air quality sensor data and returns a health and safety recommendation.
        """
    try:
        result = agent.run({"input": prompt})
        return {"input": prompt, "advice": result}
    except Exception as ex:
        return {"error": str(ex)}
