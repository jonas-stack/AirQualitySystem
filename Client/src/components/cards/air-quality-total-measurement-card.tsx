import { Area, AreaChart } from "recharts";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ChartConfig, ChartContainer } from "@/components/ui/chart";

export default function AirQualityTotalMeasurementCard() {
const chartData = [
    { month: "January", amount: 80 },
    { month: "February", amount: 200 },
    { month: "March", amount: 120 },
    { month: "April", amount: 190 },
    { month: "May", amount: 130 },
    { month: "June", amount: 140 },
    { month: "test", amount: 2}
]

const chartConfig = {
    amount: {
      label: "Amount",
      color: "var(--chart-2)",
    },
  } satisfies ChartConfig

    return (
        <Card className="p-0 pt-6 overflow-hidden h-60">
        <div className="flex flex-col space-y-1.5 pl-2">
        <CardHeader>
            <CardTitle className="text-lg font-bold">
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
    )
}