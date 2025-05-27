import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { DeviceStatsDto } from "@/generated-client"
import { DashboardStats } from "@/types/dashboard"
import { Activity, CableIcon, CalendarRangeIcon, FileWarning, InfoIcon, MessageCircleWarningIcon, Thermometer, WifiOff, XCircleIcon, XIcon } from "lucide-react"

interface StatisticsCardsProps {
  stats: DeviceStatsDto
}

export function StatisticsCards({ stats }: StatisticsCardsProps) {
  return (
    <div className="grid gap-4 lg:grid-cols-3">
      <Card className="overflow-hidden relative">
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="flex items-center gap-2 text-lg font-bold">
            <div className="p-2 bg-gradient-to-br from-green-500 to-green-600 rounded-lg">
              <CalendarRangeIcon className="text-white w-5 h-5" /> 
            </div>
            All Time Measurements
          </CardTitle>
          <Activity className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.allTimeMeasurements?.toLocaleString()}</div>
        </CardContent>
      </Card>

      <Card className="overflow-hidden relative">
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="flex items-center gap-2 text-lg font-bold">
            <div className="p-2 bg-gradient-to-br from-yellow-500 to-yellow-600 rounded-lg">
              <CableIcon className="text-white w-5 h-5" />
            </div>
            Connected Devices
          </CardTitle>
          <Thermometer className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.connectedDevices}</div>
        </CardContent>
      </Card>

      <Card className="overflow-hidden relative">
        <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
          <CardTitle className="flex items-center gap-2 text-lg font-bold">
            <div className="p-2 bg-gradient-to-br from-red-500 to-red-600 rounded-lg">
              <InfoIcon className="text-white w-5 h-5" /> 
            </div>            
            Disconnections (24h)
          </CardTitle>
          <WifiOff className="h-4 w-4 text-muted-foreground" />
        </CardHeader>
        <CardContent>
          <div className="text-2xl font-bold">{stats.disconnectionsLast24Hours}</div>
        </CardContent>
      </Card>
    </div>
  )
}
