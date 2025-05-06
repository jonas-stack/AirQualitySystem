"use client"

import type React from "react"
import { motion } from "motion/react"
import type { Card } from "@/types/card"
import { CloseIcon } from "@/components/ui/close-icon"

interface ExpandedCardProps {
  card: Card
  id: string
  onClose: () => void
  containerRef: React.RefObject<HTMLDivElement>
}

export function ExpandedCard({ card, id, onClose, containerRef }: ExpandedCardProps) {
  return (
    <div className="fixed inset-0 grid place-items-center z-[100]">
      <motion.button
        key={`button-${card.title}-${id}`}
        layout
        initial={{
          opacity: 0,
        }}
        animate={{
          opacity: 1,
        }}
        exit={{
          opacity: 0,
          transition: {
            duration: 0.05,
          },
        }}
        className="flex absolute top-2 right-2 lg:hidden items-center justify-center bg-white rounded-full h-6 w-6"
        onClick={onClose}
      >
        <CloseIcon />
      </motion.button>
      <motion.div
        layoutId={`card-${card.title}-${id}`}
        ref={containerRef}
        className="w-full max-w-[500px] h-full md:h-fit md:max-h-[90%] flex flex-col bg-white dark:bg-neutral-900 sm:rounded-3xl overflow-hidden"
      >
        <motion.div layoutId={`image-${card.title}-${id}`}>
          <img
            width={200}
            height={200}
            src={card.src || "/placeholder.svg"}
            alt={card.title}
            className="w-full h-80 lg:h-80 sm:rounded-tr-lg sm:rounded-tl-lg object-cover object-top"
          />
        </motion.div>

        <div>
          <div className="flex justify-between items-start p-4">
            <div className="">
              <motion.h3
                layoutId={`title-${card.title}-${id}`}
                className="font-bold text-neutral-700 dark:text-neutral-200"
              >
                {card.title}
              </motion.h3>
              <motion.p
                layoutId={`description-${card.description}-${id}`}
                className="text-neutral-600 dark:text-neutral-400"
              >
                {card.description}
              </motion.p>
            </div>

            <motion.a
              layoutId={`button-${card.title}-${id}`}
              href={card.ctaLink}
              target="_blank"
              className="px-4 py-3 text-sm rounded-full font-bold bg-green-500 text-white"
              rel="noreferrer"
            >
              {card.ctaText}
            </motion.a>
          </div>
          <div className="pt-4 relative px-4">
            <motion.div
              layout
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              exit={{ opacity: 0 }}
              className="text-neutral-600 text-xs md:text-sm lg:text-base h-40 md:h-fit pb-10 flex flex-col items-start gap-4 overflow-auto dark:text-neutral-400 [mask:linear-gradient(to_bottom,white,white,transparent)] [scrollbar-width:none] [-ms-overflow-style:none] [-webkit-overflow-scrolling:touch]"
            >
              {typeof card.content === "function" ? card.content() : card.content}
            </motion.div>
          </div>
        </div>
      </motion.div>
    </div>
  )
}
