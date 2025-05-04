"use client"

import type React from "react"

import { useState, useRef, useEffect } from "react"
import { Card, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { ScrollArea } from "@/components/ui/scroll-area"
import { Send, Bot, User, Sparkles, RefreshCw } from "lucide-react"
import { cn } from "@/lib/utils"
import { TextGenerateEffect } from "../ui/text-generate-effect"

interface Message {
  id: string
  content: string
  role: "user" | "assistant"
  timestamp: Date
}

interface AIChatCardProps {
  className?: string
  title?: string
  initialMessages?: Message[]
  onSendMessage?: (message: string) => Promise<string>
}

export function AIChatCard({
  className,
  title = "AI Assistant",
  initialMessages = [],
  onSendMessage,
}: AIChatCardProps) {
  const [messages, setMessages] = useState<Message[]>(initialMessages)
  const [input, setInput] = useState("")
  const [isLoading, setIsLoading] = useState(false)
  const messagesEndRef = useRef<HTMLDivElement>(null)
  const inputRef = useRef<HTMLInputElement>(null)

  // Sample responses for demo purposes
  const sampleResponses = [
    "Based on the air quality data, I recommend keeping your windows closed today.",
    "Your indoor air quality has improved by 15% since yesterday. Great job!",
    "I've noticed higher than normal PM2.5 levels in your bedroom. Consider running an air purifier.",
    "The humidity levels in your home are optimal today.",
    "Based on historical patterns, air quality tends to worsen in your area around 5-7pm. Consider running filtration during those hours.",
  ]

  const scrollToBottom = () => {
    if (messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: "smooth" })
    }
  }

  // Scroll to bottom whenever messages change
  useEffect(() => {
    scrollToBottom()
  }, [messages])

  useEffect(() => {
    if (inputRef.current) {
      inputRef.current.focus()
    }
  }, [])

  const handleSendMessage = async () => {
    if (!input.trim()) return

    const userMessage: Message = {
      id: Date.now().toString(),
      content: input,
      role: "user",
      timestamp: new Date(),
    }

    setMessages((prev) => [...prev, userMessage])
    setInput("")
    setIsLoading(true)

    try {
      let responseContent: string

      if (onSendMessage) {
        responseContent = await onSendMessage(input)
      } else {
        await new Promise((resolve) => setTimeout(resolve, 1000))
        responseContent = sampleResponses[Math.floor(Math.random() * sampleResponses.length)]
      }

      const assistantMessage: Message = {
        id: Date.now().toString(),
        content: responseContent,
        role: "assistant",
        timestamp: new Date(),
      }

      setMessages((prev) => [...prev, assistantMessage])
    } catch (error) {
      console.error("Error sending message:", error)

      const errorMessage: Message = {
        id: Date.now().toString(),
        content: "Sorry, I encountered an error processing your request.",
        role: "assistant",
        timestamp: new Date(),
      }

      setMessages((prev) => [...prev, errorMessage])
    } finally {
      setIsLoading(false)
    }
  }

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault()
      handleSendMessage()
    }
  }

  const clearChat = () => {
    setMessages([])
  }

  return (
    <Card className={cn("flex flex-col", className)}>
      <CardHeader className="px-4 flex-shrink-0">
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2 text-lg font-bold">
            <Bot className="h-5 w-5 text-indigo-500" />
            {title}
            <Sparkles className="h-4 w-4 text-amber-500" />
          </CardTitle>
          <Button
            variant="ghost"
            size="icon"
            className="h-8 w-8 rounded-full"
            onClick={clearChat}
          >
            <RefreshCw className="h-4 w-4" />
            <span className="sr-only">Clear chat</span>
          </Button>
        </div>
      </CardHeader>

      <ScrollArea className="flex-1 -mt-4 h-60 border-t p-4">
        <div className="flex flex-col gap-3">
          {messages.length === 0 ? (
            <div className="flex flex-col items-center justify-center h-full py-8 text-center text-muted-foreground">
              <Bot className="h-12 w-12 mb-4 text-muted-foreground/50" />
              <p>No messages yet. Ask me about your air quality!</p>
              <div className="mt-4 flex flex-col gap-2">
                <Button
                  variant="outline"
                  size="sm"
                  className="text-xs"
                  onClick={() => setInput("What's the air quality like today?")}
                >
                  What's the air quality like today?
                </Button>
                <Button
                  variant="outline"
                  size="sm"
                  className="text-xs"
                  onClick={() => setInput("Should I run my air purifier?")}
                >
                  Should I run my air purifier?
                </Button>
              </div>
            </div>
          ) : (
            messages.map((message) => (
              <div
                key={message.id}
                className={cn(
                  "flex items-start gap-3 rounded-lg p-3",
                  message.role === "user" ? "ml-auto bg-primary text-primary-foreground" : "bg-muted",
                )}
                style={{ maxWidth: "85%" }}
              >
                <div className="flex-shrink-0 mt-0.5">
                  {message.role === "user" ? (
                    <div className="flex h-7 w-7 items-center justify-center rounded-full bg-primary-foreground/20">
                      <User className="h-4 w-4 text-primary-foreground" />
                    </div>
                  ) : (
                    <div className="flex h-7 w-7 items-center justify-center rounded-full bg-indigo-500/10">
                      <Bot className="h-4 w-4 text-indigo-500" />
                    </div>
                  )}
                </div>
                <div className="flex-1">
                {message.role === "user" ? (
                    <p className="text-sm text-primary-foreground">
                      {message.content}
                    </p>
                  ) : (
                    <TextGenerateEffect
                      className="text-sm"
                      words={message.content} 
                      duration={1} 
                      filter={false} 
                    />
                  )}

                  <p
                    className={cn(
                      "text-xs mt-1",
                      message.role === "user" ? "text-primary-foreground/70" : "text-muted-foreground",
                    )}
                  >
                    {new Intl.DateTimeFormat("en-US", {
                      hour: "numeric",
                      minute: "numeric",
                      hour12: true,
                    }).format(message.timestamp)}
                  </p>
                </div>
              </div>
            ))
          )}
          {isLoading && (
            <div className="flex items-center gap-3 rounded-lg p-3 bg-muted" style={{ maxWidth: "85%" }}>
              <div className="flex-shrink-0 mt-0.5">
                <div className="flex h-7 w-7 items-center justify-center rounded-full bg-indigo-500/10">
                  <Bot className="h-4 w-4 text-indigo-500" />
                </div>
              </div>
              <div className="flex space-x-1">
                <div className="h-2 w-2 animate-bounce rounded-full bg-indigo-500 [animation-delay:-0.3s]"></div>
                <div className="h-2 w-2 animate-bounce rounded-full bg-indigo-500 [animation-delay:-0.15s]"></div>
                <div className="h-2 w-2 animate-bounce rounded-full bg-indigo-500"></div>
              </div>
            </div>
          )}
          <div ref={messagesEndRef} />
        </div>
      </ScrollArea>

      <CardFooter className="border-t -mt-6 flex-shrink-0">
        <div className="flex w-full items-center gap-2">
          <Input
            ref={inputRef}
            placeholder="Ask about your air quality..."
            value={input}
            onChange={(e) => setInput(e.target.value)}
            onKeyDown={handleKeyDown}
            className="flex-1"
            disabled={isLoading}
          />
          <Button
            size="icon"
            onClick={handleSendMessage}
            disabled={!input.trim() || isLoading}
            className="shrink-0 bg-indigo-500 hover:bg-indigo-600"
          >
            <Send className="h-4 w-4 dark:text-white" />
            <span className="sr-only">Send message</span>
          </Button>
        </div>
      </CardFooter>
    </Card>
  )
}