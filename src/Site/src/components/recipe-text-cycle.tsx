"use client";

import { AnimatePresence, motion } from "motion/react";
import Link from "next/link";
import { useEffect, useState } from "react";

type UnderlineStyle = "solid" | "dashed" | "wavy" | "dotted";

const RECIPES = [
  {
    name: "Chaos",
    color: "#D4A843",
    href: "https://www.poewiki.net/wiki/Chaos_Orb",
    underline: "solid" as UnderlineStyle,
  },
  {
    name: "Chance",
    color: "#22C55E",
    href: "https://www.poewiki.net/wiki/Vendor_recipe_system#Multiple_Rare_Items",
    underline: "dashed" as UnderlineStyle,
  },
  {
    name: "Regal",
    color: "#60A5FA",
    href: "https://www.poewiki.net/wiki/Vendor_recipe_system#Multiple_Rare_Items",
    underline: "wavy" as UnderlineStyle,
  },
  {
    name: "Exalted",
    color: "#FBBF24",
    href: "https://www.poewiki.net/wiki/Vendor_recipe_system#Multiple_Rare_Items",
    underline: "dotted" as UnderlineStyle,
  },
];

function Underline({ color, style }: { color: string; style: UnderlineStyle }) {
  const glow = `0 0 8px ${color}66`;

  if (style === "wavy") {
    return (
      <motion.svg
        animate={{ scaleX: 1 }}
        aria-hidden="true"
        className="absolute -bottom-1 left-0 w-full origin-left"
        height="6"
        initial={{ scaleX: 0 }}
        preserveAspectRatio="none"
        transition={{ delay: 0.15, duration: 0.4, ease: "easeOut" }}
        viewBox="0 0 120 6"
      >
        <path
          d="M0 3 Q5 0 10 3 T20 3 T30 3 T40 3 T50 3 T60 3 T70 3 T80 3 T90 3 T100 3 T110 3 T120 3"
          fill="none"
          stroke={color}
          strokeWidth="2"
          style={{ filter: `drop-shadow(${glow})` }}
        />
      </motion.svg>
    );
  }

  if (style === "dashed") {
    return (
      <motion.span
        animate={{ scaleX: 1 }}
        className="absolute -bottom-0.5 left-0 h-[2px] w-full origin-left"
        initial={{ scaleX: 0 }}
        style={{
          backgroundImage: `repeating-linear-gradient(to right, ${color} 0, ${color} 6px, transparent 6px, transparent 12px)`,
          filter: `drop-shadow(${glow})`,
        }}
        transition={{ delay: 0.15, duration: 0.4, ease: "easeOut" }}
      />
    );
  }

  if (style === "dotted") {
    return (
      <motion.span
        animate={{ scaleX: 1 }}
        className="absolute -bottom-0.5 left-0 h-[2px] w-full origin-left"
        initial={{ scaleX: 0 }}
        style={{
          backgroundImage: `radial-gradient(circle, ${color} 1px, transparent 1px)`,
          backgroundSize: "6px 2px",
          filter: `drop-shadow(${glow})`,
        }}
        transition={{ delay: 0.15, duration: 0.4, ease: "easeOut" }}
      />
    );
  }

  // solid (default)
  return (
    <motion.span
      animate={{ scaleX: 1 }}
      className="absolute -bottom-0.5 left-0 h-[2px] w-full origin-left rounded-full"
      initial={{ scaleX: 0 }}
      style={{
        backgroundColor: color,
        boxShadow: glow,
      }}
      transition={{ delay: 0.15, duration: 0.4, ease: "easeOut" }}
    />
  );
}

export function RecipeTextCycle() {
  const [index, setIndex] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setIndex((prev) => (prev + 1) % RECIPES.length);
    }, 2500);
    return () => clearInterval(interval);
  }, []);

  const recipe = RECIPES[index];

  return (
    <span className="relative inline-block text-center align-baseline">
      {/* Invisible sizer — widest word prevents layout shift */}
      <span aria-hidden="true" className="invisible select-none">
        Exalted
      </span>

      <AnimatePresence mode="wait">
        <motion.span
          animate={{ opacity: 1, y: 0, filter: "blur(0px)" }}
          className="absolute inset-x-0 top-0"
          exit={{ opacity: 0, y: -10, filter: "blur(4px)" }}
          initial={{ opacity: 0, y: 10, filter: "blur(4px)" }}
          key={recipe.name}
          transition={{ duration: 0.3, ease: [0.25, 0.46, 0.45, 0.94] }}
        >
          <Link
            className="relative transition-opacity hover:opacity-80"
            href={recipe.href}
            rel="noopener noreferrer"
            style={{ color: recipe.color }}
            target="_blank"
          >
            {recipe.name}
            <Underline color={recipe.color} style={recipe.underline} />
          </Link>
        </motion.span>
      </AnimatePresence>
    </span>
  );
}
