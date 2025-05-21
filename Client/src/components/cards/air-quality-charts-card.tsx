"use client"

import { useEffect, useState } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import {
  type ChartConfig,
  ChartContainer,
  ChartTooltip,
  ChartTooltipContent,
  ChartLegend,
  ChartLegendContent,
} from "@/components/ui/chart"
import {
  Line,
  LineChart,
  Area,
  AreaChart,
  CartesianGrid,
  XAxis,
  YAxis,
  ResponsiveContainer,
  BarChart,
  Bar,
} from "recharts"
import { CloudRainIcon, RefreshCcw } from "lucide-react"
import { Button } from "@/components/ui/button"
import { useWsClient } from "ws-request-hook"
import { TimePeriod } from "@/generated-client"
import { useGraphData } from "@/hooks"
import { Spinner } from "@/components/ui/spinner"

type ChartConfigItem = {
  label: string
  color: string
  gradientId?: string
}

const climateChartConfig: Record<string, ChartConfigItem> = {
  temperature: {
    label: "Temperature (°C)",
    color: "var(--chart-1)",
  },
  humidity: {
    label: "Humidity (%)",
    color: "var(--chart-2)",
  },
} satisfies ChartConfig

const airQualityConfig: Record<string, ChartConfigItem> = {
  airquality: {
    label: "Air quality",
    color: "var(--chart-1)",
  },
} satisfies ChartConfig

const barChartConfig: Record<string, ChartConfigItem> = {
  pm25: {
    label: "PM2.5",
    color: "var(--chart-5)",
  },
} satisfies ChartConfig

interface AirQualityChartsCardProps {
  className?: string
  onRefresh?: () => void
  lastUpdated?: Date
}

