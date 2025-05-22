import { useEffect, useState } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { type ChartConfig, ChartContainer, ChartTooltip, ChartTooltipContent } from "@/components/ui/chart"
import {
  Line,
  LineChart,
  Bar,
  BarChart,
  XAxis,
  CartesianGrid,
  AreaChart,
  Area,
  LabelList,
} from "recharts"
import { CloudRainIcon, RefreshCcw } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useWsClient } from "ws-request-hook"
import { TimePeriod } from "@/generated-client"
import { Spinner } from "../ui/spinner"
import { useGraphData } from "@/hooks"
import { formatLastUpdated } from "@/lib/time-formatter"

type ChartConfigItem = {
  label: string
  color: string
}

const lineChartConfig: Record<string, ChartConfigItem> = {
  temperature: {
    label: "Temperature (°C)",
    color: "var(--chart-1)",
  },
  humidity: {
    label: "Humidity (%)",
    color: "var(--chart-2)",
  },
} satisfies ChartConfig

const barChartConfig: Record<string, ChartConfigItem> = {
  airquality: {
    label: "Air quality",
    color: "var(--chart-1)",
  },
} satisfies ChartConfig

const pm25Config: Record<string, ChartConfigItem> = {
  pm25: {
    label: "PM25",
    color: "var(--chart-4)",
  },
} satisfies ChartConfig

interface SimpleAirQualityChartsCardProps {
  className?: string
  onRefresh?: () => void
}

