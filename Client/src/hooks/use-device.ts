import { ClientRequestDeviceList, ClientRequestDeviceStats, ClientRequestSensorData, DeviceDto, DeviceStatsDto, PagedResultOfSensorDataDto, SensorDataDto, ServerResponseDeviceList, ServerResponseDeviceStats, TimePeriod, WebsocketEvents, WebsocketMessage_1 } from "@/generated-client";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useDeviceData() {
    const { sendRequest, onMessage, readyState } = useWsClient()

    const [devices, setDevices] = useState<Record<string, DeviceDto>>({});
    const [iseDevicesLoading, setIsDevicesLoading] = useState(true)

    const [deviceStats, setDeviceStats] = useState<DeviceStatsDto>();
    const [isDeviceStatsLoading, setIsDeviceStatsLoading] = useState(true)

    const getDevicesArray = (): DeviceDto[] => Object.values(devices);

    const requestDevices = async () => {
        setIsDevicesLoading(true)

        const requestDevices: ClientRequestDeviceList = {
            eventType: WebsocketEvents.ClientRequestDeviceList
        }

        try {
            const deviceResult: ServerResponseDeviceList = await sendRequest<ClientRequestDeviceList, ServerResponseDeviceList>(
                requestDevices,
                WebsocketEvents.ServerResponseDeviceList,
            )

            const deviceList = deviceResult?.deviceList ?? [];

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

    const requestDeviceStats = async () => {
        setIsDeviceStatsLoading(true)

        const requestStats: ClientRequestDeviceStats = {
            eventType: WebsocketEvents.ClientRequestDeviceStats
        }

        try {
            
            const statsResult: ServerResponseDeviceStats = await sendRequest<ClientRequestDeviceStats, ServerResponseDeviceStats>(
                requestStats,
                WebsocketEvents.ServerResponseDeviceStats,
            )

            const rawStats = statsResult?.stats ?? {};

            const parsedStats: DeviceStatsDto = {
                allTimeMeasurements: rawStats?.allTimeMeasurements ?? 0,
                connectedDevices: rawStats?.connectedDevices ?? 0,
                disconnectionsLast24Hours: rawStats?.disconnectionsLast24Hours ?? 0,
            };

            setDeviceStats(parsedStats);
        } catch (error) {
            toast.error("Device stats fetching failed", {
                description: "An error occured while trying to fetch device stats.",
            });
    } finally {
        setIsDeviceStatsLoading(false)
        }
    }

    // lad os bare køre requestdevices på mount
    useEffect(() => {
        if (readyState !== 1) return;

        requestDevices();
        requestDeviceStats();
        const cleanupDeviceConnectionUpdates = registerDeviceConnectionUpdates();

        // cleanup op på unmount eller readyState
        return () => {
            if (cleanupDeviceConnectionUpdates) cleanupDeviceConnectionUpdates();
        };
    }, [readyState]);

  return {
    requestDevices,
    getDevicesArray,
    devices,
    deviceStats,
    isDeviceStatsLoading,
    iseDevicesLoading,
  };
}