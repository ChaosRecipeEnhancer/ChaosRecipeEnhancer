"use client";

import { AnimatePresence, animate, motion, useInView } from "motion/react";
import Image from "next/image";
import { useEffect, useRef, useState } from "react";
import { CheckIcon, RefreshIcon } from "@/components/icons";

const ITEMS = [
  {
    id: "helmet",
    name: "Helmet",
    color: "#D4A843",
    icon: "/images/items/helmet.png",
  },
  {
    id: "body-armor",
    name: "Body Armour",
    color: "#6B7280",
    icon: "/images/items/body-armor.png",
  },
  {
    id: "gloves",
    name: "Gloves",
    color: "#22C55E",
    icon: "/images/items/gloves.png",
  },
  {
    id: "boots",
    name: "Boots",
    color: "#3B82F6",
    icon: "/images/items/boots.png",
  },
  {
    id: "weapon",
    name: "Weapon",
    color: "#06B6D4",
    icon: "/images/items/weapon.png",
  },
  {
    id: "ring",
    name: "Ring",
    color: "#EF4444",
    icon: "/images/items/ring.png",
  },
  {
    id: "amulet",
    name: "Amulet",
    color: "#EF4444",
    icon: "/images/items/amulet.png",
  },
  {
    id: "belt",
    name: "Belt",
    color: "#EF4444",
    icon: "/images/items/belt.png",
  },
];

// Each wave simulates items accumulating across fetches.
// Wave 1: sparse — body armor & weapons haven't dropped yet.
// Wave 2: building up after a zone change auto-refresh.
// Wave 3: full counts (matches real UI screenshot) — set complete.
const WAVES: number[][] = [
  [0, 0, 0, 0, 0, 0, 0, 0],
  [1, 0, 2, 1, 0, 3, 1, 2],
  [3, 2, 4, 3, 2, 7, 4, 5],
  [5, 3, 7, 6, 4, 12, 8, 9],
];

const CYCLE_MS = 13_000;

function AnimatedCount({ value }: { value: number }) {
  const [display, setDisplay] = useState(0);
  const prev = useRef(0);

  useEffect(() => {
    const controls = animate(prev.current, value, {
      duration: 0.5,
      ease: "easeOut",
      onUpdate: (v) => setDisplay(Math.round(v)),
    });
    prev.current = value;
    return () => controls.stop();
  }, [value]);

  return <span>{display}</span>;
}

