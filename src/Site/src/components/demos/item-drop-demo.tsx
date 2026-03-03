"use client";

import { motion, useInView } from "motion/react";
import { useEffect, useRef, useState } from "react";

const FADE_MS = 500;
const HOLD_MS = 2000;
const RESET_MS = 1000;
const DROP_INTERVAL = 280;

interface LootDrop {
  beam: boolean;
  beamColor: string;
  bg: string;
  cre: boolean;
  name: string;
  x: number;
  y: number;
}

// Normal drops — muted white/grey labels, lower opacity
const NORMAL: LootDrop[] = [
  {
    name: "SCROLL OF WISDOM",
    bg: "#2a2a2a",
    x: 200,
    y: 8,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "ORCHARD MAP",
    bg: "#2a2a2a",
    x: 40,
    y: 30,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "GLASS SHANK",
    bg: "#2a2a2a",
    x: 310,
    y: 50,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "JADE FLASK",
    bg: "#2a2a2a",
    x: 330,
    y: 22,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "IRON RING",
    bg: "#2a2a2a",
    x: 15,
    y: 105,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "RUSTED SWORD",
    bg: "#2a2a2a",
    x: 10,
    y: 150,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "CHAIN BELT",
    bg: "#2a2a2a",
    x: 155,
    y: 140,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "ARCANIST SLIPPERS",
    bg: "#2a2a2a",
    x: 8,
    y: 175,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "SIEGE AXE",
    bg: "#2a2a2a",
    x: 300,
    y: 192,
    cre: false,
    beam: false,
    beamColor: "",
  },
  {
    name: "HUBRIS CIRCLET",
    bg: "#2a2a2a",
    x: 340,
    y: 240,
    cre: false,
    beam: false,
    beamColor: "",
  },
];

// CRE-highlighted recipe drops — colorful with beams
const RECIPE: LootDrop[] = [
  {
    name: "MAGISTRATE CROWN",
    bg: "#D4A843",
    x: 110,
    y: 55,
    cre: true,
    beam: true,
    beamColor: "#D4A843",
  },
  {
    name: "IMBUED WAND",
    bg: "#06B6D4",
    x: 270,
    y: 125,
    cre: true,
    beam: true,
    beamColor: "#06B6D4",
  },
  {
    name: "LEATHER BELT",
    bg: "#EF4444",
    x: 145,
    y: 210,
    cre: true,
    beam: true,
    beamColor: "#EF4444",
  },
];

// Interleave: mostly normal drops with recipe drops mixed in
const DROPS: LootDrop[] = [
  NORMAL[0],
  NORMAL[1],
  NORMAL[2],
  RECIPE[0],
  NORMAL[3],
  NORMAL[4],
  NORMAL[5],
  RECIPE[1],
  NORMAL[6],
  NORMAL[7],
  NORMAL[8],
  NORMAL[9],
  RECIPE[2],
];

export function ItemDropDemo() {
  const ref = useRef<HTMLDivElement>(null);
  const inView = useInView(ref, { once: true, margin: "-100px" });
  const [started, setStarted] = useState(false);
  const [visibleCount, setVisibleCount] = useState(0);
  const [fading, setFading] = useState(false);
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
    let off = false;

    function loop() {
      if (off) {
        return;
      }
      setVisibleCount(0);
      setFading(false);

      let i = 0;
      function dropNext() {
        if (off) {
          return;
        }
        i++;
        setVisibleCount(i);

        if (i < DROPS.length) {
          setTimeout(dropNext, DROP_INTERVAL);
        } else {
          // All dropped — hold, then fade, then reset
          setTimeout(() => {
            if (off) {
              return;
            }
            setFading(true);
            setTimeout(() => {
              if (off) {
                return;
              }
              setCycle((c) => c + 1);
              setTimeout(() => {
                if (off) {
                  return;
                }
                loop();
              }, RESET_MS);
            }, FADE_MS);
          }, HOLD_MS);
        }
      }

      dropNext();
    }

    loop();
    return () => {
      off = true;
    };
  }, [started]);

  return (
    <div
      className="flex w-full flex-col items-center justify-center py-4 md:py-8"
      ref={ref}
    >
      <motion.div
        animate={started ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
        className="w-full max-w-[460px] overflow-hidden rounded-xl border border-cre-border shadow-2xl"
        initial={{ opacity: 0, y: 20 }}
        transition={{ duration: 0.6, ease: "easeOut" }}
        style={{ aspectRatio: '460 / 280' }}
      >
        {/* Dark game viewport — fixed 460×280, CSS-scaled to fit container */}
        <div
          className="relative h-full w-full origin-top-left"
          style={{
            background:
              "radial-gradient(ellipse at 50% 60%, #1a1a2e 0%, #0d0d14 60%, #07070a 100%)",
          }}
        >
          {/* Subtle ground gradient */}
          <div className="pointer-events-none absolute inset-x-0 bottom-0 h-20 bg-gradient-to-t from-black/30 to-transparent" />

          {/* Loot drops */}
          <div
            className="transition-opacity"
            style={{
              opacity: fading ? 0 : 1,
              transitionDuration: `${FADE_MS}ms`,
            }}
          >
            {DROPS.map((drop, i) => {
              if (i >= visibleCount) {
                return null;
              }

              return (
                <motion.div
                  animate={{ y: 0, opacity: 1 }}
                  className={`absolute ${drop.cre ? "z-10" : "z-0"}`}
                  initial={{ y: -25, opacity: 0 }}
                  // biome-ignore lint/suspicious/noArrayIndexKey: Interleaved drop sequence
                  key={`${drop.name}-${i}-${cycle}`}
                  style={{ left: `${(drop.x / 460) * 100}%`, top: `${(drop.y / 280) * 100}%` }}
                  transition={{
                    duration: 0.3,
                    ease: [0.2, 0.8, 0.3, 1],
                  }}
                >
                  {/* Light beam — CRE recipe items only */}
                  {drop.beam && (
                    <motion.div
                      animate={{ scaleY: 1, opacity: 1 }}
                      className="pointer-events-none absolute bottom-full left-1/2 origin-bottom -translate-x-1/2"
                      initial={{ scaleY: 0, opacity: 0 }}
                      style={{
                        width: 4,
                        height: 140,
                        background: `linear-gradient(to top, ${drop.beamColor}, ${drop.beamColor}60 30%, transparent)`,
                        boxShadow: `0 0 10px ${drop.beamColor}40, 0 0 20px ${drop.beamColor}20`,
                      }}
                      transition={{
                        delay: 0.12,
                        duration: 0.5,
                        ease: "easeOut",
                      }}
                    />
                  )}

                  {/* Item label */}
                  <div
                    className="flex items-center whitespace-nowrap border px-2 py-0.5 font-bold text-[11px] uppercase tracking-wider"
                    style={{
                      backgroundColor: drop.bg,
                      borderColor: drop.cre
                        ? "rgba(255,255,255,0.45)"
                        : "rgba(255,255,255,0.15)",
                      color: drop.cre ? "#ffffff" : "rgba(255,255,255,0.4)",
                      textShadow: drop.cre
                        ? "0 1px 3px rgba(0,0,0,0.8)"
                        : "none",
                      opacity: drop.cre ? 1 : 0.55,
                    }}
                  >
                    <span>{drop.name}</span>
                  </div>
                </motion.div>
              );
            })}
          </div>
        </div>
      </motion.div>
    </div>
  );
}
