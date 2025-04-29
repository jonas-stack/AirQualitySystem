from langchain.agents import AgentType, initialize_agent
from ..tools.health_tool import health_advice_tool
from ..tools.rag_tool import air_quality_guide_tool, general_advice_tool
from langchain_ollama.llms import OllamaLLM

#Base model
llm = OllamaLLM(model="gemma2:9b", temperature=0)

#Agent
health_agent = initialize_agent(
    tools=[health_advice_tool, air_quality_guide_tool, general_advice_tool],
    llm=llm,
    agent=AgentType.ZERO_SHOT_REACT_DESCRIPTION,
    verbose=True,
    handle_parsing_errors=True,
)

def get_health_agent():
    return health_agent