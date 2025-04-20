from fastapi import APIRouter, Query
from .llm_chain import get_qa_question

router = APIRouter()
qa_chain = get_qa_question()


@router.get("/ask")
def ask_question(query: str = Query(..., description="Your question")):
    try:
        response = qa_chain.run(query)
        return {"question": query, "answer": response}
    except Exception as ex:
        return {"error": str(ex)}
