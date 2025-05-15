from ..chains.communicator_chain import communicator_chain
from ..tools.live_data_tool import summarized_environment_tool


def user_friendly_advice(raw_text: str) -> str:
    """Invokes the chain with data and returns health advice."""
    return communicator_chain.run(raw_text=raw_text)

def data_summary() -> str:
    """Returns a short summary of the newest sensor data"""
    return summarized_environment_tool()