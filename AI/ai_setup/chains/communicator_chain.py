from langchain.chains import LLMChain
from langchain.prompts import PromptTemplate
from langchain_ollama import OllamaLLM

llm = OllamaLLM(model="llama3:8b", temperature=0)

user_friendly_prompt = PromptTemplate(
    input_variables=["raw_answer"],
    template="""
You are a helpful indoor air quality assistant.

Please explain the following technical health or environmental information in simple and clear English that anyone can understand:

{raw_text}

Make the response friendly and polite. Use bullet points and provide concrete, easy-to-follow advice.
"""
)

communicator_chain = LLMChain(llm=llm, prompt=user_friendly_prompt)