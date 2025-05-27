"use client"
import React, {useRef} from "react"
import { Card, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Bot, Sparkles } from "lucide-react";
import { cn } from "@/lib/utils";
import { useEffect, useState } from "react";
import {useWsClient} from "ws-request-hook";
import { Badge } from "../ui/badge";

interface LiveDataCardProps {
  className?: string
  title?: string
}

export function LiveDataCard({
  className,
  title = "Environment Watcher",
}: LiveDataCardProps) {
    const [messages, setMessages] = useState<string[]>([])
    const [timestamps, setTimestamps] = useState<Date[]>([])
    const [status, setStatus] = useState<"good" | "warning" | null>(null);
    const { onMessage, readyState } = useWsClient();

    useEffect(() => {
      const unsubscribe = onMessage("*", (raw: any) => {
        console.log("🔥 RAW WebSocket message:", raw);

        let aiAdvice = raw?.data?.aiAdvice || raw?.aiAdvice;

        // Remove stringified quotes if wrapped
        if (typeof aiAdvice === "string" && aiAdvice.startsWith('"') && aiAdvice.endsWith('"')) {
          try {
            aiAdvice = JSON.parse(aiAdvice);
          } catch (e) {
            console.warn("❗ Failed to parse AI advice:", aiAdvice, e);
          }
        }

        if (typeof aiAdvice === "string" && aiAdvice.length > 0) {
          setMessages([aiAdvice]);
          setTimestamps((prev) => [...prev, new Date()]);
          // Set status based on AI advice
          if (aiAdvice.toLowerCase().includes("all values are good")) {
            setStatus("good");
          } else {
            setStatus("warning");
          }
          console.log("✅ Cleaned AI advice:", aiAdvice);
        } else {
          console.warn("❗ Unexpected message format:", raw);
        }
      });

      return () => unsubscribe();
    }, [readyState]);

  return (
    <Card className={cn("flex flex-col", className)}>
      <CardHeader className="px-4 flex-shrink-0">
        <div className="flex w-full items-center justify-between">
          <CardTitle className="flex items-center gap-2 text-lg font-bold">
            <div className="p-2 bg-gradient-to-br from-indigo-500 to-indigo-600 rounded-lg">
              <Bot className="h-5 w-5 text-white" />
            </div>
            {title}
          </CardTitle>

          <div className="flex items-center gap-2">
            <Badge variant="outline" className="bg-background/50 backdrop-blur-sm">
                {status === "good" && <span className="text-muted-foreground">All values are good!</span>}
                {status === "warning" && <span className="text-muted-foreground">Warning values are NOT good!</span>}
                {status === null && <span className="text-muted-foreground">Waiting on status...</span>}
            </Badge>
          </div>
        </div>
      </CardHeader>

      <ScrollArea className="h-24 p-4 border-t text-center text-base md:text-lg text-muted-foreground">
        {messages.length > 0 ? messages[messages.length - 1] : "Waiting for data..."}
      </ScrollArea>
    </Card>
  )
}