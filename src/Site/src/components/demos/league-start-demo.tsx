"use client";

import { motion, useInView } from "motion/react";
import { useRef } from "react";

// ---------------------------------------------------------------------------
// Chart data — two curves over first ~2 weeks of a league
//   "without": mapping alone, no recipe farming — slow linear-ish grind
//   "with":    mapping + chaos recipe via CRE — steep early gains
// x = day (0–14), y = cumulative divine equivalent (0–1 normalized)
// ---------------------------------------------------------------------------

interface Point {
  x: number;
  y: number;
}

const WITHOUT_CRE: Point[] = [
  { x: 0, y: 0 },
  { x: 1, y: 0.01 },
  { x: 2, y: 0.03 },
  { x: 3, y: 0.06 },
  { x: 4, y: 0.1 },
  { x: 5, y: 0.15 },
  { x: 6, y: 0.2 },
  { x: 7, y: 0.26 },
  { x: 8, y: 0.32 },
  { x: 9, y: 0.38 },
  { x: 10, y: 0.44 },
  { x: 11, y: 0.5 },
  { x: 12, y: 0.56 },
  { x: 13, y: 0.62 },
  { x: 14, y: 0.68 },
];

const WITH_CRE: Point[] = [
  { x: 0, y: 0 },
  { x: 1, y: 0.04 },
  { x: 2, y: 0.12 },
  { x: 3, y: 0.22 },
  { x: 4, y: 0.35 },
  { x: 5, y: 0.48 },
  { x: 6, y: 0.58 },
  { x: 7, y: 0.67 },
  { x: 8, y: 0.74 },
  { x: 9, y: 0.8 },
  { x: 10, y: 0.85 },
  { x: 11, y: 0.89 },
  { x: 12, y: 0.92 },
  { x: 13, y: 0.95 },
  { x: 14, y: 0.97 },
];

// Phase labels on the x-axis
const PHASES = [
  { label: "Acts", start: 0, end: 2 },
  { label: "White Maps", start: 2, end: 5 },
  { label: "Yellow Maps", start: 5, end: 9 },
  { label: "Red Maps", start: 9, end: 14 },
];

// Milestones on the "with CRE" curve
const MILESTONES = [
  { day: 3, label: "First Divine", y: 0.22, color: "#D4A843" },
  { day: 6, label: "5+ Divines", y: 0.58, color: "#D4A843" },
  { day: 9, label: "Switch to endgame", y: 0.8, color: "#3494E1" },
];

// ---------------------------------------------------------------------------
// SVG chart dimensions
// ---------------------------------------------------------------------------

const W = 600;
const H = 300;
const PAD_L = 48;
const PAD_R = 20;
const PAD_T = 30;
const PAD_B = 60;
const CHART_W = W - PAD_L - PAD_R;
const CHART_H = H - PAD_T - PAD_B;
const MAX_DAY = 14;

function toSvg(pt: Point) {
  return {
    sx: PAD_L + (pt.x / MAX_DAY) * CHART_W,
    sy: PAD_T + (1 - pt.y) * CHART_H,
  };
}

function buildLinePath(points: Point[]): string {
  return points
    .map((pt, i) => {
      const { sx, sy } = toSvg(pt);
      return `${i === 0 ? "M" : "L"}${sx},${sy}`;
    })
    .join(" ");
}

function buildAreaPath(points: Point[]): string {
  const line = buildLinePath(points);
  const last = points.at(-1);
  const first = points[0];
  if (!(last && first)) {
    return "";
  }
  const lastSvg = toSvg(last);
  const firstSvg = toSvg(first);
  return `${line} L${lastSvg.sx},${PAD_T + CHART_H} L${firstSvg.sx},${PAD_T + CHART_H} Z`;
}

// ---------------------------------------------------------------------------
// Component
// ---------------------------------------------------------------------------

