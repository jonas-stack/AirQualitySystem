import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { ScrollArea } from "@/components/ui/scroll-area"
import { DeviceDto } from "@/generated-client"

interface SensorDataTableProps {
  selectedDevice: DeviceDto
  sensorData: any
  // skal have lavet en sensordata dto..
  //sensorData: SensorData[]
}

export function SensorDataTable({ selectedDevice, sensorData }: SensorDataTableProps) {
  const getAirQualityVariant = (airQuality: number) => {
    if (airQuality >= 80) return "default"
    if (airQuality >= 60) return "secondary"
    return "destructive"
  }

  const getPM25Variant = (pm25: number) => {
    if (pm25 <= 15) return "default"
    if (pm25 <= 25) return "secondary"
    return "destructive"
  }

  return (
    <Card className="h-[600px]">
      <CardHeader>
        <CardTitle>Sensor Data - {selectedDevice.DeviceName}</CardTitle>
        <CardDescription>Recent measurements from the selected device</CardDescription>
      </CardHeader>
      <CardContent className="p-0">
        <ScrollArea className="h-[500px]">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>ID</TableHead>
                <TableHead>Temperature (°C)</TableHead>
                <TableHead>Humidity (%)</TableHead>
                <TableHead>Air Quality</TableHead>
                <TableHead>PM2.5 (μg/m³)</TableHead>
                <TableHead>Timestamp</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {sensorData.length > 0 ? (
                sensorData.map((reading) => (
                  <TableRow key={reading.id}>
                    <TableCell className="font-medium">{reading.id}</TableCell>
                    <TableCell>{reading.temperature}°C</TableCell>
                    <TableCell>{reading.humidity}%</TableCell>
                    <TableCell>
                      <Badge variant={getAirQualityVariant(reading.airQuality)}>{reading.airQuality}</Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant={getPM25Variant(reading.pm25)}>{reading.pm25}</Badge>
                    </TableCell>
                    <TableCell className="text-muted-foreground">{reading.timestamp}</TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={6} className="text-center text-muted-foreground py-8">
                    No sensor data available for this device
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </ScrollArea>
      </CardContent>
    </Card>
  )
}
