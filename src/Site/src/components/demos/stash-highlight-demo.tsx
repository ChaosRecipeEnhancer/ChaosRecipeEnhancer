"use client";

import { motion, useInView } from "motion/react";
import { useEffect, useRef, useState } from "react";

const CELL = 40;
const GAP = 2;
const PAD = 16;
const COLS = 8;
const ROWS = 6;
const RED = "#EF4444";

const PICK_MS = 800;
const EMPTY_MS = 1500;
const FILL_MS = 1200;

interface StashItem {
  col: number;
  height: number;
  id: string;
  row: number;
  step: number;
  width: number;
}

const ITEMS: StashItem[] = [
  { id: "helmet", col: 0, row: 0, width: 2, height: 2, step: 1 },
  { id: "body-armor", col: 2, row: 0, width: 2, height: 3, step: 2 },
  { id: "gloves", col: 5, row: 0, width: 2, height: 2, step: 3 },
  { id: "boots", col: 0, row: 3, width: 2, height: 2, step: 4 },
  { id: "weapon", col: 5, row: 3, width: 1, height: 3, step: 5 },
  { id: "ring", col: 5, row: 2, width: 1, height: 1, step: 6 },
  { id: "amulet", col: 6, row: 2, width: 1, height: 1, step: 7 },
  { id: "belt", col: 3, row: 3, width: 2, height: 1, step: 8 },
];

const SORTED = [...ITEMS].sort((a, b) => a.step - b.step);

function center(item: StashItem) {
  return {
    x:
      PAD +
      item.col * (CELL + GAP) +
      (item.width * CELL + (item.width - 1) * GAP) / 2,
    y:
      PAD +
      item.row * (CELL + GAP) +
      (item.height * CELL + (item.height - 1) * GAP) / 2,
  };
}

