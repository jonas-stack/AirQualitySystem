import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import type { DeviceDto, SensorDataDto } from "@/generated-client"
import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
  PaginationEllipsis,
} from "@/components/ui/pagination"
import { useEffect } from "react"
import { formatTicksToUTC } from "@/lib/time-formatter"
import { CpuIcon } from "lucide-react"

interface SensorDataTableProps {
  selectedDevice: DeviceDto | null
  sensorData: SensorDataDto[]
  currentPage: number
  setCurrentPage: (page: number) => void
  itemsPerPage: number
  setItemsPerPage?: (itemsPerPage: number) => void
  totalPages: number
}

export function SensorDataTable({
  selectedDevice,
  sensorData,
  currentPage,
  setCurrentPage,
  itemsPerPage,
  setItemsPerPage,
  totalPages,
}: SensorDataTableProps) {
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

  const generatePaginationItems = () => {
    const items = []
    const showEllipsis = totalPages > 7

    if (!showEllipsis) {
      for (let i = 1; i <= totalPages; i++) {
        items.push(i)
      }
    } else {
      if (currentPage <= 4) {
        items.push(1, 2, 3, 4, 5, "ellipsis", totalPages)
      } else if (currentPage >= totalPages - 3) {
        items.push(1, "ellipsis", totalPages - 4, totalPages - 3, totalPages - 2, totalPages - 1, totalPages)
      } else {
        items.push(1, "ellipsis", currentPage - 1, currentPage, currentPage + 1, "ellipsis", totalPages)
      }
    }

    return items
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
    <Card className="h-[580px] flex flex-col">
      <CardHeader className="flex-shrink-0">
        <div className="flex items-center justify-between">
          <div>
            <CardTitle className="flex items-center justify-center gap-2 text-lg font-bold">
              <CpuIcon className="text-green-600" /> Sensor Data - {selectedDevice.DeviceName}
            </CardTitle>
            <CardDescription>Recent measurements from the selected device</CardDescription>
          </div>
          {setItemsPerPage && (
            <div className="flex items-center space-x-2">
              <span className="text-sm font-medium">Rows per page:</span>
              <Select
                value={itemsPerPage.toString()}
                onValueChange={(value) => setItemsPerPage(Number.parseInt(value))}
              >
                <SelectTrigger className="w-[100px]">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent side="bottom" align="end">
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

      <CardContent className="p-0 flex-1 flex flex-col">
        <div className="flex-1 overflow-hidden">
          <ScrollArea className="h-100">
            <Table>
              <TableHeader>
                <TableRow>
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
                      <TableCell>{data.temperature}°C</TableCell>
                      <TableCell>{data.humidity}%</TableCell>
                      <TableCell>
                        <Badge variant={getAirQualityVariant(data.air_quality ?? 0)}>{data.air_quality}</Badge>
                      </TableCell>
                      <TableCell>
                        <Badge variant={getPM25Variant(data.pm25 ?? 0)}>{data.pm25}</Badge>
                      </TableCell>
                      <TableCell className="text-muted-foreground">
                        {data.timestamp ? formatTicksToUTC(data.timestamp) : "Unknown"}
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={5} className="text-center text-muted-foreground py-8">
                      No sensor data available for this device
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </ScrollArea>
        </div>
        {totalPages > 1 && (
          <div className="border-t p-4 flex justify-center">
            <Pagination>
              <PaginationContent>
                <PaginationItem>
                  <PaginationPrevious
                    href="#"
                    onClick={(e) => {
                      e.preventDefault()
                      if (currentPage > 1) setCurrentPage(currentPage - 1)
                    }}
                    className={currentPage === 1 ? "pointer-events-none opacity-50" : ""}
                  />
                </PaginationItem>

                {generatePaginationItems().map((item, index) => (
                  <PaginationItem key={index}>
                    {item === "ellipsis" ? (
                      <PaginationEllipsis />
                    ) : (
                      <PaginationLink
                        href="#"
                        onClick={(e) => {
                          e.preventDefault()
                          setCurrentPage(item as number)
                        }}
                        isActive={currentPage === item}
                      >
                        {item}
                      </PaginationLink>
                    )}
                  </PaginationItem>
                ))}

                <PaginationItem>
                  <PaginationNext
                    href="#"
                    onClick={(e) => {
                      e.preventDefault()
                      if (currentPage < totalPages) setCurrentPage(currentPage + 1)
                    }}
                    className={currentPage === totalPages ? "pointer-events-none opacity-50" : ""}
                  />
                </PaginationItem>
              </PaginationContent>
            </Pagination>
          </div>
        )}
      </CardContent>
    </Card>
  )
}
