"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Button } from "@/components/ui/button"
import type { DeviceDto, SensorDataDto } from "@/generated-client"
import { useEffect } from "react"
import { formatTicksToUTC } from "@/lib/time-formatter"
import {
  ChevronLeft,
  ChevronRight,
  ChevronsLeft,
  ChevronsRight,
  Thermometer,
  Droplets,
  Wind,
  Gauge,
} from "lucide-react"

interface SensorDataTableProps {
  selectedDevice: DeviceDto | null
  sensorData: SensorDataDto[]
  currentPage: number
  setCurrentPage: (page: number) => void
  itemsPerPage: number
  setItemsPerPage?: (itemsPerPage: number) => void
  totalPages: number
  totalItems?: number
}

export function SensorDataTable({
  selectedDevice,
  sensorData,
  currentPage,
  setCurrentPage,
  itemsPerPage,
  setItemsPerPage,
  totalPages,
  totalItems,
}: SensorDataTableProps) {
  const getAirQualityVariant = (airQuality: number) => {
    if (airQuality >= 80) return "default"
    if (airQuality >= 60) return "secondary"
    return "destructive"
  }

  const getAirQualityLabel = (airQuality: number) => {
    if (airQuality >= 80) return "Good"
    if (airQuality >= 60) return "Moderate"
    return "Poor"
  }

  const getPM25Variant = (pm25: number) => {
    if (pm25 <= 15) return "default"
    if (pm25 <= 25) return "secondary"
    return "destructive"
  }

  const getPM25Label = (pm25: number) => {
    if (pm25 <= 15) return "Good"
    if (pm25 <= 25) return "Moderate"
    return "Unhealthy"
  }

  const getTemperatureColor = (temp: number) => {
    if (temp < 10) return "text-blue-600"
    if (temp < 25) return "text-green-600"
    if (temp < 35) return "text-orange-600"
    return "text-red-600"
  }

  const getHumidityColor = (humidity: number) => {
    if (humidity < 30) return "text-orange-600"
    if (humidity < 70) return "text-green-600"
    return "text-blue-600"
  }

  // Generate pagination range
  const generatePaginationRange = () => {
    const delta = 2
    const range = []
    const rangeWithDots = []

    for (let i = Math.max(2, currentPage - delta); i <= Math.min(totalPages - 1, currentPage + delta); i++) {
      range.push(i)
    }

    if (currentPage - delta > 2) {
      rangeWithDots.push(1, "...")
    } else {
      rangeWithDots.push(1)
    }

    rangeWithDots.push(...range)

    if (currentPage + delta < totalPages - 1) {
      rangeWithDots.push("...", totalPages)
    } else if (totalPages > 1) {
      rangeWithDots.push(totalPages)
    }

    return rangeWithDots
  }

  useEffect(() => {
    console.log(sensorData)
  }, [sensorData])

  if (!selectedDevice) {
    return (
      <Card className="h-[700px] flex items-center justify-center">
        <CardContent className="text-center">
          <div className="mx-auto w-24 h-24 bg-muted rounded-full flex items-center justify-center mb-4">
            <Gauge className="w-12 h-12 text-muted-foreground" />
          </div>
          <h3 className="text-lg font-semibold mb-2">No Device Selected</h3>
          <p className="text-muted-foreground">Please select a device to view sensor data</p>
        </CardContent>
      </Card>
    )
  }

  const startItem = (currentPage - 1) * itemsPerPage + 1
  const endItem = Math.min(currentPage * itemsPerPage, totalItems || sensorData.length)

  return (
    <Card className="h-[700px] flex flex-col">
      <CardHeader className="pb-4">
        <div className="flex items-center justify-between">
          <div>
            <CardTitle className="flex items-center gap-2">
              <Gauge className="w-5 h-5" />
              Sensor Data - {selectedDevice.DeviceName}
            </CardTitle>
            <CardDescription>Recent measurements from the selected device</CardDescription>
          </div>
          {setItemsPerPage && (
            <div className="flex items-center gap-2">
              <span className="text-sm text-muted-foreground">Rows per page:</span>
              <Select
                value={itemsPerPage.toString()}
                onValueChange={(value) => setItemsPerPage(Number.parseInt(value))}
              >
                <SelectTrigger className="w-20">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="10">10</SelectItem>
                  <SelectItem value="25">25</SelectItem>
                  <SelectItem value="50">50</SelectItem>
                  <SelectItem value="100">100</SelectItem>
                </SelectContent>
              </Select>
            </div>
          )}
        </div>
      </CardHeader>

      <CardContent className="flex-1 flex flex-col p-0">
        <div className="flex-1 border-b">
          <ScrollArea className="h-[480px]">
            <Table>
              <TableHeader className="sticky top-0 bg-background z-10">
                <TableRow>
                  <TableHead className="w-[120px]">
                    <div className="flex items-center gap-2">
                      <Thermometer className="w-4 h-4" />
                      Temperature
                    </div>
                  </TableHead>
                  <TableHead className="w-[120px]">
                    <div className="flex items-center gap-2">
                      <Droplets className="w-4 h-4" />
                      Humidity
                    </div>
                  </TableHead>
                  <TableHead className="w-[140px]">
                    <div className="flex items-center gap-2">
                      <Wind className="w-4 h-4" />
                      Air Quality
                    </div>
                  </TableHead>
                  <TableHead className="w-[140px]">PM2.5</TableHead>
                  <TableHead>Timestamp</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {sensorData.length > 0 ? (
                  sensorData.map((data, index) => (
                    <TableRow key={data.timestamp || index} className="hover:bg-muted/50">
                      <TableCell>
                        <span className={`font-medium ${getTemperatureColor(data.temperature || 0)}`}>
                          {data.temperature}°C
                        </span>
                      </TableCell>
                      <TableCell>
                        <span className={`font-medium ${getHumidityColor(data.humidity || 0)}`}>{data.humidity}%</span>
                      </TableCell>
                      <TableCell>
                        <Badge variant={getAirQualityVariant(data.air_quality ?? 0)} className="font-medium">
                          {data.air_quality} - {getAirQualityLabel(data.air_quality ?? 0)}
                        </Badge>
                      </TableCell>
                      <TableCell>
                        <Badge variant={getPM25Variant(data.pm25 ?? 0)} className="font-medium">
                          {data.pm25} μg/m³ - {getPM25Label(data.pm25 ?? 0)}
                        </Badge>
                      </TableCell>
                      <TableCell className="text-muted-foreground font-mono text-sm">
                        {data.timestamp ? formatTicksToUTC(data.timestamp) : "Unknown"}
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={5} className="text-center py-12">
                      <div className="flex flex-col items-center gap-3">
                        <div className="w-16 h-16 bg-muted rounded-full flex items-center justify-center">
                          <Gauge className="w-8 h-8 text-muted-foreground" />
                        </div>
                        <div>
                          <h3 className="font-semibold mb-1">No Data Available</h3>
                          <p className="text-muted-foreground text-sm">No sensor data available for this device</p>
                        </div>
                      </div>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </ScrollArea>
        </div>

        {/* Enhanced Pagination */}
        {totalPages > 1 && (
          <div className="flex items-center justify-between px-6 py-4 bg-muted/30">
            <div className="text-sm text-muted-foreground">
              Showing {startItem} to {endItem} of {totalItems || sensorData.length} entries
            </div>

            <div className="flex items-center gap-2">
              <Button
                variant="outline"
                size="sm"
                onClick={() => setCurrentPage(1)}
                disabled={currentPage === 1}
                className="h-8 w-8 p-0"
              >
                <ChevronsLeft className="h-4 w-4" />
              </Button>

              <Button
                variant="outline"
                size="sm"
                onClick={() => setCurrentPage(Math.max(currentPage - 1, 1))}
                disabled={currentPage === 1}
                className="h-8 w-8 p-0"
              >
                <ChevronLeft className="h-4 w-4" />
              </Button>

              <div className="flex items-center gap-1">
                {generatePaginationRange().map((page, index) => (
                  <div key={index}>
                    {page === "..." ? (
                      <span className="px-2 py-1 text-muted-foreground">...</span>
                    ) : (
                      <Button
                        variant={page === currentPage ? "default" : "outline"}
                        size="sm"
                        onClick={() => setCurrentPage(page as number)}
                        className="h-8 w-8 p-0"
                      >
                        {page}
                      </Button>
                    )}
                  </div>
                ))}
              </div>

              <Button
                variant="outline"
                size="sm"
                onClick={() => setCurrentPage(Math.min(currentPage + 1, totalPages))}
                disabled={currentPage === totalPages}
                className="h-8 w-8 p-0"
              >
                <ChevronRight className="h-4 w-4" />
              </Button>

              <Button
                variant="outline"
                size="sm"
                onClick={() => setCurrentPage(totalPages)}
                disabled={currentPage === totalPages}
                className="h-8 w-8 p-0"
              >
                <ChevronsRight className="h-4 w-4" />
              </Button>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  )
}
