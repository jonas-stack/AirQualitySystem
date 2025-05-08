import os
from langchain.embeddings import OllamaEmbeddings
from langchain.vectorstores import FAISS
from .tools.pdf_loader import load_and_split_pdf

INDEX_PATH = "faiss_index"
INDEX_FILE = os.path.join(INDEX_PATH, "index.faiss")
PDF_PATHS = [
    "files/data_godt_indeklima.pdf",
    "files/indeklima_sundhedsstyreslsen.pdf",
]

def build_or_load_vector_store():
    embedding_model = OllamaEmbeddings(model="nomic-embed-text")

    if not os.path.exists(INDEX_FILE):
        print("Creating new index....")
        all_docs = []
        for path in PDF_PATHS:
            docs = load_and_split_pdf(path)
            all_docs.extend(docs)

        vector_store = FAISS.from_documents(all_docs, embedding_model)
        vector_store.save_local(INDEX_PATH)
    else:
        print("Loading existing index....")
        vector_store = FAISS.load_local(INDEX_PATH, embedding_model, allow_dangerous_deserialization=True)

    return vector_store