export function StashHighlightDemo() {
  const ref = useRef<HTMLDivElement>(null);
  const inView = useInView(ref, { once: true, margin: "-100px" });
  const [started, setStarted] = useState(false);
  const [step, setStep] = useState(-1);
  const [cycle, setCycle] = useState(0);

  useEffect(() => {
    if (inView) {
      const t = setTimeout(() => setStarted(true), 300);
      return () => clearTimeout(t);
    }
  }, [inView]);

  useEffect(() => {
    if (!started) {
      return;
    }
    setStep(-1);
    let off = false;

    function run() {
      let i = 0;
      function pick() {
        if (off) {
          return;
        }
        setStep(i);
        i++;
        if (i < SORTED.length) {
          setTimeout(pick, PICK_MS);
        } else {
          // All picked — empty grid pause, then refill
          setTimeout(() => {
            if (off) {
              return;
            }
            setStep(-1);
            setCycle((c) => c + 1);
            setTimeout(() => {
              if (off) {
                return;
              }
              run();
            }, FILL_MS);
          }, EMPTY_MS);
        }
      }
      // Let items animate in before picking starts
      setTimeout(() => {
        if (!off) {
          pick();
        }
      }, FILL_MS);
    }

    run();
    return () => {
      off = true;
    };
  }, [started]);

  const gw = COLS * CELL + (COLS - 1) * GAP;
  const gh = ROWS * CELL + (ROWS - 1) * GAP;
  const target = step >= 0 ? SORTED[step] : null;
  const cursor = target
    ? center(target)
    : { x: gw / 2 + PAD, y: gh + PAD + 10 };

  return (
    <div
      className="flex w-full flex-col items-center justify-center py-4 md:py-8"
      ref={ref}
    >
      <motion.div
        animate={started ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
        className="flex flex-col overflow-hidden rounded-xl border border-cre-border bg-cre-bg-deep shadow-2xl"
        initial={{ opacity: 0, y: 20 }}
        transition={{ duration: 0.6, ease: "easeOut" }}
      >
        {/* Tab Header */}
        <div className="flex h-10 items-center border-cre-border border-b bg-cre-bg-container px-4">
          <div className="flex gap-2">
            <div className="h-3 w-3 rounded-full bg-[#FF5F56]" />
            <div className="h-3 w-3 rounded-full bg-[#FFBD2E]" />
            <div className="h-3 w-3 rounded-full bg-[#27C93F]" />
          </div>
          <div className="ml-4 flex h-full items-center border-cre-gold/60 border-t-2 bg-red-600/80 px-4 font-medium text-sm text-white">
            Chaos Items
          </div>
          <div className="flex h-full items-center px-4 text-cre-text-muted text-sm">
            Currency
          </div>
          <div className="flex h-full items-center px-4 text-cre-text-muted text-sm">
            Maps
          </div>
        </div>

        {/* Grid */}
        <div
          className="relative border-cre-gold/20 border-t bg-[#0a0a0a]"
          style={{ width: gw + PAD * 2, height: gh + PAD * 2, padding: PAD }}
        >
          {/* Background cells */}
          <div
            className="grid"
            style={{
              gridTemplateColumns: `repeat(${COLS}, ${CELL}px)`,
              gridTemplateRows: `repeat(${ROWS}, ${CELL}px)`,
              gap: `${GAP}px`,
            }}
          >
            {Array.from({ length: COLS * ROWS }).map((_, i) => (
              // biome-ignore lint/suspicious/noArrayIndexKey: Static grid
              <div className="border border-[#1f1f1f] bg-[#141414]" key={i} />
            ))}
          </div>

          {/* Items — all start visible, get removed one by one */}
          {SORTED.map((item, idx) => {
            // Already picked — gone
            if (step > idx) {
              return null;
            }

            const picking = step === idx;
            const left = PAD + item.col * (CELL + GAP);
            const top = PAD + item.row * (CELL + GAP);
            const w = item.width * CELL + (item.width - 1) * GAP;
            const h = item.height * CELL + (item.height - 1) * GAP;

            return (
              <motion.div
                animate={{
                  opacity: picking ? 0 : 1,
                  scale: picking ? 0.85 : 1,
                }}
                className="absolute origin-center"
                initial={{ opacity: 0, scale: 0.92 }}
                key={`${item.id}-${cycle}`}
                style={{ left, top, width: w, height: h }}
                transition={{
                  duration: picking ? 0.3 : 0.35,
                  delay: picking ? 0.2 : idx * 0.06,
                  ease: "easeOut",
                }}
              >
                {/* Red overlay */}
                <div
                  className="absolute inset-0 rounded-sm border-2"
                  style={{
                    borderColor: RED,
                    backgroundColor: `${RED}15`,
                    boxShadow: `inset 0 0 12px ${RED}25`,
                  }}
                />
              </motion.div>
            );
          })}

          {/* Cursor */}
          <motion.div
            animate={{
              left: cursor.x,
              top: cursor.y,
              opacity: step >= 0 ? 1 : 0,
            }}
            className="pointer-events-none absolute z-20"
            transition={{
              left: { type: "spring", stiffness: 170, damping: 22 },
              top: { type: "spring", stiffness: 170, damping: 22 },
              opacity: { duration: 0.15 },
            }}
          >
            {/* Click press animation */}
            <motion.div
              animate={{ scale: [1, 0.75, 1.1, 1] }}
              initial={{ scale: 1 }}
              key={`click-${step}-${cycle}`}
              transition={{
                delay: 0.2,
                duration: 0.3,
                ease: "easeInOut",
                times: [0, 0.3, 0.7, 1],
              }}
            >
              <svg
                aria-hidden="true"
                className="h-5 w-5 drop-shadow-md"
                fill="white"
                stroke="#333"
                strokeLinejoin="round"
                strokeWidth="1"
                viewBox="0 0 20 24"
              >
                <path d="M2 1L2 18L6.5 13.5L10.5 21.5L13 20.5L9 12.5L15 12.5Z" />
              </svg>
            </motion.div>

            {/* Click ripple */}
            <motion.div
              animate={{ scale: 2.5, opacity: 0 }}
              className="absolute -top-1.5 -left-1.5 rounded-full"
              initial={{ scale: 0, opacity: 0.4 }}
              key={`ripple-${step}-${cycle}`}
              style={{
                width: 12,
                height: 12,
                border: `2px solid ${RED}80`,
              }}
              transition={{ delay: 0.25, duration: 0.4, ease: "easeOut" }}
            />
          </motion.div>
        </div>
      </motion.div>
    </div>
  );
}
