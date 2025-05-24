import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Badge } from "@/components/ui/badge"
import { ScrollArea } from "@/components/ui/scroll-area"
import { AlertTriangle, RefreshCw, Wifi, WifiOff, XIcon, ZapIcon } from "lucide-react"
import { DeviceDto } from "@/generated-client"
import { formatTicksToUTC } from "@/lib/time-formatter"
import Flex from "../utils/Flex"
import { Spinner } from "../ui/spinner"
import { useWsClient } from "ws-request-hook"

interface DeviceSelectorProps {
  devices: DeviceDto[]
  selectedDevice: DeviceDto | null
  isDevicesLoading: boolean,
  onDeviceSelect: (device: DeviceDto) => void
}

export function DeviceSelector({ devices, selectedDevice, onDeviceSelect, isDevicesLoading }: DeviceSelectorProps) {
    const { readyState } = useWsClient();

  if (readyState !== 1) {
    return (
    <Card className="h-[624px]">
      <CardHeader className="border-b">
        <CardTitle className="flex items-center gap-2 text-lg font-bold">
          <ZapIcon className="text-purple-700" /> Device Selection
        </CardTitle>
        <CardDescription>Select a device to view its sensor data</CardDescription>
      </CardHeader>
      <CardContent className="flex justify-center items-center flex-col h-full">
        <div className="flex flex-col items-center space-y-6 text-center max-w-sm">
          <div className="relative">
            <div className="w-20 h-20 bg-destructive/10 rounded-full flex items-center justify-center">
              <WifiOff className="w-10 h-10 text-destructive" />
            </div>
            <div className="absolute -top-1 -right-1 w-6 h-6 bg-destructive/20 rounded-full flex items-center justify-center">
              <AlertTriangle className="w-4 h-4 text-destructive" />
            </div>
          </div>

          <div className="space-y-2">
            <h3 className="text-xl font-semibold text-destructive">Connection Failed</h3>
            <p className="text-muted-foreground leading-relaxed">
              Unable to establish a connection to the server socket. Please check your network connection and try again.
            </p>
          </div>

          <div className="text-xs mt-20 text-muted-foreground bg-muted/50 rounded-lg p-3 w-full">
            <p className="font-medium mb-1">Troubleshooting tips:</p>
            <ul className="text-left space-y-1">
              <li>• Check your internet connection</li>
              <li>• Verify server is powered on</li>
              <li>• Try refreshing the page</li>
            </ul>
          </div>
        </div>
      </CardContent>
    </Card>
    )
  }

  return (
    <Card className="h-[624px]">
      <CardHeader className="border-b">
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
          <div className="space-y-2 px-4">
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
                      <Badge variant="default" className="bg-green-500 text-white">
                        <Wifi className="h-3 w-3 mr-1" />
                        Online
                      </Badge>
                    ) : (
                      <Badge variant="destructive" className="text-white">
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
