import type React from "react"
export interface Card {
  description: string
  title: string
  src: string
  ctaText: string
  ctaLink: string
  content: string | (() => React.ReactNode)
}

export type CardArray = Card[]
