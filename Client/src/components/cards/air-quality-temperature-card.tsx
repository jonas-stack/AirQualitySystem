import { Area, AreaChart } from "recharts";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ChartConfig, ChartContainer, ChartTooltip } from "@/components/ui/chart";
import { useEffect, useState } from "react";
import { useWsClient } from "ws-request-hook";
import { GraphModel_1, WebsocketEvents } from "@/generated-client";
import { Thermometer } from "lucide-react";

export default function AirQualityTemperatureCard() {
    const {onMessage, readyState} = useWsClient()

    const [chartData, setChartData] = useState([
        { time: "00:00", amount: 22 },
        { time: "04:00", amount: 21 },
        { time: "08:00", amount: 23 },
        { time: "12:00", amount: 25 },
        { time: "16:00", amount: 24 },
        { time: "20:00", amount: 22 },
        { time: "24:00", amount: 21 },
    ]);

    const chartConfig = {
        amount: {
        label: "Amount",
        color: "var(--chart-2)",
        },
    } satisfies ChartConfig

    useEffect(() => {
        if (readyState !== 1)
            return;

        const reactToMessageSetup = onMessage<GraphModel_1>(
            WebsocketEvents.GraphTotalMeasurement,
            (dto) => {
                setChartData(prevData => {
                    const newEntry = {
                        time: dto.identifier,
                        amount: dto.amount ?? 0,
                    };

                    const updatedData = [...prevData, newEntry];

                    // max vis 6 af gangen og ikke flere
                    return updatedData.length > 6 ? updatedData.slice(-6) : updatedData;
                });
            }
        );

        return () => reactToMessageSetup();
    }, [readyState]);

    const newest = chartData.at(-1);

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
              <span className="text-2xl font-bold text-primary">{newest?.amount ?? "--"}°</span>
            </div>
          </div>
        </CardHeader>
        <CardContent className="p-0 h-full mt-10 flex-1 flex flex-col justify-end">
          <div className="relative h-full w-full">
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
                      <stop offset="5%" stopColor="var(--color-amount)" stopOpacity={0.8} />
                      <stop offset="95%" stopColor="var(--color-amount)" stopOpacity={0.1} />
                    </linearGradient>
                  </defs>
                  <Area
                    dataKey="amount"
                    type="natural"
                    fill="url(#fillAmount)"
                    fillOpacity={0.4}
                    stroke="var(--color-amount)"
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