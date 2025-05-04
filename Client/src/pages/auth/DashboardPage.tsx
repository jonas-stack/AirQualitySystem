import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { ChartConfig, ChartContainer } from "@/components/ui/chart";
import { useState } from "react";
import { Area, AreaChart } from "recharts";
import { ConnectionStatusCard } from "@/components/cards/connection-status-card";
import { CardArray } from "@/types/card";
import { AirQualityInsightsCard } from "@/components/cards/air-quality-insights-card";
import { AIChatCard } from "@/components/cards/ai-chat-card";
import { AirQualityChartsCard } from "@/components/cards/air-quality-charts-card";
import { SimpleAirQualityChartsCard } from "@/components/cards/simple-air-quality-charts-card";

const chartData = [
    { month: "January", amount: 80 },
    { month: "February", amount: 200 },
    { month: "March", amount: 120 },
    { month: "April", amount: 190 },
    { month: "May", amount: 130 },
    { month: "June", amount: 140 },
]

const chartConfig = {
    amount: {
      label: "Amount",
      color: "var(--chart-2)",
    },
  } satisfies ChartConfig

  const airQualityData: CardArray = [
    {
      description: "Today at 23:55",
      title: "PM2.5 Levels Elevated",
      src: "/placeholder.svg?height=200&width=200",
      ctaText: "Details",
      ctaLink: "#",
      content: () => (
        <div>
          <p className="mb-4">
            PM2.5 levels in your living room have increased by 35% in the last 24 hours. Current reading: 18 μg/m³
            (Moderate).
          </p>
          <p className="mb-4">
            <strong>Possible causes:</strong> Recent cooking activities, candle usage, or outdoor pollution entering
            through open windows.
          </p>
          <p>
            <strong>Recommendation:</strong> Consider running your air purifier at a higher setting and keep windows
            closed during peak traffic hours.
          </p>
        </div>
      ),
    },
    {
      description: "Yesterday",
      title: "Good AQI Today",
      src: "/placeholder.svg?height=200&width=200",
      ctaText: "Details",
      ctaLink: "#",
      content:
        "The Air Quality Index (AQI) in your area is currently 42 (Good). This is an excellent time for outdoor activities. Pollen counts are low, and pollution levels are within healthy ranges for all population groups.",
    },
    {
      description: "3 days ago",
      title: "Humidity Below Optimal Range",
      src: "/placeholder.svg?height=200&width=200",
      ctaText: "Details",
      ctaLink: "#",
      content:
        "Indoor humidity in your bedroom is currently at 28%, which is below the recommended range of 30-50%. Low humidity can cause dry skin, irritated eyes, and respiratory discomfort. Consider using a humidifier to increase moisture levels.",
    },
    {
      description: "3 days ago",
      title: "CO2 Concentration Normal",
      src: "/placeholder.svg?height=200&width=200",
      ctaText: "Details",
      ctaLink: "#",
      content:
        "Carbon dioxide levels in your home office are at 650 ppm, which is within the normal range. Good ventilation is maintaining healthy air quality. CO2 levels below 1000 ppm are considered optimal for cognitive function and productivity.",
    },
  ]


