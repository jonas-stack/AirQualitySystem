-- Drop tables in correct order (dependent tables first)
DROP TABLE IF EXISTS "DeviceConnectionHistory";
DROP TABLE IF EXISTS "Devices";
DROP TABLE IF EXISTS "SensorData";
DROP TABLE IF EXISTS "InvalidPayloads";

-- Existing SensorData table
CREATE TABLE IF NOT EXISTS "TestSensorData" (
        "Id" SERIAL PRIMARY KEY,
        "Temperature" DOUBLE PRECISION NOT NULL,
        "Humidity" DOUBLE PRECISION NOT NULL,
        "AirQuality" DOUBLE PRECISION NOT NULL,
        "PM25" DOUBLE PRECISION NOT NULL,
        "DeviceId" UUID NOT NULL,
        "Timestamp" TIMESTAMP NOT NULL,
        CONSTRAINT uq_deviceid_timestamp UNIQUE ("DeviceId", "Timestamp")
    );

-- New table for device information
CREATE TABLE IF NOT EXISTS "Devices" (
        "DeviceId" UUID PRIMARY KEY,
        "DeviceName" VARCHAR(100) NOT NULL,
        "IsConnected" BOOLEAN NOT NULL DEFAULT FALSE,
        "LastSeen" TIMESTAMP NOT NULL
    );

-- New table for connection history
CREATE TABLE IF NOT EXISTS "DeviceConnectionHistory" (
        "Id" SERIAL PRIMARY KEY,
        "DeviceId" UUID NOT NULL,
        "IsConnected" BOOLEAN NOT NULL,
        "Timestamp" TIMESTAMP NOT NULL,
        CONSTRAINT fk_device FOREIGN KEY ("DeviceId") REFERENCES "Devices"("DeviceId")
    );

-- New table for invalid payloads
CREATE TABLE IF NOT EXISTS "InvalidPayloads" (
        "Id" SERIAL PRIMARY KEY,
        "DeviceId" VARCHAR(100) NULL, -- Using VARCHAR since ID might be invalid
        "RawPayload" TEXT NOT NULL,
        "ErrorReason" VARCHAR(255) NOT NULL,
        "Timestamp" TIMESTAMP NOT NULL
    );