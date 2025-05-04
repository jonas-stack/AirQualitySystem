"use client"

import { useState } from "react"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Wind, RefreshCw } from "lucide-react"
import type { CardArray } from "@/types/card"
import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { ExpandableCardList } from "./expandable-card/expandable-card-list"

interface AirQualityInsightsCardProps {
  cards: CardArray
  title?: string
  description?: string
  className?: string
  onRefresh?: () => void
  lastUpdated?: Date
}

export function AirQualityInsightsCard({
  cards,
  title = "Air Quality Insights",
  description = "Environmental data analysis from your sensors",
  className,
  onRefresh,
  lastUpdated = new Date(),
}: AirQualityInsightsCardProps) {

  const handleRefresh = () => {
    if (onRefresh) {
      onRefresh()
    }
  }

  const formattedTime = new Intl.DateTimeFormat("en-US", {
    hour: "numeric",
    minute: "numeric",
    hour12: true,
  }).format(lastUpdated)

  return (
    <Card className={cn("overflow-hidden", className)}>
      <CardHeader className="">
        <div className="flex items-center justify-between">
          <div>
            <CardTitle className="flex items-center gap-2 text-2xl font-bold">
              <Wind className="h-5 w-5 text-emerald-500" />
              {title}
            </CardTitle>
            <CardDescription className="text-sm mt-1">
              {description} <br/> Last updated: {formattedTime}
            </CardDescription>
          </div>
          <Button
            variant="ghost"
            size="icon"
            onClick={handleRefresh}
            className="rounded-full"
          >
            <RefreshCw className="h-4 w-4" />
            <span className="sr-only">Refresh data</span>
          </Button>
        </div>
      </CardHeader>
      <CardContent className="p-0 mt-[-1rem] border-t">
              <ExpandableCardList
                cards={cards.map((card) => ({
                  ...card,
                  content: typeof card.content === "function" ? card.content : card.content,
                }))}
                className="divide-y"
              />

              <div className="absolute bottom-0 left-0 right-0 h-16 bg-gradient-to-t from-background to-transparent pointer-events-none" />
      </CardContent>
    </Card>
  )
}
