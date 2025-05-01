-- Create table for sensor data
CREATE TABLE SensorData (
                            Id SERIAL PRIMARY KEY,
                            Temperature FLOAT NOT NULL,
                            Humidity FLOAT NOT NULL,
                            AirQuality FLOAT NOT NULL,
                            PM25 FLOAT NOT NULL,
                            DeviceId VARCHAR(50) NOT NULL,
                            Timestamp TIMESTAMP NOT NULL
);

-- Create table for device status messages
CREATE TABLE DeviceStatus (
                              Id SERIAL PRIMARY KEY,
                              Status VARCHAR(255) NOT NULL,
                              DeviceId VARCHAR(50) NOT NULL,
                              Timestamp TIMESTAMP NOT NULL
);