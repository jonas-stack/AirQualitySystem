import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { ScrollArea } from "@/components/ui/scroll-area"
import { DeviceDto, SensorDataDto } from "@/generated-client"
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
  PaginationEllipsis
} from "@/components/ui/pagination"
import { useEffect } from "react"

interface SensorDataTableProps {
  selectedDevice: DeviceDto | null;
  sensorData: SensorDataDto[];
  currentPage: number;
  setCurrentPage: (page: number) => void;
  itemsPerPage: number;
  totalPages: number;
}

export function SensorDataTable({ selectedDevice, sensorData, currentPage, setCurrentPage, itemsPerPage, totalPages}: SensorDataTableProps) {
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

  useEffect(() => {
    console.log(sensorData)
  }, [sensorData])

  if (!selectedDevice) {
    return (
      <Card className="h-[600px] flex items-center justify-center">
        <CardContent>
          <p className="text-muted-foreground">No device selected.</p>
        </CardContent>
      </Card>
    )
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
                sensorData.map((data) => (
                  <TableRow key={data.timestamp}>
                    <TableCell className="font-medium">{data.device_id}</TableCell>
                    <TableCell>{data.temperature}°C</TableCell>
                    <TableCell>{data.humidity}%</TableCell>
                    <TableCell>
                      <Badge variant={getAirQualityVariant(data.air_quality ?? 0)}>{data.air_quality}</Badge>
                    </TableCell>
                    <TableCell>
                      <Badge variant={getPM25Variant(data.pm25 ?? 0)}>{data.pm25}</Badge>
                    </TableCell>
                    <TableCell className="text-muted-foreground">{data.timestamp}</TableCell>
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
        <Pagination>
          <PaginationContent>
            <PaginationItem>
              <PaginationPrevious
                href="#"
                onClick={(e) => {
                  e.preventDefault();
                  setCurrentPage(Math.max(currentPage - 1, 1));
                }}
              />
            </PaginationItem>

            {[...Array(totalPages)].map((_, i) => {
              const page = i + 1;
              return (
                <PaginationItem key={page}>
                  <PaginationLink
                    href="#"
                    isActive={page === currentPage}
                    onClick={(e) => {
                      e.preventDefault();
                      setCurrentPage(page);
                    }}
                  >
                    {page}
                  </PaginationLink>
                </PaginationItem>
              );
            })}

            {totalPages > 5 && currentPage < totalPages - 2 && (
              <PaginationItem>
                <PaginationEllipsis />
              </PaginationItem>
            )}

            <PaginationItem>
              <PaginationNext
                href="#"
                onClick={(e) => {
                  e.preventDefault();
                  setCurrentPage(Math.min(currentPage + 1, totalPages));
                }}
              />
            </PaginationItem>
          </PaginationContent>
        </Pagination>

      </CardContent>
    </Card>
  )
}
