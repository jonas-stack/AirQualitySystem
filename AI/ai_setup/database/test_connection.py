from db_loader import get_connection

try:
    conn = get_connection()
    print("✅ Connected:", conn.get_dsn_parameters())
    conn.close()
except Exception as e:
    print("❌ Connection failed:", e)
