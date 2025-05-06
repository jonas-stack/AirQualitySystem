import { useState } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { type ChartConfig, ChartContainer, ChartTooltip, ChartTooltipContent } from "@/components/ui/chart"
import {
  Line,
  LineChart,
  Bar,
  BarChart,
  PieChart,
  Pie,
  Cell,
  XAxis,
  CartesianGrid,
  ResponsiveContainer,
} from "recharts"
import { CloudRainIcon, RefreshCcw } from "lucide-react"
import { Button } from "@/components/ui/button"

const airQualityData = {
  daily: [
    { time: "00:00", pm25: 12, aqi: 65 },
    { time: "06:00", pm25: 10, aqi: 60 },
    { time: "12:00", pm25: 18, aqi: 75 },
    { time: "18:00", pm25: 14, aqi: 68 },
  ],
  weekly: [
    { day: "Mon", pm25: 14, aqi: 68 },
    { day: "Tue", pm25: 12, aqi: 65 },
    { day: "Wed", pm25: 18, aqi: 75 },
    { day: "Thu", pm25: 15, aqi: 70 },
    { day: "Fri", pm25: 20, aqi: 80 },
    { day: "Sat", pm25: 10, aqi: 60 },
    { day: "Sun", pm25: 8, aqi: 55 },
  ],
  pollutantDistribution: [
    { name: "PM2.5", value: 35 },
    { name: "PM10", value: 25 },
    { name: "VOCs", value: 20 },
    { name: "CO2", value: 15 },
    { name: "Other", value: 5 },
  ],
}

const lineChartConfig = {
  pm25: {
    label: "PM2.5 (μg/m³)",
    color: "var(--chart-1)",
  },
  aqi: {
    label: "AQI",
    color: "var(--chart-2)",
  },
} satisfies ChartConfig

const barChartConfig = {
  pm25: {
    label: "PM2.5 (μg/m³)",
    color: "var(--chart-1)",
  },
} satisfies ChartConfig

const pieChartConfig = {
  value: {
    label: "Distribution",
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

interface SimpleAirQualityChartsCardProps {
  className?: string
  onRefresh?: () => void
}

export function SimpleAirQualityChartsCard({ className = "", onRefresh }: SimpleAirQualityChartsCardProps) {
  const [timeRange, setTimeRange] = useState("daily")

  const getTimeData = () => {
    return timeRange === "daily" ? airQualityData.daily : airQualityData.weekly
  }

  const getTimeKey = () => {
    return timeRange === "daily" ? "time" : "day"
  }

  const handleRefresh = () => {
    if (onRefresh) {
      onRefresh()
    }
  }

  return (
    <Card className={`${className}`}>
      <CardHeader className="flex flex-row items-center justify-between pb-2">
        <CardTitle className="flex gap-2 text-lg font-bold"><CloudRainIcon className="text-yellow-600"/> Air Quality Metrics</CardTitle>
        <Button variant="ghost" size="icon" onClick={handleRefresh} className="h-8 w-8">
          <RefreshCcw className="h-4 w-4" />
          <span className="sr-only">Refresh data</span>
        </Button>
      </CardHeader>
      <CardContent className="p-0">
        <Tabs defaultValue="line" className="w-full">
          <div className="px-4">
            <TabsList className="grid w-full grid-cols-3">
              <TabsTrigger value="line">Trends</TabsTrigger>
              <TabsTrigger value="bar">Weekly</TabsTrigger>
              <TabsTrigger value="pie">Distribution</TabsTrigger>
            </TabsList>
          </div>

          <TabsContent value="line" className="mt-0 pt-2">
            <div className="px-4 pb-2">
              <Tabs defaultValue="daily" onValueChange={setTimeRange}>
                <TabsList className="grid w-full grid-cols-2">
                  <TabsTrigger value="daily">Daily</TabsTrigger>
                  <TabsTrigger value="weekly">Weekly</TabsTrigger>
                </TabsList>
              </Tabs>
            </div>
            <div className="px-4 pt-4">
              <ChartContainer config={lineChartConfig}>
                <LineChart accessibilityLayer data={getTimeData()} margin={{ top: 5, right: 5, left: 5, bottom: 5 }}>
                  <CartesianGrid strokeDasharray="3 3" vertical={false} opacity={0.3} />
                  <XAxis dataKey={getTimeKey()} tickLine={false} axisLine={false} tickMargin={10} />
                  <ChartTooltip content={<ChartTooltipContent />} />
                  <Line
                    type="monotone"
                    dataKey="pm25"
                    stroke="var(--color-pm25)"
                    strokeWidth={2}
                    dot={{ r: 3 }}
                    activeDot={{ r: 5 }}
                  />
                  <Line
                    type="monotone"
                    dataKey="aqi"
                    stroke="var(--color-aqi)"
                    strokeWidth={2}
                    dot={{ r: 3 }}
                    activeDot={{ r: 5 }}
                  />
                </LineChart>
              </ChartContainer>
            </div>
          </TabsContent>

          <TabsContent value="bar" className="mt-0 pt-2">
            <div className="px-4 pt-8">
              <ChartContainer config={barChartConfig}>
                <BarChart
                  accessibilityLayer
                  data={airQualityData.weekly}
                  margin={{ top: 5, right: 10, left: 0, bottom: 0 }}
                >
                  <CartesianGrid strokeDasharray="3 3" vertical={false} opacity={0.3} />
                  <XAxis dataKey="day" tickLine={false} axisLine={false} tickMargin={10} />
                  <ChartTooltip content={<ChartTooltipContent />} />
                  <Bar dataKey="pm25" fill="var(--color-pm25)" radius={[4, 4, 0, 0]} barSize={20} />
                </BarChart>
              </ChartContainer>
            </div>
          </TabsContent>

          <TabsContent value="pie" className="mt-0 pt-2">
            <div className="px-4">
              <ChartContainer config={pieChartConfig}>
                <ResponsiveContainer width="100%" height="100%">
                  <PieChart>
                    <Pie
                      data={airQualityData.pollutantDistribution}
                      cx="50%"
                      cy="50%"
                      labelLine={false}
                      outerRadius={70}
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
        </Tabs>
      </CardContent>
    </Card>
  )
}
