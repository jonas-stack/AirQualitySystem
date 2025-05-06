import { useState } from "react"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CheckCircle2, WifiOff } from "lucide-react"

export interface ConnectionStatusCardProps {
  isConnected?: boolean
  onConnectionChange?: (isConnected: boolean) => void
  title?: string
  description?: string
  className?: string
}

export function ConnectionStatusCard({
  isConnected: initialIsConnected = true,
  onConnectionChange,
  title = "Connection Status",
  description = "Your connection status to backend",
  className = "",
}: ConnectionStatusCardProps) {
  const [internalIsConnected, setInternalIsConnected] = useState(initialIsConnected)

  const isConnected = onConnectionChange ? initialIsConnected : internalIsConnected

  const handleToggle = () => {
    const newStatus = !isConnected
    if (onConnectionChange) {
      onConnectionChange(newStatus)
    } else {
      setInternalIsConnected(newStatus)
    }
  }

  return (
    <Card className={`w-full ${className}`}>
      <CardHeader className="pb-2">
        <div className="flex items-center justify-between">
          <CardTitle className="text-lg font-bold">{title}</CardTitle>
          {isConnected ? (
            <Badge className="bg-emerald-500 hover:bg-emerald-600 text-white">Connected</Badge>
          ) : (
            <Badge variant="destructive">Disconnected</Badge>
          )}
        </div>
        <CardDescription className="text-sm text-muted-foreground">{description}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex items-center space-x-4 p-4 mt-7 rounded-lg bg-muted/50">
            {isConnected ? (
              <>
                <div className="relative">
                  <CheckCircle2 className="h-8 w-8 text-emerald-500" />
                  <span className="absolute top-0 right-0 h-3 w-3 rounded-full bg-emerald-500">
                    <span className="absolute inset-0 rounded-full bg-emerald-500 opacity-75 animate-ping"></span>
                  </span>
                </div>
                <div>
                  <h3 className="font-medium">You are currently connected!</h3>
                  <p className="text-sm text-muted-foreground">All systems operational</p>
                </div>
              </>
            ) : (
              <>
                <WifiOff className="h-8 w-8 text-destructive" />
                <div>
                  <h3 className="font-medium">Connection lost</h3>
                  <p className="text-sm text-muted-foreground">Please check your network</p>
                </div>
              </>
            )}
          </div>
      </CardContent>
    </Card>
  )
}
