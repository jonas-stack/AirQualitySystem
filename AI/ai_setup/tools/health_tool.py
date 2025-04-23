from langchain_core.tools import Tool
from ..llm_chain import get_health_chain

health_chain = get_health_chain()

#Method
def generate_helath_advise(prompt: str) -> str:
    """Invokes the chain with data and returns health advice."""
    response = health_chain.invoke({"prompt": prompt})
    return response.content if hasattr(response, "content") else str(response)


#Tool
health_advice_tool = Tool(
    name="Health Advice",
    func=generate_helath_advise,
    description="Provides health and safety advice based on the data from the sensors",
    return_direct=True,
)