"use client"

import { Area, AreaChart } from "recharts"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { type ChartConfig, ChartContainer, ChartTooltip } from "@/components/ui/chart"
import { useEffect, useState } from "react"
import { useWsClient } from "ws-request-hook"
import { type RequestAirQualityData, TimePeriod, WebsocketEvents, type WebsocketMessage_1 } from "@/generated-client"
import { Thermometer } from "lucide-react"
import { toast } from "sonner"
import { Spinner } from "@/components/ui/spinner"

export default function AirQualityTemperatureCard() {
  const { onMessage, readyState, sendRequest } = useWsClient()

  const [chartData, setChartData] = useState<any[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [lastFetch, setLastFetch] = useState<Date>()

  const requestGraphData = async (requestTypes: any[], timePeriod: TimePeriod) => {
    setIsLoading(true)
    setLastFetch(undefined)

    const requestData: RequestAirQualityData = {
      eventType: WebsocketEvents.RequestAirQualityData,
      timePeriod: timePeriod,
      data: requestTypes,
    }

    try {
      const graphResult: WebsocketMessage_1 = await sendRequest<RequestAirQualityData, WebsocketMessage_1>(
        requestData,
        WebsocketEvents.GraphGetMeasurement,
      )

      setChartData((graphResult as any)?.Data?.Data ?? [])
      console.log(graphResult)
    } catch (error) {
      toast.error("Chart data failed", {
        description: "An error occured while trying to fetching chart data.",
      })
      setLastFetch(undefined)
    } finally {
      setIsLoading(false)
    }
  }

  const chartConfig = {
    temperature: {
      label: "Amount",
      color: "var(--chart-2)",
    },
  } satisfies ChartConfig

  useEffect(() => {
    if (readyState !== 1) return

    requestGraphData(["temperature"], TimePeriod.Daily)

    const reactToMessageSetup = onMessage<WebsocketMessage_1>(WebsocketEvents.GraphTemperatureUpdate, (dto) => {
      setChartData((prevData) => {
        const newEntry = {
          time: dto.data?.label,
          temperature: dto.data?.amount ?? 0,
        }

        const updatedData = [...prevData, newEntry]

        return updatedData.length > 6 ? updatedData.slice(-6) : updatedData
      })
    })

    return () => reactToMessageSetup()
  }, [readyState])

  const newest = chartData.at(-1)

  const ChartLoading = () => (
    <div className="absolute inset-0 flex items-center justify-center bg-card/50 rounded-md z-10">
      <div className="flex flex-col items-center gap-2">
        <Spinner size="md" className="text-primary" />
        <p className="text-xs text-muted-foreground">Loading data...</p>
      </div>
    </div>
  )

  return (
    <Card className="overflow-hidden h-60 border shadow-lg">
      <div className="h-full flex flex-col">
        <CardHeader>
          <div className="flex items-center justify-between">
            <div>
              <CardTitle className="text-lg font-bold flex items-center gap-2">
                <Thermometer className="h-5 w-5 text-primary" />
                Current Temperature
              </CardTitle>
              <p className="text-sm text-muted-foreground">Measured in celsius</p>
            </div>
            <div className="flex items-center bg-primary/10 px-3 py-1 rounded-full">
              <span className="text-2xl font-bold text-primary">
                {isLoading ? "--" : (newest?.temperature ?? "--")}°
              </span>
            </div>
          </div>
        </CardHeader>
        <CardContent className="p-0 h-full mt-10 flex-1 flex flex-col justify-end">
          <div className="relative h-full w-full">
            {isLoading && <ChartLoading />}
            <ChartContainer config={chartConfig} className="w-full h-32">
              <AreaChart
                accessibilityLayer
                data={chartData}
                margin={{
                  left: 0,
                  right: 0,
                  top: 5,
                  bottom: 5,
                }}
              >
                <defs>
                  <linearGradient id="fillAmount" x1="0" y1="0" x2="0" y2="1">
                    <stop offset="5%" stopColor="var(--color-chart-3)" stopOpacity={0.8} />
                    <stop offset="95%" stopColor="var(--color-chart-3)" stopOpacity={0.1} />
                  </linearGradient>
                </defs>
                <Area
                  dataKey="temperature"
                  type="natural"
                  fill="url(#fillAmount)"
                  fillOpacity={0.4}
                  stroke="var(--color-chart-3)"
                  strokeWidth={2}
                  stackId="a"
                />
                <ChartTooltip
                  content={({ active, payload }) => {
                    if (active && payload && payload.length) {
                      return (
                        <div className="bg-background border rounded-md shadow-sm p-2 text-xs">
                          <p className="font-medium">{payload[0].payload.time}</p>
                          <p className="text-primary">{payload[0].value}°C</p>
                        </div>
                      )
                    }
                    return null
                  }}
                />
              </AreaChart>
            </ChartContainer>
          </div>
        </CardContent>
      </div>
    </Card>
  )
}
