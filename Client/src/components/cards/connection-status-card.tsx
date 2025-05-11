import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { CheckCircle2, WifiOff, Loader2, AlertTriangle } from "lucide-react"
import { useEffect } from "react"
import { useWsClient } from "ws-request-hook"

export interface ConnectionStatusCardProps {
  title?: string
  description?: string
  className?: string
}

export function ConnectionStatusCard({
  title = "Connection Status",
  description = "Your connection status to backend",
  className = "",
}: ConnectionStatusCardProps) {
  const { readyState } = useWsClient()

  // WebSocket readyState:
  // 0: CONNECTING
  // 1: OPEN (connected)
  // 2: CLOSING
  // 3: CLOSED (disconnected)

  const getStatusInfo = () => {
    switch (readyState) {
      case 0:
        return {
          badge: {
            text: "Connecting",
            variant: "outline",
            className: "bg-amber-100 text-amber-700 hover:bg-amber-100",
          },
          icon: <Loader2 className="h-8 w-8 text-amber-500 animate-spin" />,
          title: "Establishing connection",
          description: "Please wait while we connect to the server",
        }
      case 1:
        return {
          badge: { text: "Connected", variant: "default", className: "bg-emerald-500 hover:bg-emerald-600 text-white" },
          icon: (
            <div className="relative">
              <CheckCircle2 className="h-8 w-8 text-emerald-500" />
              <span className="absolute top-0 right-0 h-3 w-3 rounded-full bg-emerald-500">
                <span className="absolute inset-0 rounded-full bg-emerald-500 opacity-75 animate-ping"></span>
              </span>
            </div>
          ),
          title: "You are currently connected!",
          description: "All systems operational",
        }
      case 2:
        return {
          badge: {
            text: "Closing",
            variant: "outline",
            className: "bg-orange-100 text-orange-700 hover:bg-orange-100",
          },
          icon: <AlertTriangle className="h-8 w-8 text-orange-500" />,
          title: "Connection closing",
          description: "The connection is in the process of closing",
        }
      case 3:
      default:
        return {
          badge: { text: "Disconnected", variant: "destructive", className: "" },
          icon: <WifiOff className="h-8 w-8 text-destructive" />,
          title: "Connection lost",
          description: "Please check your network",
        }
    }
  }

  const statusInfo = getStatusInfo()

  return (
    <Card className={`w-full ${className}`}>
      <CardHeader className="pb-2">
        <div className="flex items-center justify-between">
          <CardTitle className="text-lg font-bold">{title}</CardTitle>
          <Badge className={statusInfo.badge.className} variant={statusInfo.badge.variant as any}>
            {statusInfo.badge.text}
          </Badge>
        </div>
        <CardDescription className="text-sm text-muted-foreground">{description}</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="flex items-center space-x-4 p-4 mt-7 rounded-lg bg-muted/50">
          {statusInfo.icon}
          <div>
            <h3 className="font-medium">{statusInfo.title}</h3>
            <p className="text-sm text-muted-foreground">{statusInfo.description}</p>
          </div>
        </div>
      </CardContent>
    </Card>
  )
}