export function LeagueStartDemo() {
  const ref = useRef<HTMLDivElement>(null);
  const inView = useInView(ref, { margin: "-80px", once: true });

  const withLine = buildLinePath(WITH_CRE);
  const withArea = buildAreaPath(WITH_CRE);
  const withoutLine = buildLinePath(WITHOUT_CRE);
  const withoutArea = buildAreaPath(WITHOUT_CRE);

  const phaseLines = PHASES.slice(1).map((p) => ({
    x: PAD_L + (p.start / MAX_DAY) * CHART_W,
    label: p.label,
  }));

  return (
    <div
      className="relative mx-auto w-full max-w-2xl overflow-hidden rounded-xl border border-cre-border bg-cre-bg-deep p-6"
      ref={ref}
    >
      {/* Legend */}
      <motion.div
        animate={inView ? { opacity: 1, y: 0 } : { opacity: 0, y: -10 }}
        className="mb-4 flex flex-wrap items-center gap-x-5 gap-y-2"
        initial={{ opacity: 0, y: -10 }}
        transition={{ duration: 0.5, delay: 0.2 }}
      >
        <div className="flex items-center gap-2">
          <span className="relative flex h-2.5 w-2.5">
            <span className="absolute inline-flex h-full w-full animate-ping rounded-full bg-green-400 opacity-75" />
            <span className="relative inline-flex h-2.5 w-2.5 rounded-full bg-green-500" />
          </span>
          <span className="font-medium text-green-400 text-sm tracking-wide">
            Earning while you map
          </span>
        </div>
        <div className="flex items-center gap-4 text-xs">
          <span className="flex items-center gap-1.5">
            <span className="inline-block h-0.5 w-4 rounded-full bg-cre-gold" />
            <span className="text-cre-text-secondary">With CRE</span>
          </span>
          <span className="flex items-center gap-1.5">
            <span className="inline-block h-0.5 w-4 rounded-full bg-[#555]" />
            <span className="text-cre-text-muted">Without</span>
          </span>
        </div>
      </motion.div>

      <svg
        aria-label="Chart comparing currency gains with and without Chaos Recipe Enhancer during league start"
        className="w-full"
        role="img"
        viewBox={`0 0 ${W} ${H}`}
        xmlns="http://www.w3.org/2000/svg"
      >
        <defs>
          {/* Gold area fill — with CRE */}
          <linearGradient id="areaGradWith" x1="0" x2="0" y1="0" y2="1">
            <stop offset="0%" stopColor="#D4A843" stopOpacity="0.25" />
            <stop offset="100%" stopColor="#D4A843" stopOpacity="0.02" />
          </linearGradient>
          {/* Grey area fill — without CRE */}
          <linearGradient id="areaGradWithout" x1="0" x2="0" y1="0" y2="1">
            <stop offset="0%" stopColor="#555555" stopOpacity="0.12" />
            <stop offset="100%" stopColor="#555555" stopOpacity="0.01" />
          </linearGradient>
          {/* Clip path for reveal animation */}
          <clipPath id="chartReveal">
            <motion.rect
              animate={inView ? { width: W } : { width: 0 }}
              height={H}
              initial={{ width: 0 }}
              transition={{ duration: 2.5, ease: "easeOut", delay: 0.4 }}
              x={0}
              y={0}
            />
          </clipPath>
          {/* "Done" zone gradient */}
          <linearGradient id="doneZone" x1="0" x2="0" y1="0" y2="1">
            <stop offset="0%" stopColor="#3494E1" stopOpacity="0.08" />
            <stop offset="100%" stopColor="#3494E1" stopOpacity="0.02" />
          </linearGradient>
        </defs>

        {/* Grid lines */}
        {[0.25, 0.5, 0.75, 1].map((v) => {
          const y = PAD_T + (1 - v) * CHART_H;
          return (
            <line
              key={v}
              stroke="#313131"
              strokeDasharray="4 4"
              x1={PAD_L}
              x2={PAD_L + CHART_W}
              y1={y}
              y2={y}
            />
          );
        })}

        {/* Phase backgrounds — subtle alternating */}
        {PHASES.map((phase, i) => {
          const x1 = PAD_L + (phase.start / MAX_DAY) * CHART_W;
          const x2 = PAD_L + (phase.end / MAX_DAY) * CHART_W;
          return (
            // biome-ignore lint/suspicious/noArrayIndexKey: Static layout
            <g key={i}>
              {i % 2 === 1 && (
                <rect
                  fill="rgba(255,255,255,0.015)"
                  height={CHART_H}
                  width={x2 - x1}
                  x={x1}
                  y={PAD_T}
                />
              )}
              <text
                className="fill-cre-text-muted text-[10px]"
                textAnchor="middle"
                x={(x1 + x2) / 2}
                y={PAD_T + CHART_H + 20}
              >
                {phase.label}
              </text>
            </g>
          );
        })}

        {/* Phase divider lines */}
        {phaseLines.map((pl) => (
          <line
            key={pl.label}
            stroke="#313131"
            strokeDasharray="2 3"
            x1={pl.x}
            x2={pl.x}
            y1={PAD_T}
            y2={PAD_T + CHART_H}
          />
        ))}

        {/* "You're done" zone overlay */}
        <motion.rect
          animate={inView ? { opacity: 1 } : { opacity: 0 }}
          fill="url(#doneZone)"
          height={CHART_H}
          initial={{ opacity: 0 }}
          transition={{ duration: 0.8, delay: 2.2 }}
          width={(5 / MAX_DAY) * CHART_W}
          x={PAD_L + (9 / MAX_DAY) * CHART_W}
          y={PAD_T}
        />

        {/* Curves — clipped by reveal animation */}
        <g clipPath="url(#chartReveal)">
          {/* Without CRE — grey, behind */}
          <path d={withoutArea} fill="url(#areaGradWithout)" />
          <path
            d={withoutLine}
            fill="none"
            stroke="#555555"
            strokeDasharray="6 4"
            strokeLinecap="round"
            strokeWidth={1.5}
          />

          {/* With CRE — gold, in front */}
          <path d={withArea} fill="url(#areaGradWith)" />
          <path
            d={withLine}
            fill="none"
            stroke="#D4A843"
            strokeLinecap="round"
            strokeWidth={2.5}
          />
        </g>

        {/* Gap annotation — appears mid-chart showing the difference */}
        <motion.g
          animate={inView ? { opacity: 1 } : { opacity: 0 }}
          initial={{ opacity: 0 }}
          transition={{ duration: 0.5, delay: 1.6 }}
        >
          {(() => {
            const day = 7;
            const withPt = toSvg(WITH_CRE[day]);
            const withoutPt = toSvg(WITHOUT_CRE[day]);
            return (
              <>
                {/* Vertical bracket line */}
                <line
                  stroke="#D4A843"
                  strokeDasharray="3 2"
                  strokeOpacity={0.5}
                  x1={withPt.sx + 8}
                  x2={withPt.sx + 8}
                  y1={withPt.sy + 4}
                  y2={withoutPt.sy - 4}
                />
              </>
            );
          })()}
        </motion.g>

        {/* Y-axis label */}
        <text
          className="fill-cre-text-muted text-[10px]"
          textAnchor="middle"
          transform={`rotate(-90, 14, ${PAD_T + CHART_H / 2})`}
          x={14}
          y={PAD_T + CHART_H / 2}
        >
          Currency (Divines)
        </text>

        {/* Milestone dots + labels on the "with CRE" curve */}
        {MILESTONES.map((ms) => {
          const { sx, sy } = toSvg({ x: ms.day, y: ms.y });
          const delay = 0.4 + (ms.day / MAX_DAY) * 2.5;
          return (
            <motion.g
              animate={inView ? { opacity: 1 } : { opacity: 0 }}
              initial={{ opacity: 0 }}
              key={ms.label}
              transition={{ duration: 0.4, delay }}
            >
              <circle cx={sx} cy={sy} fill={ms.color} r={4} />
              <circle
                cx={sx}
                cy={sy}
                fill="none"
                r={7}
                stroke={ms.color}
                strokeOpacity={0.4}
                strokeWidth={1.5}
              />
              <text
                className="fill-cre-text font-medium text-[11px]"
                textAnchor="end"
                x={sx - 12}
                y={sy - 10}
              >
                {ms.label}
              </text>
            </motion.g>
          );
        })}

        {/* "Focus on endgame" annotation */}
        <motion.g
          animate={inView ? { opacity: 1 } : { opacity: 0 }}
          initial={{ opacity: 0 }}
          transition={{ duration: 0.6, delay: 2.6 }}
        >
          <text
            className="fill-cre-accent font-medium text-[10px]"
            textAnchor="middle"
            x={PAD_L + (11.5 / MAX_DAY) * CHART_W}
            y={PAD_T + CHART_H + 42}
          >
            Focus on endgame
          </text>
        </motion.g>

        {/* X-axis line */}
        <line
          stroke="#313131"
          x1={PAD_L}
          x2={PAD_L + CHART_W}
          y1={PAD_T + CHART_H}
          y2={PAD_T + CHART_H}
        />
      </svg>

      {/* Bottom callout */}
      <motion.p
        animate={inView ? { opacity: 1, y: 0 } : { opacity: 0, y: 10 }}
        className="mt-4 text-center text-cre-text-muted text-sm"
        initial={{ opacity: 0, y: 10 }}
        transition={{ duration: 0.5, delay: 2.8 }}
      >
        CRE runs in the background — no extra clicks while you play.
      </motion.p>
    </div>
  );
}
