"use client";

import { motion, useInView } from "motion/react";
import Image from "next/image";
import { useEffect, useRef, useState } from "react";

// Each step in the animation sequence — simulates someone configuring their filter.
// Some steps change the item, some tweak colors/icons within the same item.
const ANIM_STEPS = [
  {
    slot: "Ring",
    name: "Ring",
    bgColor: "#6B7280",
    borderColor: "#6B7280",
    textColor: "#9ca3af",
    icon: "",
    iconSize: "--",
    iconColor: "--",
    iconShape: "--",
  },
  // User picks red BG
  {
    slot: "Ring",
    name: "Ring",
    bgColor: "#EF4444",
    borderColor: "#6B7280",
    textColor: "#9ca3af",
    icon: "",
    iconSize: "--",
    iconColor: "--",
    iconShape: "--",
  },
  // User sets white border + text
  {
    slot: "Ring",
    name: "Ring",
    bgColor: "#EF4444",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "",
    iconSize: "--",
    iconColor: "--",
    iconShape: "--",
  },
  // User picks map icon
  {
    slot: "Ring",
    name: "Ring",
    bgColor: "#EF4444",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "/images/filter-icons/circle-cyan.png",
    iconSize: "Small",
    iconColor: "Yellow",
    iconShape: "Circle",
  },
  // Switch to Amulet
  {
    slot: "Amulet",
    name: "Amulet",
    bgColor: "#6B7280",
    borderColor: "#6B7280",
    textColor: "#9ca3af",
    icon: "",
    iconSize: "--",
    iconColor: "--",
    iconShape: "--",
  },
  // Pick gold BG
  {
    slot: "Amulet",
    name: "Amulet",
    bgColor: "#D4A843",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "",
    iconSize: "--",
    iconColor: "--",
    iconShape: "--",
  },
  // Pick star icon
  {
    slot: "Amulet",
    name: "Amulet",
    bgColor: "#D4A843",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "/images/filter-icons/star-yellow.png",
    iconSize: "Small",
    iconColor: "Yellow",
    iconShape: "Star",
  },
  // Switch to Belt — quick one
  {
    slot: "Belt",
    name: "Belt",
    bgColor: "#22C55E",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "/images/filter-icons/diamond-green.png",
    iconSize: "Large",
    iconColor: "Green",
    iconShape: "Diamond",
  },
  // Switch to Boots
  {
    slot: "Boots",
    name: "Boots",
    bgColor: "#3B82F6",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "/images/filter-icons/diamond-blue.png",
    iconSize: "Large",
    iconColor: "Blue",
    iconShape: "Diamond",
  },
  // User changes icon shape to Hexagon
  {
    slot: "Boots",
    name: "Boots",
    bgColor: "#3B82F6",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "/images/filter-icons/hexagon-green.png",
    iconSize: "Large",
    iconColor: "Green",
    iconShape: "Hexagon",
  },
  // Switch to Gloves — final
  {
    slot: "Gloves",
    name: "Gloves",
    bgColor: "#06B6D4",
    borderColor: "#ffffff",
    textColor: "#ffffff",
    icon: "/images/filter-icons/diamond-yellow.png",
    iconSize: "Small",
    iconColor: "Yellow",
    iconShape: "Diamond",
  },
];

const ALL_TABS = [
  "Ring",
  "Amulet",
  "Belt",
  "Boots",
  "Gloves",
  "Helmet",
  "Body Armour",
  "Weapons",
];

// Track which slots have been fully configured (have an icon)
function getConfiguredSlots(stepIndex: number) {
  const configured = new Set<string>();
  for (let i = 0; i <= stepIndex; i++) {
    if (ANIM_STEPS[i].icon) {
      configured.add(ANIM_STEPS[i].slot);
    }
  }
  return configured;
}

// Get the final color a configured slot ended with
function getSlotColor(stepIndex: number, slot: string) {
  for (let i = stepIndex; i >= 0; i--) {
    if (ANIM_STEPS[i].slot === slot && ANIM_STEPS[i].icon) {
      return ANIM_STEPS[i].bgColor;
    }
  }
  return null;
}

