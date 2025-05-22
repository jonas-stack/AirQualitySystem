"use client"
import type React from "react"
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Bot, Sparkles } from "lucide-react"
import { cn } from "@/lib/utils"
import { TextGenerateEffect } from "../ui/text-generate-effect"

import { useEffect, useState } from "react"

interface LiveDataCardProps {
  className?: string
  title?: string
}

export function LiveDataCard({
  className,
  title = "Air Quality Assistant",
}: LiveDataCardProps) {
  const [messages, setMessages] = useState<string[]>([])
  const [timestamps, setTimestamps] = useState<Date[]>([])

  /*
  useEffect(() => {
    const fetchLiveData = async () => {
      try {
        const response = await fetch("ws://localhost:8181");
        const data = await response.json();
        console.log("Live data response:", data);
        if (data?.user_friendly_advice && data.user_friendly_advice !== messages[messages.length - 1]) {
          setMessages((prev) => [...prev, data.user_friendly_advice]);
          setTimestamps((prev) => [...prev, new Date()]);
        }
      } catch (err) {
        console.error("Error fetching live data:", err);
      }
    };

    fetchLiveData();
    const interval = setInterval(fetchLiveData, 330000);
    return () => clearInterval(interval);
  }, [messages]);*/

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