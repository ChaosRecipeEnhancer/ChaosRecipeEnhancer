"use client";

import { motion, useInView } from "motion/react";
import { useEffect, useRef, useState } from "react";

// ---------------------------------------------------------------------------
// Scene 1 — Loot rain: every item looks identical, recipe pieces buried
// x/y are percentages so drops scale with any viewport size
// ---------------------------------------------------------------------------

const DROPS = [
  { n: "SCROLL OF WISDOM", x: 4, y: 2 },
  { n: "JADE FLASK", x: 62, y: 4 },
  { n: "ORCHARD MAP", x: 32, y: 8 },
  { n: "GLASS SHANK", x: 78, y: 1 },
  { n: "MAGISTRATE CROWN", x: 14, y: 13 },
  { n: "IRON RING", x: 50, y: 17 },
  { n: "QUICKSILVER FLASK", x: 6, y: 21 },
  { n: "GOLD AMULET", x: 68, y: 11 },
  { n: "ASSASSIN'S GARB", x: 38, y: 25 },
  { n: "LEATHER CAP", x: 82, y: 7 },
  { n: "SORCERER GLOVES", x: 54, y: 28 },
  { n: "STONE HAMMER", x: 22, y: 33 },
  { n: "MURDER BOOTS", x: 72, y: 18 },
  { n: "CORAL RING", x: 42, y: 37 },
  { n: "ONYX AMULET", x: 10, y: 41 },
  { n: "DUSK BLADE", x: 58, y: 44 },
  { n: "GEMINI CLAW", x: 18, y: 48 },
  { n: "STUDDED BELT", x: 74, y: 33 },
  { n: "WOOL SHOES", x: 30, y: 52 },
  { n: "VAAL REGALIA", x: 64, y: 56 },
  { n: "SULPHUR FLASK", x: 5, y: 60 },
  { n: "CHAIN BELT", x: 76, y: 42 },
  { n: "SIEGE AXE", x: 24, y: 64 },
  { n: "PRISMATIC RING", x: 48, y: 68 },
  // More drops — heavier on the right side
  { n: "TOPAZ RING", x: 85, y: 15 },
  { n: "GRANITE FLASK", x: 80, y: 24 },
  { n: "SACRIFICIAL GARB", x: 70, y: 48 },
  { n: "VOID SCEPTRE", x: 88, y: 36 },
  { n: "RUBY RING", x: 84, y: 52 },
  { n: "TITAN GREAVES", x: 60, y: 72 },
  { n: "GOLDEN BUCKLER", x: 76, y: 60 },
  { n: "AMETHYST FLASK", x: 2, y: 72 },
  { n: "HUBRIS CIRCLET", x: 40, y: 76 },
  { n: "IMBUED WAND", x: 82, y: 68 },
  { n: "CARNAL ARMOUR", x: 16, y: 78 },
  { n: "SAPPHIRE RING", x: 90, y: 44 },
  { n: "EZOMYTE TOWER SHIELD", x: 52, y: 80 },
  { n: "DEICIDE AXE", x: 78, y: 76 },
  { n: "SATIN GLOVES", x: 8, y: 82 },
  { n: "AGATE AMULET", x: 66, y: 84 },
];

// ---------------------------------------------------------------------------
// Scene 2 — Stash confusion: packed tab, cursor searching aimlessly
// ---------------------------------------------------------------------------

const SC = 24;
const SG = 1;
const SP = 8;
const SCOLS = 12;
const SROWS = 8;

interface StashBlock {
  c: number;
  h: number;
  r: number;
  w: number;
}

// 21 items filling ~76% of cells. All generic amber — no CRE highlighting.
const STASH: StashBlock[] = [
  { c: 0, r: 0, w: 2, h: 3 },
  { c: 2, r: 0, w: 1, h: 2 },
  { c: 3, r: 0, w: 2, h: 2 },
  { c: 6, r: 0, w: 2, h: 3 },
  { c: 9, r: 0, w: 1, h: 3 },
  { c: 10, r: 0, w: 2, h: 2 },
  { c: 3, r: 2, w: 2, h: 1 },
  { c: 5, r: 2, w: 1, h: 2 },
  { c: 0, r: 3, w: 2, h: 2 },
  { c: 2, r: 3, w: 1, h: 2 },
  { c: 3, r: 3, w: 2, h: 2 },
  { c: 6, r: 3, w: 2, h: 2 },
  { c: 8, r: 3, w: 2, h: 2 },
  { c: 10, r: 3, w: 2, h: 2 },
  { c: 0, r: 5, w: 1, h: 3 },
  { c: 1, r: 5, w: 2, h: 2 },
  { c: 5, r: 5, w: 1, h: 3 },
  { c: 6, r: 5, w: 2, h: 2 },
  { c: 8, r: 5, w: 2, h: 3 },
  { c: 10, r: 5, w: 1, h: 2 },
  { c: 3, r: 7, w: 2, h: 1 },
];

