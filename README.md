# IoT Indoor Air Quality Monitoring System

## Overview
This project is a prototype IoT-based environmental monitoring system designed to measure air quality and atmospheric parameters indoors. The system provides real-time data on pollutants, temperature, pressure, and gas levels to help identify potential health and safety issues.

## Table of Contents
- [Introduction](#introduction)
- [Project Description](#project-description)
- [Hardware Architecture](#hardware-architecture)
- [System Architecture](#system-architecture)
- [Technologies & Design Principles](#technologies--design-principles)
- [Software Architecture](#software-architecture)
- [Conclusion](#conclusion)

## Introduction
**Purpose**: To develop an IoT-based system that helps monitor and improve indoor air quality for better human health.

**Problem Statement**: How can a prototype-based IoT system help monitor and improve air quality in indoor environments?

**Team & Roles**:
- **[@jonas-stack](https://github.com/jonas-stack)** – IoT, Backend (.NET), Frontend (React)
- **[@MadsFM](https://github.com/MadsFM)** – AI Module (Python + LLM)
- **[ADD YOUR GIT PROFILE HERE]** – IoT, Backend (.NET), Frontend (React)

## Project Description
The prototype integrates multiple sensors with an ESP32E microcontroller to:
- Measure PM2.5 (fine particles) with a PMS5003
- Measure pressure and temperature using BMP280
- Detect CO2 and other gases with MQ-135
- Display data on a 16x2 I2C LCD
- Transmit data to a server for storage and analysis

## Hardware Architecture
### Component List
| Component     | Description                                           |
|--------------|-------------------------------------------------------|
| 16x2 I2C LCD | Displays sensor data                                  |
| BMP280       | Measures temperature and pressure                    |
| PMS5003      | Measures fine dust particles (PM2.5)                 |
| MQ-135       | Gas sensor for CO2 and other pollutants              |
| ESP32E       | Microcontroller for collecting and transmitting data |

### Wiring Summary
- LCD: VCC, GND, SDA, SCL → ESP32
- BMP280: VCC, GND, SDA, SCL → ESP32
- PMS5003: VCC, GND, TX → GPIO1, RX → GPIO3
- MQ-135: VCC, GND, ANALOG → GPIO14

## System Architecture
### Data Flow
Sensor → ESP32E → HiveMQ (MQTT) → .NET Backend → PostgreSQL + WebSocket → React Frontend → AI Module

### Communication Protocols
- I2C: For LCD & BMP280
- UART: For PMS5003
- GPIO: For MQ-135
- MQTT: ESP32 → Backend
- WebSocket: Backend → Frontend (real-time updates)
- REST API: Backend ↔ Frontend

## Technologies & Design Principles
### Technologies Used
- **ESP32E** – Microcontroller with Wi-Fi
- **React + TypeScript** – Frontend
- **.NET (C#)** – Backend with Onion Architecture
- **PostgreSQL** – Database
- **HiveMQ** – MQTT Broker
- **WebSocket** – Real-time communication
- **Python + LLM (Ollama)** – AI module for insights

### Design Principles
- Onion architecture for clean separation of concerns
- Scalable sensor integration
- Modular and extensible prototype

## Software Architecture

![Architecture Diagram](Images/System.Png)


### Frontend (React + TypeScript)
- Real-time data from WebSocket
- Natural language recommendations from AI
- Hosted on Firebase

### Backend (.NET, C#)
- MQTT listener for incoming sensor data
- REST API for frontend
- WebSocket server for live updates
- Data storage in PostgreSQL
- AI data pipeline support

### AI Module (Python + LLM)
- Fetches data from PostgreSQL
- Analyzes patterns and generates natural-language advice

**Example outputs**:
- "Consider ventilating the room in the next 15 minutes."
- "PM2.5 particle levels are rising – maybe you should vacuum right now."
- "CO₂ levels have increased since yesterday."

## Conclusion
The prototype successfully demonstrates how IoT, cloud, and AI technologies can work together to create a smart air monitoring system. It is a functional proof of concept with:
- Real-time sensor data collection and visualization
- Reliable storage and communication using MQTT, WebSocket, and REST
- AI-generated health and environment recommendations

This system showcases the potential of IoT to contribute to healthier indoor environments by offering actionable insights in a user-friendly manner.

