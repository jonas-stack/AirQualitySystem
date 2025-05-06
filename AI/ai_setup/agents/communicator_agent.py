from ..chains.communicator_chain import communicator_chain


def user_friendly_advice(raw_text: str) -> str:
    """Invokes the chain with data and returns health advice."""
    return communicator_chain.run(raw_text=raw_text)