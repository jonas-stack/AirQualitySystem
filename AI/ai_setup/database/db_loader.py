import os
import psycopg2
from dotenv import load_dotenv

dotenv_path = os.path.join(os.path.dirname(__file__), '../../../Server/Startup/.env')
load_dotenv(dotenv_path)

def parse_connection_string(conn_str: str) -> str:
    if not conn_str:
        raise ValueError("DEPLOYED_CONN_STR_LOCAL is missing from .env or not loaded.")
    parts = {k.strip().lower(): v.strip() for k, v in (item.split('=') for item in conn_str.split(';') if item)}
    return f"postgresql://{parts['username']}:{parts['password']}@{parts['host']}:{parts['port']}/{parts['database']}"

DEPLOYED_CONN_STR = os.getenv("DEPLOYED_CONN_STR_LOCAL")
DATABASE_URL = parse_connection_string(DEPLOYED_CONN_STR)

def get_connection():
    return psycopg2.connect(DATABASE_URL)
