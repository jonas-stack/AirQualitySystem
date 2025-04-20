import os
from langchain_core.documents import Document
from langchain_text_splitters import RecursiveCharacterTextSplitter
import fitz


def load_pdf(pdf_path: str = "files/indeklima.pdf"):
    pdf_docs = []

    if os.path.exists(pdf_path):
        pdf_text = ""
        doc = fitz.open(pdf_path)
        for page in doc:
            pdf_text += page.get_text()

        text_splitter = RecursiveCharacterTextSplitter(chunk_size=800, chunk_overlap=100)
        pdf_docs = text_splitter.split_documents([Document(page_content=pdf_text)])

        print(f"Loaded {len(pdf_docs)} PDF chunks from {pdf_path}")
    else:
        print(f"PDF file not found at {pdf_path}, skipping PDF loading.")

    return pdf_docs