from langchain.agents import AgentType, initialize_agent
from langchain_ollama.llms import OllamaLLM

from ..tools.health_tool import health_advice_tool
from ..tools.rag_tool import air_quality_guide_tool, general_advice_tool
from ..tools.live_data_tool import live_environment_tool
from .communicator_agent import user_friendly_advice


#Combined health Agent
def get_combined_health_agent():
    llm = OllamaLLM(model="gemma2:9b", temperature=0)
    return initialize_agent(
        tools=[
            health_advice_tool,
            air_quality_guide_tool,
            general_advice_tool,
            live_environment_tool,
        ],
        llm=llm,
        agent=AgentType.ZERO_SHOT_REACT_DESCRIPTION,
        verbose=True,
        handle_parsing_errors=True,
        max_iterations=8,
    )

def ask_environment_ai(question: str, mode: str = "friendly") -> str:
    agent = get_combined_health_agent()
    raw = agent.run(question)

    if mode == "raw":
        return raw
    elif mode == "both":
        friendly = user_friendly_advice(raw)
        return {
            "raw": raw,
            "friendly": friendly
        }
    else:
        return user_friendly_advice(raw)