export function RecipeTrackerDemo() {
  const containerRef = useRef<HTMLDivElement>(null);
  const isInView = useInView(containerRef, { margin: "-100px", once: true });
  const [counts, setCounts] = useState(WAVES[0]);
  const [fetchActive, setFetchActive] = useState(false);
  const [zoneMsg, setZoneMsg] = useState<string | null>(null);
  const [complete, setComplete] = useState(false);

  useEffect(() => {
    if (!isInView) {
      return;
    }

    let active = true;
    let timers: ReturnType<typeof setTimeout>[] = [];

    function clearTimers() {
      for (const t of timers) {
        clearTimeout(t);
      }
      timers = [];
    }

    function after(fn: () => void, ms: number) {
      if (!active) {
        return;
      }
      timers.push(
        setTimeout(() => {
          if (active) {
            fn();
          }
        }, ms)
      );
    }

    function runCycle() {
      clearTimers();

      // Reset all state
      setCounts(WAVES[0]);
      setFetchActive(false);
      setZoneMsg(null);
      setComplete(false);

      // 0.8s — Fetch button presses
      after(() => setFetchActive(true), 800);

      // 1.3s — Button releases, first wave of items arrives
      after(() => {
        setFetchActive(false);
        setCounts(WAVES[1]);
      }, 1300);

      // 3.8s — Zone change triggers auto-refresh
      after(() => setZoneMsg("Zone changed — auto-refreshing"), 3800);

      // 5.0s — Second wave arrives
      after(() => {
        setZoneMsg(null);
        setCounts(WAVES[2]);
      }, 5000);

      // 7.5s — Another zone change
      after(() => setZoneMsg("Zone changed — auto-refreshing"), 7500);

      // 8.7s — Final wave arrives
      after(() => {
        setZoneMsg(null);
        setCounts(WAVES[3]);
      }, 8700);

      // 9.5s — Set complete!
      after(() => setComplete(true), 9500);

      // Loop
      after(() => runCycle(), CYCLE_MS);
    }

    runCycle();

    return () => {
      active = false;
      clearTimers();
    };
  }, [isInView]);

  return (
    <div
      className="flex w-full flex-col items-center justify-center py-4 md:py-8"
      ref={containerRef}
    >
      {/* Set Complete badge — reserved space above tracker, no layout shift */}
      <div className="relative mb-3 flex h-8 items-center justify-center">
        <AnimatePresence>
          {complete && (
            <motion.div
              animate={{ opacity: 1, scale: 1, y: 0 }}
              className="absolute flex items-center gap-2 whitespace-nowrap rounded-full border border-red-500/20 bg-red-500/10 px-4 py-1.5 text-red-400"
              exit={{ opacity: 0, scale: 0.9, y: 5 }}
              initial={{ opacity: 0, scale: 0.9, y: 5 }}
              transition={{ duration: 0.4, ease: "easeOut" }}
            >
              <CheckIcon />
              <span className="font-medium text-sm">Set Complete!</span>
            </motion.div>
          )}
        </AnimatePresence>
      </div>

      {/* Main tracker container */}
      <motion.div
        animate={isInView ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
        className="relative rounded-xl border border-cre-border bg-cre-bg-container/80 p-4 shadow-xl backdrop-blur sm:px-5"
        initial={{ opacity: 0, y: 20 }}
        transition={{ duration: 0.6, ease: "easeOut" }}
      >
        {/* Items: 2-col grid on mobile, 4-col on sm, single row on md+ */}
        <div className="grid grid-cols-2 place-items-center gap-3 sm:grid-cols-4 md:flex md:gap-3">
          {ITEMS.map((item, i) => (
            <div className="flex flex-col items-center gap-1.5" key={item.id}>
              <div className="relative flex h-10 w-10 items-center justify-center overflow-hidden rounded-full border-2 border-cre-border">
                {/* Color fill — dim when empty, full when items exist */}
                <motion.div
                  animate={{ opacity: counts[i] > 0 ? 1 : 0.12 }}
                  className="absolute inset-0"
                  style={{ backgroundColor: item.color }}
                  transition={{ duration: 0.4, ease: "easeOut" }}
                />
                {/* Item icon */}
                <div className="relative z-10 h-5 w-5">
                  <Image
                    alt={item.name}
                    className="object-contain brightness-0 invert"
                    fill
                    sizes="20px"
                    src={item.icon}
                  />
                </div>
              </div>
              {/* Count */}
              <span className="font-mono text-cre-text-secondary text-xs tabular-nums">
                <AnimatedCount value={counts[i]} />
              </span>
            </div>
          ))}
        </div>

        {/* Red border pulse on complete */}
        <AnimatePresence>
          {complete && (
            <motion.div
              animate={{
                borderColor: [
                  "rgba(239, 68, 68, 0)",
                  "rgba(239, 68, 68, 0.5)",
                  "rgba(239, 68, 68, 0)",
                ],
              }}
              className="pointer-events-none absolute inset-0 rounded-xl border-2"
              exit={{ opacity: 0 }}
              initial={{ opacity: 1 }}
              transition={{ duration: 1.5, ease: "easeInOut" }}
            />
          )}
        </AnimatePresence>
      </motion.div>

      {/* Fetch Items button */}
      <motion.div
        animate={{
          backgroundColor: fetchActive
            ? "rgba(212, 168, 67, 0.08)"
            : "rgba(255, 255, 255, 0.02)",
          borderColor: fetchActive
            ? "rgba(212, 168, 67, 0.5)"
            : "rgba(255, 255, 255, 0.08)",
          color: fetchActive ? "#D4A843" : "rgba(255, 255, 255, 0.45)",
          scale: fetchActive ? 0.93 : 1,
        }}
        className="mt-4 flex cursor-default select-none items-center gap-2 rounded-full border px-4 py-1.5 font-medium text-sm"
        transition={{ duration: 0.15 }}
      >
        <motion.svg
          animate={{ rotate: fetchActive ? 180 : 0 }}
          aria-hidden="true"
          className="h-3.5 w-3.5"
          fill="none"
          stroke="currentColor"
          transition={{ duration: 0.3, ease: "easeInOut" }}
          viewBox="0 0 24 24"
          xmlns="http://www.w3.org/2000/svg"
        >
          <path
            d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
            strokeLinecap="round"
            strokeLinejoin="round"
            strokeWidth={2}
          />
        </motion.svg>
        Fetch Items
      </motion.div>

      {/* Zone change notification — fixed height prevents layout shift */}
      <div className="relative mt-3 h-5">
        <AnimatePresence>
          {zoneMsg && (
            <motion.div
              animate={{ opacity: 1, y: 0 }}
              className="absolute inset-x-0 flex items-center justify-center gap-1.5 whitespace-nowrap text-cre-accent text-xs"
              exit={{ opacity: 0, y: 4 }}
              initial={{ opacity: 0, y: -4 }}
              transition={{ duration: 0.3 }}
            >
              <RefreshIcon />
              {zoneMsg}
            </motion.div>
          )}
        </AnimatePresence>
      </div>
    </div>
  );
}
