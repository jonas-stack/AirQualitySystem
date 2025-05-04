import { useState } from "react"
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
  Pie,
  PieChart,
  CartesianGrid,
  XAxis,
  YAxis,
  ResponsiveContainer,
  Cell,
  Radar,
  RadarChart,
  PolarGrid,
  PolarAngleAxis,
} from "recharts"
import { RefreshCcw } from "lucide-react"
import { Button } from "@/components/ui/button"

// Sample air quality data - replace with your actual data
const airQualityData = {
  daily: [
    { time: "00:00", pm25: 12, pm10: 25, voc: 120, co2: 650, temperature: 21, humidity: 45 },
    { time: "04:00", pm25: 10, pm10: 22, voc: 110, co2: 600, temperature: 20, humidity: 44 },
    { time: "08:00", pm25: 15, pm10: 30, voc: 150, co2: 750, temperature: 22, humidity: 48 },
    { time: "12:00", pm25: 18, pm10: 35, voc: 180, co2: 850, temperature: 24, humidity: 50 },
    { time: "16:00", pm25: 14, pm10: 28, voc: 140, co2: 700, temperature: 23, humidity: 47 },
    { time: "20:00", pm25: 11, pm10: 24, voc: 130, co2: 680, temperature: 22, humidity: 46 },
  ],
  weekly: [
    { day: "Mon", pm25: 14, pm10: 28, voc: 140, co2: 700, temperature: 22, humidity: 47 },
    { day: "Tue", pm25: 12, pm10: 25, voc: 130, co2: 650, temperature: 21, humidity: 45 },
    { day: "Wed", pm25: 18, pm10: 35, voc: 170, co2: 800, temperature: 23, humidity: 49 },
    { day: "Thu", pm25: 15, pm10: 30, voc: 150, co2: 720, temperature: 22, humidity: 46 },
    { day: "Fri", pm25: 20, pm10: 40, voc: 190, co2: 850, temperature: 24, humidity: 50 },
    { day: "Sat", pm25: 10, pm10: 22, voc: 120, co2: 600, temperature: 21, humidity: 44 },
    { day: "Sun", pm25: 8, pm10: 18, voc: 100, co2: 550, temperature: 20, humidity: 42 },
  ],
  monthly: [
    { month: "Jan", pm25: 12, pm10: 25, voc: 130, co2: 650, temperature: 20, humidity: 45 },
    { month: "Feb", pm25: 14, pm10: 28, voc: 140, co2: 700, temperature: 21, humidity: 46 },
    { month: "Mar", pm25: 10, pm10: 22, voc: 120, co2: 600, temperature: 22, humidity: 47 },
    { month: "Apr", pm25: 16, pm10: 32, voc: 160, co2: 750, temperature: 23, humidity: 48 },
    { month: "May", pm25: 18, pm10: 35, voc: 170, co2: 800, temperature: 24, humidity: 49 },
    { month: "Jun", pm25: 15, pm10: 30, voc: 150, co2: 720, temperature: 25, humidity: 50 },
  ],
  pollutantDistribution: [
    { name: "PM2.5", value: 35 },
    { name: "PM10", value: 25 },
    { name: "VOCs", value: 20 },
    { name: "CO2", value: 15 },
    { name: "Other", value: 5 },
  ],
  airQualityIndex: [
    { metric: "PM2.5", value: 75 },
    { metric: "PM10", value: 65 },
    { metric: "VOCs", value: 80 },
    { metric: "CO2", value: 50 },
    { metric: "Temperature", value: 90 },
    { metric: "Humidity", value: 70 },
  ],
}

// Chart configurations
const lineChartConfig = {
  pm25: {
    label: "PM2.5 (μg/m³)",
    color: "var(--chart-1)",
  },
  pm10: {
    label: "PM10 (μg/m³)",
    color: "var(--chart-2)",
  },
  voc: {
    label: "VOC (ppb)",
    color: "var(--chart-3)",
  },
  co2: {
    label: "CO2 (ppm)",
    color: "var(--chart-4)",
  },
} satisfies ChartConfig

