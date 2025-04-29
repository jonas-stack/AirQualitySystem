from langchain.tools import Tool

def get_history_advice(summary: str) -> str:
    try:
        return (
            f"The pattern '{summary}' suggest to take action"
            "consider ventilation before this pattern, recurses, "
            "or find out why this spike is caused."
        )
    except Exception as ex:
        return f"An error occurred, during analysis: {ex}"


history_tool = Tool(
    name="History Pattern Analyzer",
    func=get_history_advice,
    description="Analyzes historical air quality patterns and suggests planning actions based on recurring problems."
)
