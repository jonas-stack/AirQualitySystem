import { DashboardStats } from "@/types/dashboard";
import { DeviceDto } from "@/generated-client";
import { StatisticsCards } from "@/components/cards/statistics-cards";
import { DeviceSelector } from "@/components/cards/device-selector";
import { SensorDataTable } from "@/components/table/sensor-data-table";
import { useEffect, useState } from "react";
import { useDeviceData, useDeviceSensorData } from "@/hooks";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useWsClient } from "ws-request-hook";
import { useDeviceConnectionHistory } from "@/hooks/use-device-connection-history";
import { DeviceConnectionHistoryTable } from "@/components/table/device-connection-history-table";

export default function DevicePage() {
    const { readyState } = useWsClient();
    const { getDevicesArray, iseDevicesLoading } = useDeviceData();
    const { requestSensorDataForDevice, sensorData } = useDeviceSensorData();
    const { requestDeviceConnectionHistoryForDevice, deviceConnectionHistory } = useDeviceConnectionHistory();

    const devices = getDevicesArray();

    const connectedDevicesCount = devices.filter((d) => d.IsConnected).length
    const [selectedDevice, setSelectedDevice] = useState<DeviceDto | null>(null);

    const [activeTab, setActiveTab] = useState("sensor-data")

    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(10);

    const stats: DashboardStats = {
        totalMeasurements: 15847,
        totalDevices: 1,
        disconnectionsLast24h: 3,
    }

    useEffect(() => {
      if (readyState !== 1) {
        setSelectedDevice(null);
      }
    }, [readyState])
    
    useEffect(() => {
      if (selectedDevice?.device_id) {
          switch (activeTab) {
            case "sensor-data":
              requestSensorDataForDevice(selectedDevice.device_id, currentPage, itemsPerPage);
              break;
            case "connection-history":
              requestDeviceConnectionHistoryForDevice(selectedDevice.device_id, currentPage, itemsPerPage);
              break;
          }
      }
    }, [selectedDevice, activeTab, currentPage, itemsPerPage]);

    return (
    <div className="space-y-4 py-2">
      <StatisticsCards stats={stats} connectedDevicesCount={connectedDevicesCount} />

      <div className="grid gap-4 lg:grid-cols-12">
        <div className="lg:col-span-4">
          <DeviceSelector devices={devices} isDevicesLoading={iseDevicesLoading} selectedDevice={selectedDevice} onDeviceSelect={setSelectedDevice} />
        </div>

        <div className="lg:col-span-8">
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
            <TabsList className="grid w-full grid-cols-2">
              <TabsTrigger value="sensor-data" className="cursor-pointer">Sensor Data</TabsTrigger>
              <TabsTrigger value="connection-history" className="cursor-pointer">Connection History</TabsTrigger>
            </TabsList>

            <TabsContent value="sensor-data" className="">
              <SensorDataTable
                selectedDevice={selectedDevice}
                sensorData={sensorData?.items ?? []}
                currentPage={currentPage}
                setCurrentPage={setCurrentPage}
                setItemsPerPage={setItemsPerPage}
                itemsPerPage={itemsPerPage}
                totalPages={sensorData?.totalPages ?? 1}
              />
            </TabsContent>

            <TabsContent value="connection-history" className="mt-4">
                <DeviceConnectionHistoryTable selectedDevice={selectedDevice} deviceConnectionHistory={deviceConnectionHistory?.items ?? []}/>
            </TabsContent>
          </Tabs>
        </div>

      </div>
    </div>

    )
}