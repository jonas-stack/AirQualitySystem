import os
import psycopg2
from dotenv import load_dotenv

# Gå 3 niveauer op: database/ → ai_setup/ → AI/ → så ned i Server/Startup
dotenv_path = os.path.join(os.path.dirname(__file__), '../../../Server/Startup/.env')
load_dotenv(dotenv_path)

def parse_connection_string(conn_str: str) -> str:
    if not conn_str:
        raise ValueError("DEPLOYED_CONN_STR is missing from .env or not loaded.")
    parts = dict(item.strip().split('=') for item in conn_str.split(';') if item)
    return f"postgresql://{parts['Username']}:{parts['Password']}@{parts['Host']}:{parts['Port']}/{parts['Database']}"

DEPLOYED_CONN_STR = os.getenv("DEPLOYED_CONN_STR")
DATABASE_URL = parse_connection_string(DEPLOYED_CONN_STR)

def get_connection():
    return psycopg2.connect(DATABASE_URL)
