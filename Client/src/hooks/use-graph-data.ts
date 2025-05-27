import { RequestAirQualityData, ServerResponseAirQualityData, TimePeriod, WebsocketEvents, WebsocketMessage_1 } from "@/generated-client";
import { useState } from "react";
import { toast } from "sonner";
import { useWsClient } from "ws-request-hook";

export function useGraphData() {
    const [chartData, setChartData] = useState<any[]>([])
    const [isLoading, setIsLoading] = useState(true)
    const [lastFetch, setLastFetch] = useState<Date>()
    const {sendRequest} = useWsClient()

    const requestGraphData = async (requestTypes: any[], timePeriod: TimePeriod) => {
        setIsLoading(true)
        setLastFetch(undefined)

        const requestData: RequestAirQualityData = {
            eventType: WebsocketEvents.RequestAirQualityData,
            timePeriod: timePeriod,
            data: requestTypes
        }

        try {
        const graphResult: ServerResponseAirQualityData = await sendRequest<RequestAirQualityData, ServerResponseAirQualityData>(
            requestData,
            WebsocketEvents.ServerResponseAirQualityData,
        )

        const data = graphResult.data ?? [];

        setChartData(data);
        setLastFetch(new Date())
        } catch (error) {
        toast.error("Chart data fetching failed", {
            description: "An error occured while trying to fetch chart data.",
        });
        setLastFetch(undefined)
    } finally {
        setIsLoading(false)
        }
    }

  return {
    requestGraphData,
    chartData,
    setChartData,
    isLoading,
    lastFetch
  };
}