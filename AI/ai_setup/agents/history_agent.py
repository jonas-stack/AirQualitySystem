from langchain.agents import initialize_agent
from langchain.agents.agent_types import AgentType
from langchain_ollama import OllamaLLM
from ..tools.history_tool import history_tool
from ..tools.health_tool import health_advice_tool


llm = OllamaLLM(model="gemma2:9b", temperature=0)

history_agent = initialize_agent(
    tools=[history_tool, health_advice_tool],
    llm=llm,
    agent=AgentType.ZERO_SHOT_REACT_DESCRIPTION,
    verbose=True,
    handle_parsing_errors=True,
)

def get_history_agent():
    return history_agent