from  fastapi import FastAPI
from ai_setup.router import router


app = FastAPI()

app.include_router(router, prefix="/AetherAI", tags=["AetherAI"])


@app.get("/")
def read_root():
    return {"Hej, Jeg er AetherAI. "
            "Hvordan kan jeg hjælpe dig?"}