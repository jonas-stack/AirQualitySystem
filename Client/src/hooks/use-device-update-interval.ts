import { DeviceDto, DeviceIntervalUpdateDto, RequestAirQualityData, ServerResponseAirQualityData, ServerResponseDeviceUpdateInterval, WebsocketEvents } from "@/generated-client";
import { useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useDeviecUpdateInterval() {
    const [success, setSuccess] = useState<boolean>(false)
    const {sendRequest} = useWsClient()

    const updateDeviceInterval = async (device: DeviceDto, updatedInterval: number) => {
        setSuccess(false)

        if (device == null)
            return toast.error("Device is not valid");

        if (device.device_id == null)
            return toast.error("Device ID is not valid")

        const updateInterval: DeviceIntervalUpdateDto = {
            eventType: WebsocketEvents.DeviceIntervalUpdateDto,
            deviceId: device.device_id,
            interval: updatedInterval
        }

        try {
        const updatedResult: ServerResponseDeviceUpdateInterval = await sendRequest<DeviceIntervalUpdateDto, ServerResponseDeviceUpdateInterval>(
            updateInterval,
            WebsocketEvents.ServerResponseDeviceUpdateInterval,
        )

        const success = updatedResult.success ?? false;
        setSuccess(success)

        if (success) {
            toast.success("Interval updated successfully", {
                action: { label: "OK", onClick: () => {} },
            })
            device.updateInterval = updatedInterval;
        }

        } catch (error) {
            toast.error("Interval amount not valid", {
                description: "The interval amount is not valid.",
            });
        }
    }

  return {
    updateDeviceInterval,
    success,
  };
}