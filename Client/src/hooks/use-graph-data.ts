import { RequestAirQualityData, TimePeriod, WebsocketEvents, WebsocketMessage_1 } from "@/generated-client";
import { useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useGraphData() {
    const [chartData, setChartData] = useState<any[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const {sendRequest} = useWsClient()

    const requestGraphData = async (requestTypes: any[], timePeriod: TimePeriod) => {
        setIsLoading(true)

        const requestData: RequestAirQualityData = {
            eventType: WebsocketEvents.RequestAirQualityData,
            timePeriod: timePeriod,
            data: requestTypes
        }

        try {
        const signInResult: WebsocketMessage_1 = await sendRequest<RequestAirQualityData, WebsocketMessage_1>(
            requestData,
            WebsocketEvents.GraphTemperatureUpdate,
        )

        setChartData(((signInResult as any)?.Data?.Data) ?? []);
        } catch (error) {
        toast.error("Chart data failed", {
            description: "An error occured while trying to fetching chart data.",
        });      
    } finally {
        setIsLoading(false)
        }
    }

  return {
    requestGraphData,
    chartData,
    isLoading,
  };
}