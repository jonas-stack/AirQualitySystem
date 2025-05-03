-- Create table for sensor data
CREATE TABLE "SensorData" (
                              "Id" SERIAL PRIMARY KEY,
                              "Temperature" FLOAT NOT NULL,
                              "Humidity" FLOAT NOT NULL,
                              "AirQuality" FLOAT NOT NULL,
                              "PM25" FLOAT NOT NULL,
                              "DeviceId" UUID NOT NULL,
                              "Timestamp" TIMESTAMP NOT NULL
);