const climateChartConfig = {
  temperature: {
    label: "Temperature (°C)",
    color: "var(--chart-1)",
  },
  humidity: {
    label: "Humidity (%)",
    color: "var(--chart-2)",
  },
} satisfies ChartConfig

const pieChartConfig = {
  pm25: {
    label: "PM2.5",
    color: "var(--chart-1)",
  },
  pm10: {
    label: "PM10",
    color: "var(--chart-2)",
  },
  voc: {
    label: "VOCs",
    color: "var(--chart-3)",
  },
  co2: {
    label: "CO2",
    color: "var(--chart-4)",
  },
  other: {
    label: "Other",
    color: "var(--chart-5)",
  },
} satisfies ChartConfig

const radarChartConfig = {
  value: {
    label: "Index Value",
    color: "var(--chart-1)",
  },
} satisfies ChartConfig

const COLORS = [
  "var(--chart-1)",
  "var(--chart-2)",
  "var(--chart-3)",
  "var(--chart-4)",
  "var(--chart-5)",
]

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
  const [timeRange, setTimeRange] = useState("daily")
  const [chartType, setChartType] = useState("pollutants")

  // Get the appropriate data based on the selected time range
  const getTimeData = () => {
    switch (timeRange) {
      case "daily":
        return airQualityData.daily
      case "weekly":
        return airQualityData.weekly
      case "monthly":
        return airQualityData.monthly
      default:
        return airQualityData.daily
    }
  }

  // Get the appropriate x-axis key based on the selected time range
  const getTimeKey = () => {
    switch (timeRange) {
      case "daily":
        return "time"
      case "weekly":
        return "day"
      case "monthly":
        return "month"
      default:
        return "time"
    }
  }

  const handleRefresh = () => {
    if (onRefresh) {
      onRefresh()
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

  return (
    <Card className={`${className} flex flex-col`}>
      <CardHeader className="flex flex-row items-center justify-between pb-2">
        <div>
          <CardTitle>Air Quality Metrics</CardTitle>
          <CardDescription>Last updated: {formatLastUpdated(lastUpdated)}</CardDescription>
        </div>
        <Button variant="ghost" size="icon" onClick={handleRefresh}>
          <RefreshCcw className="h-4 w-4" />
          <span className="sr-only">Refresh data</span>
        </Button>
      </CardHeader>
      <CardContent className="p-0 flex-1 flex flex-col">
        <Tabs defaultValue="pollutants" className="w-full h-full flex flex-col" onValueChange={setChartType}>
          <div className="px-4">
            <TabsList className="grid w-full grid-cols-4">
              <TabsTrigger value="pollutants">Pollutants</TabsTrigger>
              <TabsTrigger value="climate">Climate</TabsTrigger>
              <TabsTrigger value="distribution">Distribution</TabsTrigger>
              <TabsTrigger value="aqi">AQI Radar</TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="pollutants" className="mt-0 pt-4 flex-1 flex flex-col">
            <div className="px-4 pb-2">
              <Tabs defaultValue="daily" onValueChange={setTimeRange}>
                <TabsList className="grid w-full grid-cols-3">
                  <TabsTrigger value="daily">Daily</TabsTrigger>
                  <TabsTrigger value="weekly">Weekly</TabsTrigger>
                  <TabsTrigger value="monthly">Monthly</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
            <div className="py-4 flex-1">
              <ChartContainer config={lineChartConfig} className="h-72 md:h-80 lg:h-96 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <LineChart data={getTimeData()} margin={{ top: 5, right: 50, left: 0, bottom: 5 }}>
                    <CartesianGrid strokeDasharray="3 3" vertical={false} />
                    <XAxis dataKey={getTimeKey()} tickLine={false} axisLine={false} tickMargin={10} />
                    <YAxis tickLine={false} axisLine={false} tickMargin={10} />
                    <ChartTooltip content={<ChartTooltipContent />} />
                    <ChartLegend content={<ChartLegendContent />} />
                    <Line
                      type="monotone"
                      dataKey="pm25"
                      stroke="var(--chart-1)"
                      strokeWidth={2}
                      dot={{ r: 3 }}
                      activeDot={{ r: 5 }}
                    />
                    <Line
                      type="monotone"
                      dataKey="pm10"
                      stroke="var(--chart-2)"
                      strokeWidth={2}
                      dot={{ r: 3 }}
                      activeDot={{ r: 5 }}
                    />
                    <Line
                      type="monotone"
                      dataKey="voc"
                      stroke="var(--chart-3)"
                      strokeWidth={2}
                      dot={{ r: 3 }}
                      activeDot={{ r: 5 }}
                    />
                    <Line
                      type="monotone"
                      dataKey="co2"
                      stroke="var(--chart-4)"
                      strokeWidth={2}
                      dot={{ r: 3 }}
                      activeDot={{ r: 5 }}
                    />
                  </LineChart>
                </ResponsiveContainer>
              </ChartContainer>
            </div>
          </TabsContent>

          <TabsContent value="climate" className="mt-0 pt-4 flex-1 flex flex-col">
            <div className="px-4 pb-2">
              <Tabs defaultValue="daily" onValueChange={setTimeRange}>
                <TabsList className="grid w-full grid-cols-3">
                  <TabsTrigger value="daily">Daily</TabsTrigger>
                  <TabsTrigger value="weekly">Weekly</TabsTrigger>
                  <TabsTrigger value="monthly">Monthly</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
            <div className="py-4 flex-1">
              <ChartContainer config={climateChartConfig} className="h-72 md:h-80 lg:h-96 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <AreaChart data={getTimeData()} margin={{ top: 5, right: 50, left: 0, bottom: 5 }}>
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
                    <XAxis dataKey={getTimeKey()} tickLine={false} axisLine={false} tickMargin={10} />
                    <YAxis tickLine={false} axisLine={false} tickMargin={10} />
                    <ChartTooltip content={<ChartTooltipContent />} />
                    <ChartLegend content={<ChartLegendContent />} />
                    <Area
                      type="monotone"
                      dataKey="temperature"
                      stroke="var(--chart-1)"
                      fillOpacity={1}
                      fill="url(#colorTemp)"
                    />
                    <Area
                      type="monotone"
                      dataKey="humidity"
                      stroke="var(--chart-2)"
                      fillOpacity={1}
                      fill="url(#colorHumidity)"
                    />
                  </AreaChart>
                </ResponsiveContainer>
              </ChartContainer>
            </div>
          </TabsContent>

          <TabsContent value="distribution" className="mt-0 pt-4 flex-1 flex flex-col">
            <div className="px-4 flex-1">
              <ChartContainer config={pieChartConfig} className="h-72 md:h-80 lg:h-96 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={airQualityData.pollutantDistribution}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      outerRadius={80}
                      fill="#8884d8"
                      dataKey="value"
                      label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                    >
                      {airQualityData.pollutantDistribution.map((entry, index) => (
                        <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                      ))}
                    </Pie>
                    <ChartTooltip content={<ChartTooltipContent />} />
                  </PieChart>
                </ResponsiveContainer>
              </ChartContainer>
            </div>
          </TabsContent>

          <TabsContent value="aqi" className="mt-0 pt-4 flex-1 flex flex-col">
            <div className="px-4 flex-1">
              <ChartContainer config={radarChartConfig} className="h-72 md:h-80 lg:h-96 w-full">
                <ResponsiveContainer width="100%" height="100%">
                  <RadarChart outerRadius={80} data={airQualityData.airQualityIndex}>
                    <PolarGrid />
                    <PolarAngleAxis dataKey="metric" />
                    <Radar
                      name="Air Quality Index"
                      dataKey="value"
                      stroke="var(--chart-1)"
                      fill="var(--chart-1)"
                      fillOpacity={0.5}
                    />
                    <ChartTooltip content={<ChartTooltipContent />} />
                  </RadarChart>
                </ResponsiveContainer>
              </ChartContainer>
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  )
}