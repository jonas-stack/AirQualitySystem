import { ClientRequestDeviceList, DeviceDto, TimePeriod, WebsocketEvents, WebsocketMessage_1 } from "@/generated-client";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useDeviceData() {
    const [devices, setDevices] = useState<Record<string, DeviceDto>>({});
    const [iseDevicesLoading, setIsDevicesLoading] = useState(true)
    const {sendRequest, readyState} = useWsClient()

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

    const getDevicesArray = (): DeviceDto[] => Object.values(devices);
 
    // lad os bare køre requestdevices på mount
    useEffect(() => {
        if (readyState !== 1)
            return;

        requestDevices();
    }, [readyState])

  return {
    requestDevices,
    getDevicesArray,
    devices,
    iseDevicesLoading,
  };
}