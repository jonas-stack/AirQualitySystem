from langchain.chains import LLMChain
from langchain.prompts import PromptTemplate
from langchain_ollama import OllamaLLM

llm = OllamaLLM(model="gemma2:9b", temperatur=0)

prompt = PromptTemplate(
    input_variables=["data"],
    template="""
    You are an indoor environment expert. Based on the current live data:
    {data}

    Please:
    1. Interpret what this data means.
    2. Identify any immediate concerns.
    3. Suggest immediate actions to improve air quality.
    """
)

live_data_chain = LLMChain(llm=llm, prompt=prompt)