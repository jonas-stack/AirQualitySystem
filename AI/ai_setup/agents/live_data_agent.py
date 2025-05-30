from langchain.agents import initialize_agent
from langchain.agents.agent_types import AgentType
from langchain_ollama import OllamaLLM
from ..tools.live_data_tool import live_environment_tool


#Live Agent
def get_live_agent():
    llm = OllamaLLM(model="gemma2:9b", temperature=0)
    return initialize_agent(
        tools=[live_environment_tool],
        llm=llm,
        agent=AgentType.ZERO_SHOT_REACT_DESCRIPTION,
        verbose=True,
        handle_parsing_errors=True,
    )