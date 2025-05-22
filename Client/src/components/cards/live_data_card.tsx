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
    const { onMessage, readyState } = useWsClient();


    useAutoSubscription([WebsocketTopics.Ai])


    useEffect(() => {
        const unsubscribe = onMessage<LiveAiFeedbackDto>(
        WebsocketTopics.Ai,
        (dto) => {
            console.log("✅ AI WebSocket message received:", dto);

            const advice = dto.aiAdvice;
            setMessages((prev) => {
                if (prev[prev.length - 1] !== advice) {
                    return [...prev, advice];
                }
                return [...prev];
            });

            setTimestamps((prev) => [...prev, new Date()]);
        }
    );

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
          </CardTitle>
        </div>
      </CardHeader>

      <ScrollArea className="h-24 p-4 border-t text-center text-sm text-muted-foreground">
        {messages.length > 0 ? messages[messages.length - 1] : "Waiting for data..."}
      </ScrollArea>
    </Card>
  )
}