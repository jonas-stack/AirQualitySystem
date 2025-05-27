import { ClientRequestDeviceHistory, ClientRequestDeviceList, ClientRequestSensorData, DeviceConnectionHistoryDto, DeviceDto, PagedResultOfDeviceConnectionHistoryDto, PagedResultOfSensorDataDto, SensorDataDto, ServerResponseDeviceHistory, TimePeriod, WebsocketEvents, WebsocketMessage_1 } from "@/generated-client";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useDeviceConnectionHistory() {
    const [deviceConnectionHistory, setDeviceConnectionHistory] = useState<PagedResultOfDeviceConnectionHistoryDto | undefined>(undefined);
    const [isDeviceConnectionHistoryLoading, setIsDeviceConnectionHistoryLoading] = useState(true)

    const { sendRequest, onMessage, readyState } = useWsClient()

    const requestDeviceConnectionHistoryForDevice = async (deviceId: string, pageNumber: number, pageSize: number) => {
        setIsDeviceConnectionHistoryLoading(true)

        const requestDevices: ClientRequestDeviceHistory = {
            eventType: WebsocketEvents.ClientRequestDeviceHistory,
            deviceId: deviceId,
            pageNumber: pageNumber,
            pageSize: pageSize
        }

        try {
            const deviceResult: ServerResponseDeviceHistory = await sendRequest<ClientRequestDeviceHistory, ServerResponseDeviceHistory>(
                requestDevices,
                WebsocketEvents.ServerResponseDeviceHistory,
            )

            const rawConnectionHistoryData = deviceResult?.connectionData;

            if (rawConnectionHistoryData) {
                const mappedSensorData: PagedResultOfDeviceConnectionHistoryDto = {
                    items: rawConnectionHistoryData.items ?? [],
                    pageNumber: rawConnectionHistoryData.pageNumber,
                    pageSize: rawConnectionHistoryData.pageSize,
                    totalCount: rawConnectionHistoryData.totalCount,
                    totalPages: rawConnectionHistoryData.totalPages,
                };

                setDeviceConnectionHistory(mappedSensorData);
            } else {
                setDeviceConnectionHistory({
                    items: [],
                    pageNumber: 1,
                    pageSize: 10,
                    totalCount: 0,
                    totalPages: 1,
                });
            }

        } catch (error) {
            toast.error("Device connection history fetching failed", {
                description: "An error occured while trying to device connection history.",
            });
    } finally {
        setIsDeviceConnectionHistoryLoading(false)
        }
    }

    const registerDeviceConnectionHistoryUpdate = () => {
        return onMessage<WebsocketMessage_1>(WebsocketEvents.BroadcastDeviceSensorDataUpdate, (dto) => {
            const updatedSensorData = dto.data as DeviceConnectionHistoryDto;
            if (!updatedSensorData) return;

            setDeviceConnectionHistory(prev => {
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

 
    useEffect(() => {
        if (readyState !== 1) return;

        const cleanupSensorDataUpdates = registerDeviceConnectionHistoryUpdate();

        // cleanup op på unmount eller readyState
        return () => {
            if (cleanupSensorDataUpdates) cleanupSensorDataUpdates();
        };
    }, [readyState]);

  return {
    requestDeviceConnectionHistoryForDevice,
    isDeviceConnectionHistoryLoading,
    deviceConnectionHistory,
  };
}