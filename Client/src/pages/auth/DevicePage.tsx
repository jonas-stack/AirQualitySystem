import { DashboardStats } from "@/types/dashboard";
import { DeviceDto } from "@/generated-client";
import { StatisticsCards } from "@/components/cards/statistics-cards";
import { DeviceSelector } from "@/components/cards/device-selector";
import { SensorDataTable } from "@/components/cards/sensor-data-table";
import { useEffect, useState } from "react";
import { useDeviceData } from "@/hooks";
import { useWsClient } from "ws-request-hook";

const fakeDevice: DeviceDto = {
    device_id: "123",
    DeviceName: "ESP-32",
    LastSeen: 937193729,
    IsConnected: true
};

export default function DevicePage() {
    const { devices } = useDeviceData();

    const connectedDevicesCount = devices.filter((d) => d.IsConnected).length
    const [selectedDevice, setSelectedDevice] = useState<DeviceDto | null>(null);

    const stats: DashboardStats = {
        totalMeasurements: 15847,
        totalDevices: 1,
        disconnectionsLast24h: 3,
    }

    useEffect(() => {
        if (devices.length > 0) {
            setSelectedDevice(devices[0]);
        }
    }, [devices]);
    
    const mockDevices: DeviceDto[] = [];
    mockDevices.push(fakeDevice);

    const currentSensorData: any = []

    return (
    <div className="space-y-6 py-2">
      <StatisticsCards stats={stats} connectedDevicesCount={connectedDevicesCount} />

      <div className="grid gap-4 lg:grid-cols-12">
        <div className="lg:col-span-4">
          <DeviceSelector devices={devices} selectedDevice={selectedDevice} onDeviceSelect={setSelectedDevice} />
        </div>

        <div className="lg:col-span-8">
          <SensorDataTable selectedDevice={selectedDevice} sensorData={currentSensorData} />
        </div>
      </div>
    </div>

    )
}