export default function DashboardPage() {
    const [isConnected, setIsConnected] = useState(true)

    const handleSendMessage = async (message: string): Promise<string> => {
        // Simulate API delay
        await new Promise((resolve) => setTimeout(resolve, 1500))
    
        // Simple response logic based on keywords
        if (message.toLowerCase().includes("air quality")) {
          return "Based on today's readings, your indoor air quality is good overall, with PM2.5 levels at 8 μg/m³. However, I've noticed slightly elevated VOC levels in the kitchen area."
        } else if (message.toLowerCase().includes("purifier")) {
          return "I recommend running your air purifier on medium setting today. Based on the forecast, pollen counts will increase in the afternoon."
        } else if (message.toLowerCase().includes("humidity")) {
          return "The current humidity in your home is 42%, which is within the optimal range of 30-50%. No action needed at this time."
        } else {
          return "I'm your air quality assistant. You can ask me about current air quality, recommendations for your purifier settings, or humidity levels in your home."
        }
      }

    return (
        <div className="grid gap-4 lg:grid-cols-12 py-2">
          <div className="lg:col-span-5">
            <Card className="overflow-hidden relative lg:col-span-12 xl:col-span-6 h-60">
              <div className="grid items-center lg:grid-cols-3">
                <div className="space-y-4 lg:col-span-2">
                <div className="flex flex-col space-y-1.5 lg:col-span-2">
                <CardHeader>
                  <CardTitle>
                    <span className="text-2xl">
                        Hi, user
                      </span>
                    <span className="text-3xl ml-2">👋</span>
                  </CardTitle>
                  <CardDescription className="text-1xl mt-2 text-current truncate">
                    Make sure to check the suggestions from the AI!
                  </CardDescription>
                </CardHeader>
                </div>
                <CardContent className="flex flex-col">
                  <div className="text-muted-foreground truncate">
                    Click the button below to visit the AI.
                  </div>
                </CardContent>
                <CardFooter>
                  <Button className="cursor-pointer mt-5">
                    Take me there!
                  </Button>
                </CardFooter>
                </div>
                <figure className="hidden lg:col-span-1 lg:block">
                  <img className="block dark:hidden" alt="..." src="/images/extra/academy-dashboard-light.svg"/>
                  <img className="hidden dark:block" alt="..." src="/images/extra/academy-dashboard-dark.svg"/>
                </figure>
                <img className="pointer-events-none absolute inset-0" alt="..." src="/images/extra/star-shape.png"></img>
                </div>
            </Card>
          </div>

          <div className="lg:col-span-4">
              <Card className="p-0 pt-6 overflow-hidden h-60">
                <div className="flex flex-col space-y-1.5 pl-2">
                <CardHeader>
                  <CardTitle>
                    Total measurements
                  </CardTitle>
                  <CardDescription className="truncate">
                    The total measurements from this year
                  </CardDescription>
                </CardHeader>
                </div>
                <CardContent>
                  <div className="pl-2 pt-0 flex">
                    <div className="text-2xl font-bold">203</div>
                  </div>
                </CardContent>
                <ChartContainer config={chartConfig} className="h-23 w-full">
                      <AreaChart
                        accessibilityLayer
                        data={chartData}
                        margin={{
                          left: 0,
                          right: 0,
                        }}>
                        <defs>
                          <linearGradient id="fillAmount" x1="0" y1="0" x2="0" y2="1">
                            <stop
                              offset="5%"
                              stopColor="var(--color-amount)"
                              stopOpacity={0.8}
                            />
                            <stop
                              offset="95%"
                              stopColor="var(--color-amount)"
                              stopOpacity={0.1}
                            />
                          </linearGradient>
                        </defs>
                        <Area
                          dataKey="amount"
                          type="natural"
                          fill="url(#fillAmount)"
                          fillOpacity={0.4}
                          stroke="var(--color-amount)"
                          stackId="a"
                        />
                      </AreaChart>
                    </ChartContainer>
              </Card>
            </div>

            <div className="lg:col-span-3">
                <ConnectionStatusCard className="h-60" isConnected={isConnected} onConnectionChange={setIsConnected} />
            </div>

            <div className="lg:col-span-4">
                <AirQualityInsightsCard
                cards={airQualityData}
                onRefresh={() => console.log("Refreshing air quality data...")}
                lastUpdated={new Date()}
                />
            </div>

            <div className="lg:col-span-4">
                <AIChatCard className="h-[500px] lg:h-[496px]" title="Air Quality Assistant" onSendMessage={handleSendMessage} />
            </div>

            <div className="lg:col-span-4">
                <SimpleAirQualityChartsCard className="h-[496px]" onRefresh={() => console.log("Refreshing chart data...")} />
            </div>
        </div>
    )
}