export function SimpleAirQualityChartsCard({ className = "", onRefresh }: SimpleAirQualityChartsCardProps) {
  const [currentTab, setCurrentTab] = useState(["temperature", "humidity"])
  const [timePeriod, setTimePeriod] = useState<TimePeriod>(TimePeriod.Daily)
  const { readyState } = useWsClient();

  const { requestGraphData, chartData, isLoading, lastFetch } = useGraphData();

  useEffect(() => {
    if (readyState === 1) {
      requestGraphData(currentTab, timePeriod);
    }
  }, [timePeriod, currentTab, readyState]);

  const handleTimeShift = (time: string) => {
    setTimePeriod(time as TimePeriod)
  }

  const handleTabChange = (tab: string) => {
    switch (tab) {
      case "general":
        setTimePeriod(TimePeriod.Daily)
        setCurrentTab(["temperature", "humidity"])
        break
      case "airquality":
        setTimePeriod(TimePeriod.Weekly)
        setCurrentTab(["airquality"])
        break
      case "pm25":
        setTimePeriod(TimePeriod.Weekly)
        setCurrentTab(["pm25"])
        break
      default:
        break
    }
  }

  const handleRefresh = () => {
    if (onRefresh) {
      requestGraphData(currentTab, timePeriod);
    }
  }

  const ChartLoading = () => (
    <div className="flex items-center justify-center w-full h-[200px] bg-card/50 rounded-md">
      <div className="flex flex-col items-center gap-2">
        <Spinner size="lg" className="text-primary" />
        <p className="text-sm text-muted-foreground">Loading chart data...</p>
      </div>
    </div>
  )

  return (
      <Card className={`${className}`}>
      <CardHeader className="flex flex-row items-center justify-between pb-2">
        <div>
        <CardTitle className="flex gap-2 text-lg font-bold">
          <CloudRainIcon className="text-yellow-600" /> Air Quality Metrics
        </CardTitle>
            <CardDescription>
              {lastFetch === undefined
                ? "No data available"
                : `Last updated: ${formatLastUpdated(lastFetch)}`}
            </CardDescription>    
        </div>
        <Button variant="ghost" size="icon" onClick={handleRefresh} className="h-8 w-8 cursor-pointer">
          <RefreshCcw className={`h-4 w-4 ${isLoading ? "animate-spin" : ""}`} />
          <span className="sr-only">Refresh data</span>
        </Button>
      </CardHeader>
      <CardContent className="p-0">
        <Tabs defaultValue="general" onValueChange={handleTabChange} className="w-full">
          <div className="px-4">
            <TabsList className="grid w-full grid-cols-3">
              <TabsTrigger value="general">General</TabsTrigger>
              <TabsTrigger value="airquality">Air Quality</TabsTrigger>
              <TabsTrigger value="pm25">PM25</TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="general" className="mt-0 pt-2">
            <div className="px-4 pb-2">
              <Tabs defaultValue={TimePeriod.Daily} onValueChange={handleTimeShift}>
                <TabsList className="grid w-full grid-cols-2">
                  <TabsTrigger value={TimePeriod.Daily}>Daily</TabsTrigger>
                  <TabsTrigger value={TimePeriod.Weekly}>Weekly</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
            <div className="px-4 pt-4">
              {isLoading ? (
                <ChartLoading />
              ) : (
                <ChartContainer className="h-60 w-full" config={lineChartConfig}>
                  <AreaChart accessibilityLayer data={chartData} margin={{ top: 5, right: 5, left: 5, bottom: 5 }}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} opacity={0.3} />
                    <XAxis dataKey="time" tickLine={false} axisLine={false} tickMargin={10} />
                    <ChartTooltip content={<ChartTooltipContent />} />
                    {currentTab.map((key) => (
                      <Area
                        key={key}
                        type="natural"
                        dataKey={key}
                        fillOpacity={0.4}
                        fill={lineChartConfig[key]?.color ?? "var(--chart-1)"}
                        stroke={lineChartConfig[key]?.color ?? "var(--chart-1)"}
                        stackId="a"
                        dot={{ r: 3 }}
                        activeDot={{ r: 5 }}
                      />
                    ))}
                  </AreaChart>
                </ChartContainer>
              )}
            </div>
          </TabsContent>

          <TabsContent value="airquality" className="mt-0 pt-2">
            <div className="px-4 pt-8">
              {isLoading ? (
                <ChartLoading />
              ) : (
                <ChartContainer className="h-60 w-full" config={barChartConfig}>
                  <BarChart accessibilityLayer data={chartData} margin={{ top: 5, right: 10, left: 0, bottom: 0 }}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} opacity={0.3} />
                    <XAxis dataKey="time" tickLine={false} axisLine={false} tickMargin={10} />
                    <ChartTooltip content={<ChartTooltipContent />} />
                    {currentTab.map((key) => (
                      <Bar
                        key={key}
                        dataKey={key}
                        fill={barChartConfig[key]?.color ?? "var(--color-default)"}
                        radius={[4, 4, 0, 0]}
                        barSize={20}
                      />
                    ))}
                  </BarChart>
                </ChartContainer>
              )}
            </div>
          </TabsContent>

          <TabsContent value="pm25" className="mt-0 pt-2">
            <div className="px-4 pt-4">
              {isLoading ? (
                <ChartLoading />
              ) : (
                <ChartContainer className="h-60 w-full" config={pm25Config}>
                  <LineChart accessibilityLayer data={chartData} margin={{ top: 5, right: 5, left: 5, bottom: 5 }}>
                    <CartesianGrid vertical={false}/>
                    <XAxis dataKey="time" tickLine={false} axisLine={false} tickMargin={10}/>
                    <ChartTooltip cursor={false} content={<ChartTooltipContent />} />
                    {currentTab.map((key) => (
                      <Line
                        key={key}
                        type="natural"
                        dataKey={key}
                        stroke={pm25Config[key]?.color ?? "var(--chart-1)"}
                        strokeWidth={2}
                        dot={{ fill: pm25Config[key]?.color ?? "var(--chart-1)" }}
                        activeDot={{ r: 6 }}
                      >
                        <LabelList position="top" offset={12} className="fill-foreground" fontSize={12} />
                      </Line>
                    ))}
                  </LineChart>
                </ChartContainer>
              )}
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  )
}
