import { Area, AreaChart } from "recharts";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { ChartConfig, ChartContainer } from "@/components/ui/chart";
import { useEffect, useState } from "react";
import { useWsClient } from "ws-request-hook";
import { GraphModel_1, StringConstants } from "@/generated-client";

export default function AirQualityTotalMeasurementCard() {
    const {onMessage, readyState} = useWsClient()

    const [chartData, setChartData] = useState([
    { month: "January", amount: 80 },
    { month: "February", amount: 200 },
    { month: "March", amount: 120 },
    { month: "April", amount: 190 },
    { month: "May", amount: 130 },
    { month: "June", amount: 140 },
    ]);

    const chartConfig = {
        amount: {
        label: "Amount",
        color: "var(--chart-2)",
        },
  } satisfies ChartConfig

    useEffect(() => {
        if (readyState != 1)
            return;

        const reactToMessageSetup = onMessage<GraphModel_1>
        ("TotalMeasurementsOfDevice", (dto) => {
            setChartData(prevData => {
            const newEntry = {
                month: dto.eventType ?? `Month ${prevData.length + 1}`,
                amount: dto.amount ?? 0, // amount is now guaranteed to be a number
            };

            return [...prevData, newEntry];
            });
        })
        return () => reactToMessageSetup();
    }, [readyState]);


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