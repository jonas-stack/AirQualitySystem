import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Wifi, WifiOff, ZapIcon } from "lucide-react"
import { DeviceDto } from "@/generated-client"
import { formatTicksToUTC } from "@/lib/time-formatter"
import Flex from "../utils/Flex"
import { Spinner } from "../ui/spinner"

interface DeviceSelectorProps {
  devices: DeviceDto[]
  selectedDevice: DeviceDto | null
  isDevicesLoading: boolean,
  onDeviceSelect: (device: DeviceDto) => void
}

export function DeviceSelector({ devices, selectedDevice, onDeviceSelect, isDevicesLoading }: DeviceSelectorProps) {

  return (
    <Card className="h-[600px]">
      <CardHeader>
        <CardTitle className="flex items-center gap-2 text-lg font-bold">
          <ZapIcon className="text-purple-600" /> Device Selection
        </CardTitle>
        <CardDescription>Select a device to view its sensor data</CardDescription>
      </CardHeader>
      <CardContent className="p-0">
        {isDevicesLoading ? (
            <Flex alignItems="center" justifyContent="center" className="w-full h-100">
              <Flex direction="column" alignItems="center" gap="2">
                <Spinner size="lg" className="text-primary" />
                <p className="text-sm text-muted-foreground">Loading devices...</p>
              </Flex>
            </Flex>
        ) : (
        <ScrollArea className="h-[500px]">
          <div className="space-y-2 p-4">
            {devices.map((device) => (
              <Button
                key={device.device_id}
                variant={selectedDevice?.device_id === device.device_id ? "secondary" : "ghost"}
                className="w-full justify-start h-auto p-4"
                onClick={() => onDeviceSelect(device)}
              >
                <div className="flex items-center justify-between w-full">
                  <div className="text-left">
                    <div className="font-medium">{device.DeviceName}</div>
                    <div className="text-xs text-muted-foreground">Last seen: {device.LastSeen ? formatTicksToUTC(device.LastSeen) : "Unknown"}</div>
                  </div>
                  <div className="flex items-center gap-2">
                    {device.IsConnected ? (
                      <Badge variant="default" className="bg-green-500">
                        <Wifi className="h-3 w-3 mr-1" />
                        Online
                      </Badge>
                    ) : (
                      <Badge variant="destructive">
                        <WifiOff className="h-3 w-3 mr-1" />
                        Offline
                      </Badge>
                    )}
                  </div>
                </div>
              </Button>
            ))}
          </div>
        </ScrollArea>
        )}
      </CardContent>
    </Card>
  )
}
