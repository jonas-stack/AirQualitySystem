import { useEffect, useId, useRef, useState } from "react"
import { AnimatePresence, motion } from "motion/react"
import type { Card, CardArray } from "@/types/card"
import { ExpandedCard } from "./expanded-card"
import { CardItem } from "./card-item"
import { useOutsideClick } from "@/hooks"

interface ExpandableCardListProps {
  cards?: CardArray
  className?: string
}

export function ExpandableCardList({
  cards,
  className = "max-w-2xl mx-auto w-full gap-4",
}: ExpandableCardListProps) {
  const [active, setActive] = useState<Card | boolean | null>(null)
  const ref = useRef<HTMLDivElement>(null)
  const id = useId()

  useEffect(() => {
    function onKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setActive(false)
      }
    }

    if (active && typeof active === "object") {
      document.body.style.overflow = "hidden"
    } else {
      document.body.style.overflow = "auto"
    }

    window.addEventListener("keydown", onKeyDown)
    return () => window.removeEventListener("keydown", onKeyDown)
  }, [active])

  useOutsideClick(ref, () => setActive(null))

  return (
    <>
      <AnimatePresence>
        {active && typeof active === "object" && (
          <motion.div
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            exit={{ opacity: 0 }}
            className="fixed inset-0 bg-black/20 h-full w-full z-10"
          />
        )}
      </AnimatePresence>

      <AnimatePresence>
        {active && typeof active === "object" && (
          <ExpandedCard card={active} id={id} onClose={() => setActive(null)} containerRef={ref} />
        )}
      </AnimatePresence>

      <ul className={className}>
        {cards && cards.map((card) => (
          <CardItem key={`card-item-${card.title}`} card={card} id={id} onClick={(card) => setActive(card)} />
        ))}
      </ul>
    </>
  )
}
