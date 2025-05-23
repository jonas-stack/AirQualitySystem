"use client"
import React, {useRef} from "react"
import { Card, CardHeader, CardTitle } from "@/components/ui/card";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Bot, Sparkles } from "lucide-react";
import { cn } from "@/lib/utils";
import { useEffect, useState } from "react";
import {useWsClient} from "ws-request-hook";
import {LiveAiFeedbackDto, WebsocketTopics} from "@/generated-client.ts";
import {useAutoSubscription} from "@/hooks/use-auto-subscription.ts";



interface LiveDataCardProps {
  className?: string
  title?: string
}

export function LiveDataCard({
  className,
  title = "🍃 Environment Watcher",
}: LiveDataCardProps) {
    const [messages, setMessages] = useState<string[]>([])
    const [timestamps, setTimestamps] = useState<Date[]>([])
    const [status, setStatus] = useState<"good" | "warning" | null>(null);
    const { onMessage, readyState } = useWsClient();

    useAutoSubscription([WebsocketTopics.Ai, WebsocketTopics.Dashboard])
    console.log("✅ Subscribed to topics: ", WebsocketTopics.Ai);

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
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2 text-lg font-bold">
            <Bot className="h-5 w-5 text-indigo-500" />
            {title}
            <Sparkles className="h-4 w-4 text-amber-500" />
            {status === "good" && <span className="text-green-500 text-xl">✅</span>}
            {status === "warning" && <span className="text-yellow-500 text-xl">❗</span>}
          </CardTitle>
        </div>
      </CardHeader>

      <ScrollArea className="h-24 p-4 border-t text-center text-sm text-muted-foreground">
        {messages.length > 0 ? messages[messages.length - 1] : "Waiting for data..."}
      </ScrollArea>
    </Card>
  )
}