import { DashboardStats } from "@/types/dashboard";
import { DeviceDto, DeviceStatsDto } from "@/generated-client";
import { StatisticsCards } from "@/components/cards/statistics-cards";
import { DeviceSelector } from "@/components/cards/device-selector";
import { SensorDataTable } from "@/components/table/sensor-data-table";
import { useEffect, useState } from "react";
import { useDeviceData, useDeviceSensorData, useDeviecUpdateInterval } from "@/hooks";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { useWsClient } from "ws-request-hook";
import { useDeviceConnectionHistory } from "@/hooks/use-device-connection-history";
import { DeviceConnectionHistoryTable } from "@/components/table/device-connection-history-table";
import { u } from "node_modules/framer-motion/dist/types.d-DDSxwf0n";
import { toast } from "sonner";

export default function DevicePage() {
    const { readyState } = useWsClient();

    const { getDevicesArray, iseDevicesLoading, deviceStats } = useDeviceData();
    const { requestSensorDataForDevice, sensorData } = useDeviceSensorData();
    const { requestDeviceConnectionHistoryForDevice, deviceConnectionHistory } = useDeviceConnectionHistory();
    const { success, updateDeviceInterval } = useDeviecUpdateInterval();

    const defaultDeviceStats: DeviceStatsDto = {
      allTimeMeasurements: 0,
      connectedDevices: 0,
      disconnectionsLast24Hours: 0,
    };

    const deviceUpdateInterval = (device: DeviceDto, interval: number) => {
      updateDeviceInterval(device, interval)
    }

    const devices = getDevicesArray();
    const [selectedDevice, setSelectedDevice] = useState<DeviceDto | null>(null);

    const [activeTab, setActiveTab] = useState("sensor-data")

    const [currentPage, setCurrentPage] = useState(1);
    const [itemsPerPage, setItemsPerPage] = useState(10);

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
      <StatisticsCards stats={deviceStats ?? defaultDeviceStats} />

      <div className="grid gap-4 lg:grid-cols-12">
        <div className="lg:col-span-4">
          <DeviceSelector devices={devices} isDevicesLoading={iseDevicesLoading} selectedDevice={selectedDevice} onDeviceSelect={setSelectedDevice} intervalUpdatedSuccess={success} onUpdateIntervalChange={deviceUpdateInterval}/>
        </div>

        <div className="lg:col-span-8">
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
            <TabsList className="grid w-full grid-cols-2">
              <TabsTrigger value="sensor-data" className="cursor-pointer">Sensor Data</TabsTrigger>
              <TabsTrigger value="connection-history" className="cursor-pointer">Connection History</TabsTrigger>
            </TabsList>

            <TabsContent value="sensor-data">
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

            <TabsContent value="connection-history">
                <DeviceConnectionHistoryTable 
                  selectedDevice={selectedDevice} 
                  deviceConnectionHistory={deviceConnectionHistory?.items ?? []}
                  currentPage={currentPage}
                  setCurrentPage={setCurrentPage}
                  setItemsPerPage={setItemsPerPage} 
                  itemsPerPage={itemsPerPage} 
                  totalPages={deviceConnectionHistory?.totalPages ?? 1} 
                  />
            </TabsContent>
          </Tabs>
        </div>

      </div>
    </div>

    )
}