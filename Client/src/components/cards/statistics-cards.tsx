import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { DashboardStats } from "@/types/dashboard"
import { Activity, Thermometer, WifiOff } from "lucide-react"

interface StatisticsCardsProps {
  stats: DashboardStats
  connectedDevicesCount: number
}

export function StatisticsCards({ stats, connectedDevicesCount }: StatisticsCardsProps) {
  return (
    <div className="grid gap-4 lg:grid-cols-3">
      <Card className="overflow-hidden relative">
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">All Time Measurements</CardTitle>
          <Activity className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.totalMeasurements.toLocaleString()}</div>
          <p className="text-xs text-muted-foreground">+2,847 from last month</p>
        </CardContent>
      </Card>

      <Card className="overflow-hidden relative">
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Connected Devices</CardTitle>
          <Thermometer className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.totalDevices}</div>
          <p className="text-xs text-muted-foreground">{connectedDevicesCount} currently online</p>
        </CardContent>
      </Card>

      <Card className="overflow-hidden relative">
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="text-sm font-medium">Disconnections (24h)</CardTitle>
          <WifiOff className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.disconnectionsLast24h}</div>
          <p className="text-xs text-muted-foreground">-2 from yesterday</p>
        </CardContent>
      </Card>
    </div>
  )
}
