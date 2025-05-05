from langchain.agents import AgentType, initialize_agent
from ..tools.health_tool import health_advice_tool
from ..tools.rag_tool import air_quality_guide_tool, general_advice_tool
from ..tools.live_data_tool import live_environment_tool
from langchain_ollama.llms import OllamaLLM

llm = OllamaLLM(model="llama3:8b", temperature=0)

environment_agent = initialize_agent(
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

def ask_environment_ai(question: str) -> str:
    return environment_agent.run(question)
