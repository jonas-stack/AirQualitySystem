-- Create table for sensor data
CREATE TABLE "SensorData" (
                              "Id" SERIAL PRIMARY KEY,
                              "Temperature" DOUBLE PRECISION NOT NULL,
                              "Humidity" DOUBLE PRECISION NOT NULL,
                              "AirQuality" DOUBLE PRECISION NOT NULL,
                              "PM25" DOUBLE PRECISION NOT NULL,
                              "DeviceId" UUID NOT NULL,
                              "Timestamp" TIMESTAMP NOT NULL,
                              CONSTRAINT uq_deviceid_timestamp UNIQUE ("DeviceId", "Timestamp")
);