export function AirQualityChartsCard({
  className = "",
  onRefresh,
  lastUpdated = new Date(),
}: AirQualityChartsCardProps) {
  const [currentTab, setCurrentTab] = useState(["temperature", "humidity"])
  const [timePeriod, setTimePeriod] = useState<TimePeriod>(TimePeriod.Daily)
  const { readyState } = useWsClient()

  const { requestGraphData, chartData, isLoading } = useGraphData()

  useEffect(() => {
    if (readyState === 1) {
      requestGraphData(currentTab, timePeriod)
    }
  }, [timePeriod, currentTab, readyState])

  const handleTimeShift = (time: string) => {
    setTimePeriod(time as TimePeriod)
  }

  const handleTabChange = (tab: string) => {
    setTimePeriod(TimePeriod.Daily)

    switch (tab) {
      case "climate":
        setCurrentTab(["temperature", "humidity"])
        break
      case "airquality":
        setCurrentTab(["airquality"])
        break
      case "pm25":
        setCurrentTab(["pm25"])
        break
      default:
        break
    }
  }

  const handleRefresh = () => {
    if (onRefresh) {
      requestGraphData(currentTab, timePeriod)
    }
  }

  const formatLastUpdated = (date: Date) => {
    return date.toLocaleString("en-US", {
      hour: "numeric",
      minute: "numeric",
      hour12: true,
      month: "short",
      day: "numeric",
    })
  }

  const ChartLoading = () => (
    <div className="flex items-center justify-center w-full h-full bg-card/50 rounded-md">
      <div className="flex flex-col items-center gap-3">
        <Spinner size="lg" className="text-primary" />
        <p className="text-sm text-muted-foreground">Loading chart data...</p>
      </div>
    </div>
  )

  return (
    <Card className={`${className} flex flex-col`}>
      <CardHeader className="flex flex-row items-center justify-between pb-2">
        <div>
          <CardTitle className="flex gap-2 text-lg font-bold">
            <CloudRainIcon className="text-yellow-600" /> Air Quality Metrics
          </CardTitle>
          <CardDescription>Last updated: {formatLastUpdated(lastUpdated)}</CardDescription>
        </div>
        <Button variant="ghost" size="icon" onClick={handleRefresh}>
          <RefreshCcw className={`h-4 w-4 ${isLoading ? "animate-spin" : ""}`} />
          <span className="sr-only">Refresh data</span>
        </Button>
      </CardHeader>
      <CardContent className="p-0 flex-1 flex flex-col">
        <Tabs defaultValue="climate" className="w-full h-full flex flex-col" onValueChange={handleTabChange}>
          <div className="px-4">
            <TabsList className="grid w-full grid-cols-3">
              <TabsTrigger value="climate">Climate</TabsTrigger>
              <TabsTrigger value="airquality">Air Quality</TabsTrigger>
              <TabsTrigger value="pm25">PM25</TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="climate" className="mt-0 pt-4 flex-1 flex flex-col">
            <div className="px-4 pb-2">
              <Tabs defaultValue={TimePeriod.Daily} onValueChange={handleTimeShift}>
                <TabsList className="grid w-full grid-cols-3">
                  <TabsTrigger value={TimePeriod.Daily}>Daily</TabsTrigger>
                  <TabsTrigger value={TimePeriod.Weekly}>Weekly</TabsTrigger>
                  <TabsTrigger value={TimePeriod.Monthly}>Monthly</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
            <div className="py-4 flex-1">
              {isLoading ? (
                <ChartLoading />
              ) : (
                <ChartContainer config={climateChartConfig} className="h-72 md:h-80 lg:h-96 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <LineChart data={chartData} margin={{ top: 5, right: 50, left: 0, bottom: 5 }}>
                      <CartesianGrid strokeDasharray="3 3" vertical={false} />
                      <XAxis dataKey="time" tickLine={false} axisLine={false} tickMargin={10} />
                      <YAxis tickLine={false} axisLine={false} tickMargin={10} />
                      <ChartTooltip content={<ChartTooltipContent />} />
                      <ChartLegend content={<ChartLegendContent />} />
                      {currentTab.map((key) => (
                        <Line
                          key={key}
                          type="natural"
                          dataKey={key}
                          fillOpacity={0.4}
                          fill={climateChartConfig[key]?.color ?? "var(--chart-1)"}
                          stroke={climateChartConfig[key]?.color ?? "var(--chart-1)"}
                          strokeWidth={2}
                          dot={{ r: 3 }}
                          activeDot={{ r: 5 }}
                        />
                      ))}
                    </LineChart>
                  </ResponsiveContainer>
                </ChartContainer>
              )}
            </div>
          </TabsContent>

          <TabsContent value="airquality" className="mt-0 pt-4 flex-1 flex flex-col">
            <div className="px-4 pb-2">
              <Tabs defaultValue={TimePeriod.Daily} onValueChange={handleTimeShift}>
                <TabsList className="grid w-full grid-cols-3">
                  <TabsTrigger value={TimePeriod.Daily}>Daily</TabsTrigger>
                  <TabsTrigger value={TimePeriod.Weekly}>Weekly</TabsTrigger>
                  <TabsTrigger value={TimePeriod.Monthly}>Monthly</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
            <div className="py-4 flex-1">
              {isLoading ? (
                <ChartLoading />
              ) : (
                <ChartContainer config={airQualityConfig} className="h-72 md:h-80 lg:h-96 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <AreaChart data={chartData} margin={{ top: 5, right: 50, left: 0, bottom: 5 }}>
                      <defs>
                        <linearGradient id="colorTemp" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="5%" stopColor="var(--chart-1)" stopOpacity={0.8} />
                          <stop offset="95%" stopColor="var(--chart-1)" stopOpacity={0.1} />
                        </linearGradient>
                        <linearGradient id="colorHumidity" x1="0" y1="0" x2="0" y2="1">
                          <stop offset="5%" stopColor="var(--chart-2)" stopOpacity={0.8} />
                          <stop offset="95%" stopColor="var(--chart-2)" stopOpacity={0.1} />
                        </linearGradient>
                      </defs>
                      <CartesianGrid strokeDasharray="3 3" vertical={false} />
                      <XAxis dataKey="time" tickLine={false} axisLine={false} tickMargin={10} />
                      <YAxis tickLine={false} axisLine={false} tickMargin={10} />
                      <ChartTooltip content={<ChartTooltipContent />} />
                      <ChartLegend content={<ChartLegendContent />} />
                      {currentTab.map((key) => (
                        <Area
                          key={key}
                          type="monotone"
                          dataKey={key}
                          fillOpacity={0.4}
                          fill={"url(#colorTemp)"}
                          stroke={airQualityConfig[key]?.color ?? "var(--chart-1)"}
                          strokeWidth={2}
                          dot={{ r: 3 }}
                          activeDot={{ r: 5 }}
                        />
                      ))}
                    </AreaChart>
                  </ResponsiveContainer>
                </ChartContainer>
              )}
            </div>
          </TabsContent>

          <TabsContent value="pm25" className="mt-0 pt-4 flex-1 flex flex-col">
            <div className="px-4 flex-1">
              {isLoading ? (
                <ChartLoading />
              ) : (
                <ChartContainer config={barChartConfig} className="h-72 md:h-80 lg:h-96 w-full">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart
                      accessibilityLayer
                      data={chartData}
                      layout="vertical"
                      margin={{
                        left: -20,
                      }}
                    >
                      <XAxis type="number" dataKey="pm25" hide />
                      <YAxis
                        type="category"
                        dataKey="time"
                        tickLine={false}
                        tickMargin={10}
                        axisLine={false}
                        tickFormatter={(value) => value.slice(0, 1)}
                      />
                      <ChartTooltip cursor={false} content={<ChartTooltipContent hideLabel />} />
                      <Bar dataKey="pm25" fill="var(--chart-4)" radius={5} />
                    </BarChart>
                  </ResponsiveContainer>
                </ChartContainer>
              )}
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  )
}
