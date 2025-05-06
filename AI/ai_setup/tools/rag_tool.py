from langchain.chains import RetrievalQA
from langchain_ollama import OllamaLLM
from langchain.tools import Tool
from ..vector_store import build_or_load_vector_store

#Model
llm = OllamaLLM(model="gemma2:9b", temperature=0)

#Load vector
retriever = build_or_load_vector_store().as_retriever()

#QA chain
qa_chain = RetrievalQA.from_chain_type(llm=llm, retriever=retriever)

#Tool
air_quality_guide_tool = Tool(
    name="Air Quality Guide",
    func=qa_chain.run,
    description="Provides information about the air quality, using the guides in pdf",
    return_direct=True,
)

def get_general_climate_advice(_):
    """
    Gives general climate advice about air quality
    """
    prompt = (
        "Can you give me general recommendations for improving indoor air quality? "
        "Assume I don’t have any sensor data. Please give practical, health-based advice "
        "based on the uploaded PDF guides."
    )
    try:
        result = qa_chain.run(prompt)
        return result
    except Exception as ex:
        return f"An error occurred: {ex}"

general_advice_tool = Tool(
    name="General Climate Advice",
    func=get_general_climate_advice,
    description="Provides information about the air quality, using the guides in pdf",
    return_direct=True,
)