-- Drop tables in correct order (dependent tables first)
DROP TABLE IF EXISTS "DeviceConnection";
DROP TABLE IF EXISTS "TestSensorData";
DROP TABLE IF EXISTS "InvalidPayloads";
DROP TABLE IF EXISTS "Devices";

-- Device information table
CREATE TABLE IF NOT EXISTS "TestDevices" (
    "DeviceId" UUID PRIMARY KEY,
    "DeviceName" VARCHAR(100) NOT NULL,
    "IsConnected" BOOLEAN NOT NULL DEFAULT FALSE,
    "LastSeen" TIMESTAMP NOT NULL
);

-- Device connection history
CREATE TABLE IF NOT EXISTS "TestDeviceConnectionHistory" (
    "Id" SERIAL PRIMARY KEY,
    "DeviceId" UUID NOT NULL,
    "IsConnected" BOOLEAN NOT NULL,
    "LastSeen" TIMESTAMP NOT NULL,
    CONSTRAINT "FK_DeviceConnection_Devices" FOREIGN KEY ("DeviceId") REFERENCES "TestDevices" ("DeviceId")
);

-- Existing tables...
CREATE TABLE IF NOT EXISTS "TestSensorData" (
    "Id" SERIAL PRIMARY KEY,
    "Temperature" DOUBLE PRECISION NOT NULL,
    "Humidity" DOUBLE PRECISION NOT NULL,
    "AirQuality" DOUBLE PRECISION NOT NULL,
    "PM25" DOUBLE PRECISION NOT NULL,
    "DeviceId" UUID NOT NULL,
    "Timestamp" TIMESTAMP NOT NULL,
    CONSTRAINT "FK_TestSensorData_Devices" FOREIGN KEY ("DeviceId") REFERENCES "TestDevices" ("DeviceId"),
    CONSTRAINT "uq_deviceid_timestamp" UNIQUE ("DeviceId", "Timestamp")
);

CREATE TABLE IF NOT EXISTS "TestInvalidPayloads" (
    "Id" SERIAL PRIMARY KEY,
    "DeviceId" VARCHAR(100) NULL,
    "RawPayload" TEXT NOT NULL,
    "ErrorReason" VARCHAR(255) NOT NULL,
    "Timestamp" TIMESTAMP NOT NULL
);