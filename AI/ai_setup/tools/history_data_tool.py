from langchain.tools import Tool
from ..database.db_loader import get_connection
from ..chains.history_data_chain import history_data_chain
from typing import List, Dict


def get_history_data(range_str: str = "7d") -> List[Dict]:
    # Accepts formats like "24h", "5d", "30d"
    if range_str.endswith("h"):
        interval = f"{int(range_str[:-1])} hours"
    elif range_str.endswith("d"):
        interval = f"{int(range_str[:-1])} days"
    else:
        raise ValueError(f"Unsupported time range format: {range_str}")

    conn = get_connection()
    cursor = conn.cursor()
    query = f"""
            SELECT "Timestamp", "Temperature", "Humidity", "AirQuality", "PM25"
            FROM "SensorData"
            WHERE "Timestamp" >= NOW() - INTERVAL '{interval}'
            ORDER BY "Timestamp" ASC;
        """
    cursor.execute(query)
    rows = cursor.fetchall()
    cursor.close()
    conn.close()

    return [
        {
            "timestamp": row[0].isoformat(),
            "temperature": row[1],
            "humidity": row[2],
            "airquality": row[3],
            "pm25": row[4]
        } for row in rows
    ]

def summarize_history(data: List[Dict]) -> str:
    if not data:
        return "No historical data found"

    high_humidity = sum(1 for d in data if d["humidity"] > 60)
    high_pm25 = sum(1 for d in data if d["pm25"] > 5.0)
    high_airq = sum(1 for d in data if d["airquality"] > 100)

    patterns = []
    if high_humidity > 10:
        patterns.append("frequent high humidity")
    if high_pm25 > 10:
        patterns.append("elevated PM2.5 levels")
    if high_airq > 10:
        patterns.append("poor air quality index")

    return ", ".join(patterns) if patterns else "no significant patterns"

def run_history_tool(range_str: str = "7d") -> str:
    data = get_history_data(range_str)
    summary = summarize_history(data)
    explanation = history_data_chain.run({"history": summary})
    return explanation


def get_history_advice(summary: str) -> str:
    try:
        return (
            f"The pattern '{summary}' suggests to take action"
            "consider ventilation before this pattern, recurses, "
            "or find out why this spike is caused."
        )
    except Exception as ex:
        return f"An error occurred, during analysis: {ex}"


# Utility to parse range input from agent input like "range_str = '7d'" or "range_str: '7d'"
def parse_range_input(input_str: str) -> str:
    # expected: "range_str = '7d'" or "range_str: '14d'"
    if "=" in input_str:
        return input_str.split("=")[1].strip().strip("'").strip('"')
    elif ":" in input_str:
        return input_str.split(":")[1].strip().strip("'").strip('"')
    else:
        return input_str.strip().strip("'").strip('"')

history_tool = Tool(
    name="History Pattern Analyzer",
    func=lambda s: run_history_tool(parse_range_input(s)),
    description="Analyzes historical air quality patterns and suggests planning actions based on recurring problems. Accepts a range string like '7d' or '48h'.",
    return_direct=True,
    verbose=True,
)