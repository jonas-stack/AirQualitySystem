import { DashboardStats } from "@/types/dashboard";
import { DeviceDto } from "@/generated-client";
import { StatisticsCards } from "@/components/cards/statistics-cards";
import { DeviceSelector } from "@/components/cards/device-selector";
import { SensorDataTable } from "@/components/cards/sensor-data-table";
import { useEffect, useState } from "react";
import { useDeviceData } from "@/hooks";

export default function DevicePage() {
    const { getDevicesArray, requestSensorDataForDevice, sensorData, iseDevicesLoading } = useDeviceData();
    const devices = getDevicesArray();

    const connectedDevicesCount = devices.filter((d) => d.IsConnected).length
    const [selectedDevice, setSelectedDevice] = useState<DeviceDto | null>(null);

    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(10);

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

    useEffect(() => {
      if (selectedDevice?.device_id) {
          requestSensorDataForDevice(selectedDevice.device_id, currentPage, itemsPerPage);
      }
    }, [selectedDevice, currentPage, itemsPerPage]);

    return (
    <div className="space-y-6 py-2">
      <StatisticsCards stats={stats} connectedDevicesCount={connectedDevicesCount} />

      <div className="grid gap-4 lg:grid-cols-12">
        <div className="lg:col-span-4">
          <DeviceSelector devices={devices} isDevicesLoading={iseDevicesLoading} selectedDevice={selectedDevice} onDeviceSelect={setSelectedDevice} />
        </div>

        <div className="lg:col-span-8">
          <SensorDataTable selectedDevice={selectedDevice} sensorData={sensorData?.items ?? []} currentPage={currentPage} setCurrentPage={setCurrentPage} setItemsPerPage={setItemsPerPage} itemsPerPage={itemsPerPage} totalPages={sensorData?.totalPages ?? 1} />
        </div>
      </div>
    </div>

    )
}