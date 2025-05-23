import CompactMarkdownRenderer from "@/components/ui/CompactMarkdownRenderer";
import {
    Card,
    CardHeader,
    CardTitle,
    CardDescription,
    CardContent,
    CardFooter,
} from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { useState, useEffect } from "react";

export default function AIModulePage() {
    const [input, setInput] = useState("");
    const [output, setOutput] = useState("Waiting for you...");
    const [lastUpdated, setLastUpdated] = useState<string | null>(null);
    const [timeRange, setTimeRange] = useState("7d");
    const [historyResponse, setHistoryResponse] = useState("Waiting for your request...");
    const [isAnalyzing, setIsAnalyzing] = useState(false);
    const [isLoadingHistory, setIsLoadingHistory] = useState(false);

    const handleAnalyze = async () => {
        setIsAnalyzing(true);
        setOutput("Answering your request...");

        try {
            const res = await fetch("http://localhost:8080/api/ai/chat", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer your_token_if_needed"
                },
                body: JSON.stringify({ message: input })
            });

            if (!res.ok) throw new Error("Failed to fetch response from AI");
            const data = await res.json();
            const message = (data.message || JSON.stringify(data, null, 2)).replace(/\\n/g, "\n");
            setOutput(message);
        } catch (error) {
            console.error("AI query error:", error);
            setOutput("⚠️ Failed to get a response from the AI.");
        } finally {
            setIsAnalyzing(false);
        }
    };


    // Fetch historical AI response for IoT data based on selectable time range when user clicks the button
    const handleHistoryAnalyze = async () => {
        setIsLoadingHistory(true);
        setHistoryResponse("Looking at your data...");

        try {
            const res = await fetch("http://localhost:8080/api/ai/historical_data_analysis", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": "Bearer your_token_if_needed"
                },
                body: JSON.stringify({ message: timeRange }) // assumes timeRange is the expected message format
            });

            if (!res.ok) throw new Error("Failed to fetch historical AI data");
            const data = await res.json();
            const message = (data.user_friendly_advice || JSON.stringify(data, null, 2)).replace(/\\n/g, "\n");
            setHistoryResponse(message);
            setLastUpdated(new Date().toLocaleTimeString());
        } catch (error) {
            console.error("Error fetching historical AI data:", error);
            setHistoryResponse("⚠️ Failed to fetch historical AI data.");
        } finally {
            setIsLoadingHistory(false);
        }
    };


    return (
        <div className="px-4 lg:px-6">
            <div className="grid gap-4 lg:grid-cols-12 py-2">
                <div className="lg:col-span-12 space-y-4">
                    {/* Historical Insights */}
                    <div>
                        <Card className="h-full shadow-md border-2 border-blue-200 bg-blue-50/10">
                            <CardHeader>
                                <CardTitle>🔍 Historical Air Review</CardTitle>
                                <CardDescription>
                                    Choose a time range to explore trends from past sensor readings.
                                    {lastUpdated && (
                                        <span className="ml-2 text-xs text-muted-foreground">
                                            (Last updated: {lastUpdated})
                                        </span>
                                    )}
                                </CardDescription>

                            </CardHeader>
                            <CardContent className="px-6 pb-6">
                                <div className="mb-4">
                                    <label className="text-sm font-medium mr-2">Time Range:</label>
                                    <select
                                        value={timeRange}
                                        onChange={(e) => setTimeRange(e.target.value)}
                                        className="text-sm rounded-md border border-gray-300 px-3 py-2 bg-white dark:bg-gray-800 shadow-sm focus:outline-none focus:ring-2 focus:ring-indigo-400 focus:border-indigo-400 transition-colors"
                                    >
                                        <option value="24h">24 Hours</option>
                                        <option value="48h">48 Hours</option>
                                        <option value="72h">72 Hours</option>
                                        <option value="5d">5 Days</option>
                                        <option value="7d">7 Days</option>
                                        <option value="14d">14 Days</option>
                                        <option value="30d">30 Days</option>
                                    </select>
                                    <button
                                      onClick={handleHistoryAnalyze}
                                      disabled={!timeRange.trim()}
                                      data-slot="button"
                                      className="ml-2 mt-2 inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive text-primary-foreground shadow-xs size-9 shrink-0 bg-indigo-500 hover:bg-indigo-600"
                                    >
                                      <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="lucide lucide-send h-4 w-4 dark:text-white" aria-hidden="true">
                                        <path d="M14.536 21.686a.5.5 0 0 0 .937-.024l6.5-19a.496.496 0 0 0-.635-.635l-19 6.5a.5.5 0 0 0-.024.937l7.93 3.18a2 2 0 0 1 1.112 1.11z" />
                                        <path d="m21.854 2.147-10.94 10.939" />
                                      </svg>
                                        {isLoadingHistory ? (
                                            <span className="text-xs">Analyzing...</span>
                                        ) : (
                                            <span className="sr-only">Analyze History</span>
                                        )}

                                    </button>
                                </div>
                                <CompactMarkdownRenderer content={historyResponse} />
                            </CardContent>
                        </Card>
                    </div>

                    {/* Manual Analysis */}
                    <div>
                        <Card className="h-full shadow-md border-2 border-yellow-200 bg-yellow-50/10">
                            <CardHeader>
                                <CardTitle>🤖 AI Assistant</CardTitle>
                                <CardDescription>
                                    Ask about your indoor environment or enter live data for smart advice.
                                </CardDescription>

                            </CardHeader>
                            <CardContent className="px-6 pb-4">
                              <div className="flex items-end gap-2">
                                <Textarea
                                  placeholder="Describe what you want the AI to analyze..."
                                  value={input}
                                  onChange={(e) => setInput(e.target.value)}
                                  className="min-h-[64px] text-sm flex-1"
                                />
                                <button
                                  onClick={handleAnalyze}
                                  disabled={!input.trim()}
                                  data-slot="button"
                                  className="inline-flex items-center justify-center gap-2 whitespace-nowrap rounded-md text-sm font-medium transition-all disabled:pointer-events-none disabled:opacity-50 [&_svg]:pointer-events-none [&_svg:not([class*='size-'])]:size-4 [&_svg]:shrink-0 outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px] aria-invalid:ring-destructive/20 dark:aria-invalid:ring-destructive/40 aria-invalid:border-destructive text-primary-foreground shadow-xs size-9 shrink-0 bg-indigo-500 hover:bg-indigo-600"
                                >
                                  <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" className="lucide lucide-send h-4 w-4 dark:text-white" aria-hidden="true">
                                    <path d="M14.536 21.686a.5.5 0 0 0 .937-.024l6.5-19a.496.496 0 0 0-.635-.635l-19 6.5a.5.5 0 0 0-.024.937l7.93 3.18a2 2 0 0 1 1.112 1.11z" />
                                    <path d="m21.854 2.147-10.94 10.939" />
                                  </svg>
                                    {isAnalyzing ? (
                                        <span className="text-xs">Analyzing...</span>
                                    ) : (
                                        <span className="sr-only">Send message</span>
                                    )}
                                </button>
                              </div>
                            </CardContent>
                        </Card>
                    </div>
                </div>
                <div className="lg:col-span-12">
                    <Card className="h-full shadow-md border-2 border-green-200 bg-green-50/10">
                        <CardHeader>
                            <CardTitle>🧠 AI Response</CardTitle>
                            <CardDescription>
                                Here's what the assistant thinks based on your data or question.
                            </CardDescription>
                        </CardHeader>
                        <CardContent>
                            <CompactMarkdownRenderer content={output} />
                        </CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
}