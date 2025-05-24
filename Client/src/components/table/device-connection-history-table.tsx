"use client"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import { ScrollArea } from "@/components/ui/scroll-area"
import type { DeviceConnectionHistoryDto, DeviceDto } from "@/generated-client"
import { Wifi, WifiOff, Zap, AlertTriangle, History } from "lucide-react"
import { formatDuration, unixToDateString } from "@/lib/time-formatter"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../ui/select"

interface DeviceConnectionHistoryTableProps {
    selectedDevice: DeviceDto | null
    deviceConnectionHistory: DeviceConnectionHistoryDto[];
    currentPage: number
    setCurrentPage: (page: number) => void
    itemsPerPage: number
    setItemsPerPage?: (itemsPerPage: number) => void
    totalPages: number
}

export function DeviceConnectionHistoryTable({ selectedDevice, deviceConnectionHistory, currentPage, setCurrentPage, setItemsPerPage, itemsPerPage, totalPages }: DeviceConnectionHistoryTableProps) {
  if (!selectedDevice) {
    return (
      <Card className="h-[580px]">
        <CardContent className="flex items-center justify-center h-full">
          <div className="text-center space-y-4">
            <div className="w-20 h-20 bg-gradient-to-br from-muted to-muted/50 rounded-full flex items-center justify-center mx-auto">
              <History className="w-10 h-10 text-muted-foreground" />
            </div>
            <div>
              <p className="text-lg font-semibold">No device selected</p>
              <p className="text-sm text-muted-foreground">Please select a device to view connection history</p>
            </div>
          </div>
        </CardContent>
      </Card>
    )
  }

  return (
    <Card className="h-[580px] flex flex-col">
      <CardHeader className="flex-shrink-0">
        <div className="flex items-center justify-between">
          <div className="space-y-2">
            <CardTitle className="flex items-center gap-2 text-lg font-bold">
              <div className="p-2 bg-gradient-to-br from-blue-500 to-blue-600 rounded-lg">
                <History className="w-5 h-5 text-white" />
              </div>
              Connection History
            </CardTitle>
            <CardDescription>
              Recent connection events for{" "}
              <span className="font-medium text-foreground">{selectedDevice.DeviceName}</span>
            </CardDescription>
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
              <TableHeader className="bg-muted/30 backdrop-blur-sm">
                <TableRow className="border-b border-border/50">
                  <TableHead className="font-semibold">Status</TableHead>
                  <TableHead className="font-semibold">Timestamp</TableHead>
                  <TableHead className="font-semibold">Duration</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {deviceConnectionHistory.length > 0 ? (
                  deviceConnectionHistory.map((event, index) => (
                    <TableRow key={event.id} className="hover:bg-muted/20 transition-colors border-b border-border/30">
                      <TableCell>
                        <div className="flex items-center gap-3">
                          <div
                            className={`p-2 rounded-full ${
                              event.isConnected
                                ? "bg-green-100 dark:bg-green-900/30"
                                : "bg-red-100 dark:bg-red-900/30"
                            }`}
                          >
                            {event.isConnected ? (
                              <Wifi className="w-4 h-4 text-green-600" />
                            ) : (
                              <WifiOff className="w-4 h-4 text-red-600" />
                            )}
                          </div>
                          <Badge
                            variant={event.isConnected ? "default" : "destructive"}
                            className="font-medium"
                          >
                            {event.isConnected ? "Connected" : "Disconnected"}
                          </Badge>
                        </div>
                      </TableCell>
                      <TableCell className="text-muted-foreground font-mono text-sm">{event.lastSeen ? unixToDateString(event.lastSeen) : "-"}</TableCell>
                      <TableCell>
                        {event.duration ? (
                          <Badge variant="outline" className="bg-background/50">
                            {event.lastSeen ? formatDuration(event.duration) : "-"}
                          </Badge>
                        ) : (
                          <span className="text-muted-foreground">-</span>
                        )}
                      </TableCell>
                    </TableRow>
                  ))
                ) : (
                  <TableRow>
                    <TableCell colSpan={5} className="text-center py-12">
                      <div className="flex flex-col items-center gap-4">
                        <div className="w-16 h-16 bg-gradient-to-br from-muted to-muted/50 rounded-full flex items-center justify-center">
                          <History className="w-8 h-8 text-muted-foreground" />
                        </div>
                        <div>
                          <h3 className="font-semibold text-lg mb-1">No Connection History</h3>
                          <p className="text-muted-foreground">No connection events found for this device</p>
                        </div>
                      </div>
                    </TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </ScrollArea>
        </div>
      </CardContent>
    </Card>
  )
}
