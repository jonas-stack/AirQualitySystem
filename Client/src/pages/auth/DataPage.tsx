import { AirQualityChartsCard } from "@/components/cards/air-quality-charts-card";

export default function DataPage() {
    return (
        <div className="w-full mx-auto py-2">
            <AirQualityChartsCard 
                className="w-full" 
                onRefresh={() => console.log("Refreshing chart data...")} 
            />
        </div>
    )
}