const CURSOR_PATH = [2, 13, 6, 18, 9, 15];

const GRID_KEYS = Array.from({ length: SCOLS * SROWS }, (_, i) => `c${i}`);

// ---------------------------------------------------------------------------
// Timing
// ---------------------------------------------------------------------------

const DROP_MS = 100;
const DROP_HOLD = 1200;
const CURSOR_MS = 700;
const CURSOR_WAIT = 500;
const END_HOLD = 1200;
const JIGGLE_MS = 80;
const JIGGLE_OFFSETS = [
  { x: 8, y: -6 },
  { x: -10, y: 4 },
  { x: 6, y: 8 },
  { x: -8, y: -10 },
  { x: 12, y: 2 },
  { x: -6, y: 10 },
  { x: 10, y: -4 },
  { x: -12, y: -8 },
  { x: 4, y: 6 },
  { x: -8, y: -4 },
];

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

function stashCenter(item: StashBlock) {
  return {
    x: SP + item.c * (SC + SG) + (item.w * SC + (item.w - 1) * SG) / 2,
    y: SP + item.r * (SC + SG) + (item.h * SC + (item.h - 1) * SG) / 2,
  };
}

// ---------------------------------------------------------------------------
// Component
// ---------------------------------------------------------------------------

export function ProblemStatementDemo() {
  const ref = useRef<HTMLDivElement>(null);
  const inView = useInView(ref, { margin: "-60px", once: true });
  const [scene, setScene] = useState<"drops" | "stash">("drops");
  const [dropCount, setDropCount] = useState(0);
  const [cursorIdx, setCursorIdx] = useState(-1);
  const [cycle, setCycle] = useState(0);
  const [jiggleOffset, setJiggleOffset] = useState({ x: 0, y: 0 });
  const [isJiggling, setIsJiggling] = useState(false);

  useEffect(() => {
    if (!inView) {
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
      setCycle((c) => c + 1);
      setScene("drops");
      setDropCount(0);
      setCursorIdx(-1);
      setJiggleOffset({ x: 0, y: 0 });
      setIsJiggling(false);

      for (let i = 0; i < DROPS.length; i++) {
        after(() => setDropCount((prev) => prev + 1), i * DROP_MS);
      }

      const dropsDone = DROPS.length * DROP_MS;
      after(() => setScene("stash"), dropsDone + DROP_HOLD);

      const cursorStart = dropsDone + DROP_HOLD + CURSOR_WAIT;
      for (let i = 0; i < CURSOR_PATH.length; i++) {
        const idx = i;
        after(() => setCursorIdx(idx), cursorStart + i * CURSOR_MS);
      }

      const jiggleStart = cursorStart + CURSOR_PATH.length * CURSOR_MS + 200;
      after(() => setIsJiggling(true), jiggleStart);
      for (let i = 0; i < JIGGLE_OFFSETS.length; i++) {
        const offset = JIGGLE_OFFSETS[i];
        after(() => setJiggleOffset(offset), jiggleStart + i * JIGGLE_MS);
      }
      const jiggleDone = jiggleStart + JIGGLE_OFFSETS.length * JIGGLE_MS;
      after(() => {
        setJiggleOffset({ x: 0, y: 0 });
        setIsJiggling(false);
      }, jiggleDone);

      const cycleDone = jiggleDone + END_HOLD;
      after(() => runCycle(), cycleDone);
    }

    runCycle();

    return () => {
      active = false;
      clearTimers();
    };
  }, [inView]);

  const gw = SCOLS * SC + (SCOLS - 1) * SG;
  const gh = SROWS * SC + (SROWS - 1) * SG;

  const cursorTarget =
    cursorIdx >= 0 && cursorIdx < CURSOR_PATH.length
      ? stashCenter(STASH[CURSOR_PATH[cursorIdx]])
      : { x: (gw + SP * 2) / 2, y: (gh + SP * 2) / 2 };

  return (
    <div className="relative w-full" ref={ref}>
      {/* Full-width animation viewport */}
      <motion.div
        animate={inView ? { opacity: 1 } : { opacity: 0 }}
        className="relative h-[50vh] max-h-[600px] min-h-[280px] w-full overflow-hidden md:h-[70vh] md:min-h-[380px]"
        initial={{ opacity: 0 }}
        transition={{ duration: 0.8, ease: "easeOut" }}
      >
        {/* ---- Scene 1: Loot rain ---- */}
        <motion.div
          animate={{ opacity: scene === "drops" ? 1 : 0 }}
          className="absolute inset-0"
          transition={{ duration: 0.5 }}
        >
          <div
            className="absolute inset-0"
            style={{
              background:
                "radial-gradient(ellipse at 50% 60%, #1a1a2e 0%, #0d0d14 60%, #07070a 100%)",
            }}
          />
          <div className="pointer-events-none absolute inset-x-0 bottom-0 h-32 bg-gradient-to-t from-black/40 to-transparent" />

          {DROPS.map(
            (drop, i) =>
              i < dropCount && (
                <motion.div
                  animate={{ opacity: 1, y: 0 }}
                  className="absolute whitespace-nowrap"
                  initial={{ opacity: 0, y: -25 }}
                  key={`${drop.n}-${cycle}`}
                  style={{ left: `${drop.x}%`, top: `${drop.y}%` }}
                  transition={{
                    duration: 0.25,
                    ease: [0.2, 0.8, 0.3, 1],
                  }}
                >
                  <div
                    className="border px-1.5 py-0.5 font-bold text-[11px] uppercase tracking-wider"
                    style={{
                      backgroundColor: "#2a2a2a",
                      borderColor: "rgba(255,255,255,0.12)",
                      color: "rgba(255,255,255,0.35)",
                    }}
                  >
                    {drop.n}
                  </div>
                </motion.div>
              )
          )}
        </motion.div>

        {/* ---- Scene 2: Stash confusion ---- */}
        <motion.div
          animate={{ opacity: scene === "stash" ? 1 : 0 }}
          className="absolute inset-0 flex items-center justify-center bg-[#0a0a0a]"
          transition={{ duration: 0.5 }}
        >
          {/* Grid — centered */}
          <div className="flex items-center justify-center">
            <div
              className="relative"
              style={{
                width: gw + SP * 2,
                height: gh + SP * 2,
                padding: SP,
              }}
            >
              <div
                className="grid"
                style={{
                  gridTemplateColumns: `repeat(${SCOLS}, ${SC}px)`,
                  gridTemplateRows: `repeat(${SROWS}, ${SC}px)`,
                  gap: `${SG}px`,
                }}
              >
                {GRID_KEYS.map((k) => (
                  <div
                    className="border border-[#1a1a1a] bg-[#111111]"
                    key={k}
                  />
                ))}
              </div>

              {/* Stash items — all generic amber */}
              {STASH.map((item, idx) => {
                const left = SP + item.c * (SC + SG);
                const top = SP + item.r * (SC + SG);
                const w = item.w * SC + (item.w - 1) * SG;
                const h = item.h * SC + (item.h - 1) * SG;

                return (
                  <div
                    className="absolute rounded-sm"
                    // biome-ignore lint/suspicious/noArrayIndexKey: Static layout
                    key={idx}
                    style={{
                      left,
                      top,
                      width: w,
                      height: h,
                      backgroundColor: "rgba(140, 110, 40, 0.12)",
                      border: "1px solid rgba(180, 140, 50, 0.25)",
                      boxShadow: "inset 0 0 8px rgba(180, 140, 50, 0.06)",
                    }}
                  />
                );
              })}

              {/* Wandering cursor */}
              <motion.div
                animate={{
                  left: cursorTarget.x + jiggleOffset.x,
                  opacity: scene === "stash" && cursorIdx >= 0 ? 1 : 0,
                  top: cursorTarget.y + jiggleOffset.y,
                }}
                className="pointer-events-none absolute z-20"
                transition={
                  isJiggling
                    ? {
                        left: {
                          type: "spring",
                          stiffness: 800,
                          damping: 15,
                        },
                        opacity: { duration: 0.15 },
                        top: {
                          type: "spring",
                          stiffness: 800,
                          damping: 15,
                        },
                      }
                    : {
                        left: {
                          type: "spring",
                          stiffness: 120,
                          damping: 25,
                        },
                        opacity: { duration: 0.15 },
                        top: {
                          type: "spring",
                          stiffness: 120,
                          damping: 25,
                        },
                      }
                }
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
            </div>
          </div>
        </motion.div>

        {/* Top gradient fade into page bg */}
        <div className="pointer-events-none absolute inset-x-0 top-0 z-10 h-20 bg-gradient-to-b from-cre-bg-deepest to-transparent md:h-32" />

        {/* Bottom gradient fade into page bg */}
        <div className="pointer-events-none absolute inset-x-0 bottom-0 z-10 h-24 bg-gradient-to-t from-cre-bg-deepest to-transparent md:h-40" />
      </motion.div>

      {/* Problem text — below the animation, not overlaid */}
      <div className="mx-auto max-w-lg px-6 pt-6 pb-12 text-center md:pt-10 md:pb-24">
        <p className="text-balance text-cre-text-secondary text-lg leading-relaxed">
          Every zone drops items that all look the same. Recipe pieces hide in
          the noise — and finding them in a packed stash tab is a guessing game.
        </p>
      </div>
    </div>
  );
}
