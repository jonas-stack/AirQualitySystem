from datetime import datetime
from typing import List, Dict
from langchain.tools import Tool
import numpy as np
from ..database.db_loader import get_connection


def get_live_sensor_data(minutes: int = 30) -> List[Dict]:
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        SELECT "Timestamp", "Temperature", "Humidity", "AirQuality", "PM25"
        FROM "SensorData"
        WHERE "Timestamp" >= NOW() - INTERVAL %s 
        ORDER BY "Timestamp" ASC;
       """, (f"{minutes} minutes",))
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

def group_data_by_key(data: List[Dict]) -> Dict[str, List[Dict]]:
    result = {"temperature": [], "humidity": [], "airquality": [], "pm25": []}
    for d in data:
        for key in result:
            if key in d:
                result[key].append({"timestamp": d["timestamp"], key: d[key]})
    return result


def predict_tendency_risk(measurements: List[Dict], param: str, max_value: float, unit: str, label: str, recommendation: str, max_minutes: int = 30) -> Dict:
    if len(measurements) < 2:
        return {"status": "INSUFFICIENT_DATA", "message": f"Not enough data for {label}."}

    measurements.sort(key=lambda x: x["timestamp"])

    times = [datetime.fromisoformat(m["timestamp"]) for m in measurements if param in m]
    values = [m[param] for m in measurements if param in m]

    if len(times) < 2 or len(values) < 2:
        return {"status": "INSUFFICIENT_DATA", "message": f"Missing time or {label} values."}

    minutes = [(t - times[0]).total_seconds() / 60 for t in times]

    slope, intercept = np.polyfit(minutes, values, 1)

    if slope <= 0:
        return {"status": "STABLE", "message": f"{label} is stable or decreasing."}

    time_to_max = max_value - values[-1] / slope

    if time_to_max <= 0:
        return {"status": "Alert", "message": f"{label} level is already above the safe recommendation!"}
    elif time_to_max <= max_minutes:
        return {
            "status": "PREDICT_ALERT",
            "message": f"If the current trend continues, the {label} level will reach {max_value} {unit} in approximately {int(time_to_max)} minutes. {recommendation}"
        }
    else:
        return {
            "status": "OK",
            "message": f"{label} level is increasing, but the recommended values will not be reached within the next {int(time_to_max)} minutes."
        }

def predict_environment_tendency(measurements: Dict[str, List[Dict]]) -> Dict[str, List[str]]:
    results = []

    checks = [
        {"key": "humidity", "max_value": 60, "unit": "%", "label": "Humidity",
         "recommendation": "Ventilate or dehumidify the room."},
        {"key": "airquality", "max_value": 100, "unit": "index", "label": "Air Quality",
         "recommendation": "Consider cleaning or ventilation."},
        {"key": "pm25", "max_value": 5.0, "unit": "µg/m³", "label": "PM2.5",
         "recommendation": "Consider using an air purifier."},
        {"key": "temperature", "max_value": 27, "unit": "°C", "label": "Temperature",
         "recommendation": "Adjust heating/cooling as needed."}
    ]

    for check in checks:
        key = check["key"]
        if key in measurements:
            result = predict_tendency_risk(
                measurements=measurements[key],
                param=key,
                max_value=check["max_value"],
                unit=check["unit"],
                label=check["label"],
                recommendation=check["recommendation"]
            )
            if result["status"] in ["PREDICT_ALERT", "ALERT"]:
                results.append(result["message"])

    return {
        "status": "ALERT" if results else "OK",
        "alerts": results
    }

#Function for testing data input manual
def environment_tendency_tool(measurements: Dict[str, List[Dict]]) -> str:
    result = predict_environment_tendency(measurements)
    if result["status"] == "OK":
        return "All environmental parameters are within safe levels."
    else:
        return "\n".join(result["alerts"])

def run_live_environment_tool(_: str = "") -> str:
    raw_data = get_live_sensor_data(30)
    grouped = group_data_by_key(raw_data)
    return environment_tendency_tool(grouped)


live_environment_tool = Tool(
    name="Live Environment Tendency",
    func=run_live_environment_tool,
    description=
    ("Analyzes trends in live environmental sensor data (temperature, humidity, air quality, PM2.5)."
    "Predicts if values will exceed safe thresholds soon and returns warnings and recommendations."),
    return_direct=True,
    verbose=True
)