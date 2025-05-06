import os
import fitz
from langchain_core.documents import Document
from langchain_text_splitters import RecursiveCharacterTextSplitter


def load_and_split_pdf(pdf_path: str):
    if not os.path.exists(pdf_path):
        print(f"File not found: {pdf_path}")
        return []

    doc = fitz.open(pdf_path)
    full_text = "".join([page.get_text() for page in doc])
    splitter = RecursiveCharacterTextSplitter(chunk_size=800, chunk_overlap=100)
    return splitter.split_documents([Document(page_content=full_text)])