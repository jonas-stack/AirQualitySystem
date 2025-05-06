from langchain.chains import RetrievalQA
from langchain_ollama import OllamaLLM
from ...ai_setup import vector_store

llm = OllamaLLM(model="gemma2:9b", temperature=0)

pdf_rag_chain = RetrievalQA.from_chain_type(
    llm=llm,
    retriever=vector_store.as_retriever(),
    chain_type="stuff"
)