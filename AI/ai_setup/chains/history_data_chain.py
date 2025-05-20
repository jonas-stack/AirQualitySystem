from langchain.chains import LLMChain
from langchain.prompts import PromptTemplate
from langchain_ollama import OllamaLLM

llm = OllamaLLM(model="gemma2:9b", temperature=0)

prompt = PromptTemplate(
    input_variables=["history"],
    template="""
You are a data analyst and health advisor.

Here is historical sensor data over the past few days or weeks:
{history}

The sensor used are:
- BMP280, for temperature and pressure
- MQ-135, for CO₂ and other pollutants
- PMS5003, for fine dust particles(PM2.5)

Please:
1. Identify any trends or problems.
2. Mention thresholds that were exceeded.
3. Give advice based on these long-term patterns.
"""
)

history_data_chain = LLMChain(llm=llm, prompt=prompt)