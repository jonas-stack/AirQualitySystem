import { ClientRequestDeviceList, DeviceDto, TimePeriod, WebsocketEvents, WebsocketMessage_1 } from "@/generated-client";
import { useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useDeviceData() {
    const [devices, setDevices] = useState<DeviceDto[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const {sendRequest} = useWsClient()

    const requestDevices = async () => {
        setIsLoading(true)

        const requestDevices: ClientRequestDeviceList = {
            eventType: WebsocketEvents.ClientRequestDeviceList
        }

        try {
        const deviceResult: WebsocketMessage_1 = await sendRequest<ClientRequestDeviceList, WebsocketMessage_1>(
            requestDevices,
            WebsocketEvents.ServerResponseDeviceList,
        )

        const deviceList = deviceResult?.Data?.DeviceList ?? [];
        setDevices(deviceList as DeviceDto[]);

        console.log(deviceList)
        } catch (error) {
        toast.error("Device fetching failed", {
            description: "An error occured while trying to fetch devices.",
        });
    } finally {
        setIsLoading(false)
        }
    }

  return {
    requestDevices,
    devices,
    isLoading,
  };
}