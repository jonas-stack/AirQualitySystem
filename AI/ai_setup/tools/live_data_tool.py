from datetime import datetime
from typing import List, Dict
from langchain.tools import Tool
import numpy as np


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
        {"key": "co2", "max_value": 1000, "unit": "ppm", "label": "CO₂", "recommendation": "Please ventilate the room."},
        {"key": "humidity", "max_value": 70, "unit": "%", "label": "Humidity", "recommendation": "Dehumidification is recommended."},
        {"key": "pm2_5", "max_value": 5.0, "unit": "µg/m³", "label": "PM2.5", "recommendation": "Consider cleaning or using an air purifier."},
        {"key": "pm0_1", "max_value": 10000, "unit": "particles/cm³", "label": "PM0.1", "recommendation": "Use an air purifier or increase ventilation."},
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

def environment_tendency_tool(measurements: Dict[str, List[Dict]]) -> str:
    result = predict_environment_tendency(measurements)
    if result["status"] == "OK":
        return "All environmental parameters are within safe levels."
    else:
        return "\n".join(result["alerts"])

live_environment_tool = Tool(
    name="Live Environment Tendency",
    func=environment_tendency_tool,
    description=
    ("Analyzes trends in live environmental sensor data (CO2, humidity, PM2.5, PM0.1). "
    "Predicts if values will exceed safe thresholds soon and returns warnings and recommendations."),
    return_direct=True,
)