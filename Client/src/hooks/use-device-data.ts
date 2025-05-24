import { ClientRequestDeviceList, ClientRequestSensorData, DeviceDto, PagedResultOfSensorDataDto, SensorDataDto, TimePeriod, WebsocketEvents, WebsocketMessage_1 } from "@/generated-client";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useDeviceData() {
    const [devices, setDevices] = useState<Record<string, DeviceDto>>({});
    const [iseDevicesLoading, setIsDevicesLoading] = useState(true)

    const [sensorData, setSensorData] = useState<PagedResultOfSensorDataDto | undefined>(undefined);
    const [isSensorDataLoading, setIsSensorDataLoading] = useState(true)

    const {sendRequest, onMessage, readyState} = useWsClient()

    const getDevicesArray = (): DeviceDto[] => Object.values(devices);

    const requestDevices = async () => {
        setIsDevicesLoading(true)

        const requestDevices: ClientRequestDeviceList = {
            eventType: WebsocketEvents.ClientRequestDeviceList
        }

        try {
            const deviceResult: WebsocketMessage_1 = await sendRequest<ClientRequestDeviceList, WebsocketMessage_1>(
                requestDevices,
                WebsocketEvents.ServerResponseDeviceList,
            )

            const deviceList = deviceResult?.Data?.DeviceList ?? [];

            const deviceMap: Record<string, DeviceDto> = {};
            for (const device of deviceList) {
                if (device.device_id) {
                    deviceMap[device.device_id] = device;
                }
            }

            setDevices(deviceMap);
        } catch (error) {
            toast.error("Device fetching failed", {
                description: "An error occured while trying to fetch devices.",
            });
    } finally {
        setIsDevicesLoading(false)
        }
    }

    const registerDeviceConnectionUpdates = () => {
        return onMessage<WebsocketMessage_1>(WebsocketEvents.BroadcastDeviceConnectionUpdate, (dto) => {
            const updatedDevice = dto.data as DeviceDto;
            if (updatedDevice && updatedDevice.device_id) {
                const id = updatedDevice.device_id;
                setDevices(prevDevices => ({
                    ...prevDevices,
                    [id]: updatedDevice,
                }));
            }
        });
    };

    const requestSensorDataForDevice = async (deviceId: string, pageNumber: number, pageSize: number) => {
        setIsSensorDataLoading(true)

        const requestDevices: ClientRequestSensorData = {
            eventType: "ClientRequestSensorData",
            sensorId: deviceId,
            pageNumber: pageNumber,
            pageSize: pageSize
        }

        try {
            const deviceResult: WebsocketMessage_1 = await sendRequest<ClientRequestSensorData, WebsocketMessage_1>(
                requestDevices,
                "ServerResponseSensorData",
            )

            const rawSensorData = deviceResult?.Data?.SensorData;

            if (rawSensorData) {
            const mappedSensorData: PagedResultOfSensorDataDto = {
                items: rawSensorData.Items ?? [],
                pageNumber: rawSensorData.PageNumber,
                pageSize: rawSensorData.PageSize,
                totalCount: rawSensorData.TotalCount,
                totalPages: rawSensorData.TotalPages,
            };

            setSensorData(mappedSensorData);
            } else {
            setSensorData({
                items: [],
                pageNumber: 1,
                pageSize: 10,
                totalCount: 0,
                totalPages: 1,
            });
            }

        } catch (error) {
            toast.error("Sensor data fetching failed", {
                description: "An error occured while trying to sensor data.",
            });
    } finally {
        setIsSensorDataLoading(false)
        }
    }

    const registerSensorDataUpdates = () => {
        return onMessage<WebsocketMessage_1>(WebsocketEvents.BroadcastDeviceSensorDataUpdate, (dto) => {
            const updatedSensorData = dto.data as SensorDataDto;
            console.log(dto)
            if (!updatedSensorData) return;

            setSensorData(prev => {
                if (!prev) {
                    return {
                        items: [updatedSensorData],
                        pageNumber: 1,
                        pageSize: 10,
                        totalCount: 1,
                        totalPages: 1,
                    };
                }

                const pageSize = prev.pageSize ?? 10;
                const totalCount = prev.totalCount ?? 0;

                if (prev.pageNumber !== 1) {
                    return prev;
                }

                const newItems = [updatedSensorData, ...(prev.items ?? [])];
                const trimmedItems = newItems.slice(0, pageSize);

                return {
                    ...prev,
                    items: trimmedItems,
                    totalCount: totalCount + 1,
                    totalPages: Math.ceil((totalCount + 1) / pageSize),
                };
            });
        });
    };

 
    // lad os bare køre requestdevices på mount
    useEffect(() => {
        if (readyState !== 1) return;

        requestDevices();

        const cleanupDeviceConnectionUpdates = registerDeviceConnectionUpdates();
        const cleanupSensorDataUpdates = registerSensorDataUpdates();

        // cleanup op på unmount eller readyState
        return () => {
            if (cleanupDeviceConnectionUpdates) cleanupDeviceConnectionUpdates();
            if (cleanupSensorDataUpdates) cleanupSensorDataUpdates();
        };
    }, [readyState]);

  return {
    requestDevices,
    requestSensorDataForDevice,
    sensorData,
    getDevicesArray,
    devices,
    iseDevicesLoading,
    isSensorDataLoading,
  };
}