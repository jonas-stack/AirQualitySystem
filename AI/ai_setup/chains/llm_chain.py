from langchain_core.prompts import ChatPromptTemplate
from langchain_ollama.llms import OllamaLLM

#Model initialization
llm_health = OllamaLLM(model="gemma2:9b", temperature=0)

#Prompt for Health Advice
health_advice_prompt = ChatPromptTemplate.from_messages([
    ("system",
     "You are an expert in indoor air quality and occupational health. "
     "You receive sensor data from an IoT system that monitors CO₂(base ppm = 400), temperature, humidity, and gas levels.\n"
     "Based on the provided data, you must:\n"
     "1. Explain what the numbers indicate.\n"
     "2. Provide health and safety-related advice.\n"
     "3. Use a clear, friendly tone that any person can understand."),
    ("user", "{prompt}")
])


#Setup Chain
health_chain = health_advice_prompt | llm_health


def get_health_chain():
    return health_chain