export function FilterStyleDemo() {
  const containerRef = useRef<HTMLDivElement>(null);
  const isInView = useInView(containerRef, { once: true, margin: "-100px" });
  const [startAnimation, setStartAnimation] = useState(false);
  const [step, setStep] = useState(-1);

  useEffect(() => {
    if (isInView) {
      const timer = setTimeout(() => {
        setStartAnimation(true);
      }, 300);
      return () => clearTimeout(timer);
    }
  }, [isInView]);

  useEffect(() => {
    if (!startAnimation) {
      return;
    }

    const totalSteps = ANIM_STEPS.length;
    const stepDuration = 1100;
    const pauseSteps = 2; // extra ticks to pause at end before looping
    const totalTicks = totalSteps + pauseSteps;
    let tick = 0;

    setStep(0);

    const interval = setInterval(() => {
      tick = (tick + 1) % totalTicks;
      setStep(tick < totalSteps ? tick : -1);
    }, stepDuration);

    return () => {
      clearInterval(interval);
    };
  }, [startAnimation]);

  const current = step >= 0 ? ANIM_STEPS[step] : null;
  const configured = step >= 0 ? getConfiguredSlots(step) : new Set<string>();

  return (
    <div
      className="flex w-full flex-col items-center justify-center py-8"
      ref={containerRef}
    >
      <motion.div
        animate={startAnimation ? { opacity: 1, y: 0 } : { opacity: 0, y: 20 }}
        className="w-full max-w-md overflow-hidden rounded-lg border border-[#444] bg-[#1e1e1e] shadow-2xl"
        initial={{ opacity: 0, y: 20 }}
        transition={{ duration: 0.6, ease: "easeOut" }}
      >
        {/* Filter Style Editor Header */}
        <div className="bg-[#1e1e1e] px-4 pt-3 pb-2">
          <span className="font-semibold text-[#ddd] text-[13px]">
            Filter Style Editor
          </span>
        </div>

        {/* Item Category Sub-Tabs */}
        <div className="flex border-[#333] border-b bg-[#1e1e1e] px-1">
          {ALL_TABS.map((tab) => {
            const isActive = current ? tab === current.slot : tab === "Ring";
            const slotColor = getSlotColor(step, tab);
            const isConfigured = configured.has(tab);

            return (
              <motion.div
                animate={
                  isActive
                    ? { backgroundColor: "#2a2a2a" }
                    : { backgroundColor: "#1e1e1e" }
                }
                className="relative px-2 py-1.5 text-[9px]"
                initial={{ backgroundColor: "#1e1e1e" }}
                key={tab}
                transition={{ duration: 0.15 }}
              >
                <span
                  className={
                    isActive ? "font-medium text-white" : "text-[#666]"
                  }
                >
                  {tab}
                </span>
                {isConfigured && slotColor ? (
                  <motion.div
                    animate={{ scaleX: 1, opacity: 1 }}
                    className="absolute right-0 bottom-0 left-0 h-[2px]"
                    initial={{ scaleX: 0, opacity: 0 }}
                    style={{ backgroundColor: slotColor }}
                    transition={{ duration: 0.3 }}
                  />
                ) : null}
              </motion.div>
            );
          })}
        </div>

        {/* Editor Content Area */}
        <div className="relative bg-[#161616] p-4">
          {/* Controls + Preview Row */}
          <div className="relative mb-3 flex min-h-[120px] items-center justify-center pl-28">
            {/* Color Controls Panel (TX/BD/BG) */}
            <div className="absolute top-1/2 left-4 z-10 flex -translate-y-1/2 flex-col gap-1.5 rounded bg-[#1a1a1a]/90 p-2.5">
              {(
                [
                  { label: "TX", key: "textColor" },
                  { label: "BD", key: "borderColor" },
                  { label: "BG", key: "bgColor" },
                ] as const
              ).map((ctrl) => {
                const colorValue = current ? current[ctrl.key] : "#4b5563";
                return (
                  <div className="flex items-center gap-2" key={ctrl.label}>
                    <span className="w-4 font-bold text-[#999] text-[9px]">
                      {ctrl.label}
                    </span>
                    <motion.div
                      animate={{ backgroundColor: colorValue }}
                      className="h-4 w-10 border border-[#555]"
                      initial={{ backgroundColor: "#4b5563" }}
                      transition={{ duration: 0.25 }}
                    />
                    <div className="flex h-3.5 w-3.5 items-center justify-center border border-[#555] bg-[#222]">
                      <motion.span
                        animate={{
                          opacity: current ? 1 : 0.3,
                        }}
                        className="text-[8px] text-white"
                        initial={{ opacity: 0.3 }}
                        transition={{ duration: 0.15 }}
                      >
                        &#10003;
                      </motion.span>
                    </div>
                  </div>
                );
              })}
            </div>

            {/* Live Preview — centered */}
            <div className="flex items-center gap-4 py-4">
              <div className="flex items-center gap-2.5">
                {/* Minimap icon */}
                <motion.div
                  animate={
                    current?.icon
                      ? { opacity: 1, scale: 1 }
                      : { opacity: 0, scale: 0.5 }
                  }
                  className="relative h-10 w-10 shrink-0"
                  initial={{ opacity: 0, scale: 0.5 }}
                  transition={{ duration: 0.25, ease: "easeOut" }}
                >
                  {current?.icon ? (
                    <Image
                      alt={`${current.slot} icon`}
                      className="object-contain"
                      fill
                      src={current.icon}
                    />
                  ) : null}
                </motion.div>

                {/* The item label preview */}
                <motion.div
                  animate={{
                    backgroundColor: current ? current.bgColor : "#4b5563",
                    borderColor: current ? current.borderColor : "#6b7280",
                    color: current ? current.textColor : "#9ca3af",
                  }}
                  className="border-2 px-8 py-2.5 font-mono text-lg shadow-lg"
                  initial={{
                    backgroundColor: "#4b5563",
                    color: "#9ca3af",
                    borderColor: "#6b7280",
                  }}
                  transition={{ duration: 0.3, ease: "easeInOut" }}
                >
                  {current ? current.name : "Ring"}
                </motion.div>
              </div>
            </div>
          </div>

          {/* Map Icon Controls */}
          <div className="flex flex-wrap items-center justify-center gap-2 rounded bg-[#1a1a1a]/90 px-3 py-2">
            <span className="font-bold text-[#999] text-[9px]">Map Icon:</span>
            <div className="flex h-3.5 w-3.5 items-center justify-center border border-[#555] bg-[#222]">
              <motion.span
                animate={{
                  opacity: current?.icon ? 1 : 0.3,
                  color: current?.icon ? "#ffffff" : "#555555",
                }}
                className="text-[8px]"
                initial={{ opacity: 0.3, color: "#555555" }}
                transition={{ duration: 0.15 }}
              >
                &#10003;
              </motion.span>
            </div>

            {(
              [
                { label: "Size", key: "iconSize" },
                { label: "Color", key: "iconColor" },
                { label: "Shape", key: "iconShape" },
              ] as const
            ).map((dd) => {
              const value = current ? current[dd.key] : "--";
              return (
                <div
                  className="flex items-center gap-1 border border-[#444] bg-[#222] px-1.5 py-0.5"
                  key={dd.label}
                >
                  <span className="text-[#888] text-[8px]">{dd.label}:</span>
                  <motion.span
                    animate={{ opacity: value !== "--" ? 1 : 0.4 }}
                    className="min-w-[28px] font-medium text-[8px] text-white"
                    initial={{ opacity: 0.4 }}
                    key={`${dd.label}-${value}`}
                    transition={{ duration: 0.15 }}
                  >
                    {value}
                  </motion.span>
                  <svg
                    aria-hidden="true"
                    className="h-2 w-2 text-[#666]"
                    fill="currentColor"
                    viewBox="0 0 20 20"
                  >
                    <path
                      clipRule="evenodd"
                      d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
                      fillRule="evenodd"
                    />
                  </svg>
                </div>
              );
            })}
          </div>

          {/* Beam Controls Row */}
          <div className="mt-1.5 flex items-center justify-center gap-2 rounded bg-[#1a1a1a]/90 px-3 py-2">
            <span className="font-bold text-[#999] text-[9px]">Beam:</span>
            <div className="flex h-3.5 w-3.5 items-center justify-center border border-[#555] bg-[#222]">
              <span className="text-[#555] text-[8px]">&#10003;</span>
            </div>
            <div className="flex items-center gap-1 border border-[#444] bg-[#222] px-1.5 py-0.5">
              <span className="text-[#888] text-[8px]">Color:</span>
              <span className="font-medium text-[8px] text-white">Yellow</span>
              <svg
                aria-hidden="true"
                className="h-2 w-2 text-[#666]"
                fill="currentColor"
                viewBox="0 0 20 20"
              >
                <path
                  clipRule="evenodd"
                  d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z"
                  fillRule="evenodd"
                />
              </svg>
            </div>
            <span className="text-[#555] text-[8px]">Temporary</span>
          </div>
        </div>
      </motion.div>
    </div>
  );
}
