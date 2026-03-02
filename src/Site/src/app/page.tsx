import Image from "next/image";
import Link from "next/link";
import { FilterStyleDemo } from "@/components/demos/filter-style-demo";
import { ItemDropDemo } from "@/components/demos/item-drop-demo";
import { LeagueStartDemo } from "@/components/demos/league-start-demo";
import { ProblemStatementDemo } from "@/components/demos/problem-statement-demo";
import { RecipeTrackerDemo } from "@/components/demos/recipe-tracker-demo";
import { StashHighlightDemo } from "@/components/demos/stash-highlight-demo";
import { FadeIn } from "@/components/fade-in";
import { RecipeTextCycle } from "@/components/recipe-text-cycle";
import { StaggerChildren, StaggerItem } from "@/components/stagger-children";

const TRAILING_ZERO = /\.0$/;

const GITHUB_API =
  "https://api.github.com/repos/ChaosRecipeEnhancer/ChaosRecipeEnhancer";
const GITHUB_RELEASES_API = `${GITHUB_API}/releases?per_page=100`;

async function getDownloadCount(): Promise<number | null> {
  try {
    const res = await fetch(GITHUB_RELEASES_API, {
      next: { revalidate: 3600 },
    });
    if (!res.ok) {
      return null;
    }
    const releases: { assets: { download_count: number }[] }[] =
      await res.json();
    return releases.reduce(
      (total, release) =>
        total +
        release.assets.reduce((sum, asset) => sum + asset.download_count, 0),
      0
    );
  } catch {
    return null;
  }
}

async function getStarCount(): Promise<number | null> {
  try {
    const res = await fetch(GITHUB_API, {
      next: { revalidate: 3600 },
    });
    if (!res.ok) {
      return null;
    }
    const repo: { stargazers_count: number } = await res.json();
    return repo.stargazers_count;
  } catch {
    return null;
  }
}

function formatCount(count: number): string {
  if (count >= 1_000_000) {
    return `${(count / 1_000_000).toFixed(1).replace(TRAILING_ZERO, "")}M`;
  }
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1).replace(TRAILING_ZERO, "")}K`;
  }
  return count.toLocaleString();
}

export default async function Home() {
  const [downloads, stars] = await Promise.all([
    getDownloadCount(),
    getStarCount(),
  ]);

  return (
    <main className="flex min-h-screen flex-col items-center overflow-hidden">
      {/* Section 1: Hero */}
      <section className="relative flex w-full max-w-6xl flex-col items-center px-6 pt-32 pb-24 text-center">
        <div className="absolute inset-0 -z-10 bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] from-cre-bg-float/20 via-cre-bg-deepest to-cre-bg-deepest" />

        <FadeIn delay={0.1}>
          <div className="relative mb-8 h-32 w-32">
            <Image
              alt="Chaos Recipe Enhancer Logo"
              className="object-contain drop-shadow-2xl"
              fill
              priority
              src="/images/cre-logo.png"
            />
          </div>
        </FadeIn>

        <FadeIn delay={0.2}>
          <h1 className="mb-6 font-bold text-4xl text-cre-text tracking-tight md:text-5xl lg:text-6xl">
            Chaos Recipe Enhancer
          </h1>
        </FadeIn>

        <FadeIn delay={0.3}>
          <p className="mb-4 font-medium text-cre-text-secondary text-xl md:text-2xl">
            Streamline your <RecipeTextCycle /> recipe gains.
          </p>
          <p className="mx-auto mb-10 max-w-2xl text-balance text-cre-text-secondary text-lg">
            Track chaos, regal, exalted & chance recipes. Smart stash overlays.
            Automatic loot filter manipulation.
          </p>
        </FadeIn>

        <FadeIn
          className="flex w-full flex-col items-center justify-center gap-4 sm:w-auto sm:flex-row"
          delay={0.4}
        >
          <Link
            className="flex w-full items-center justify-center gap-2 rounded-lg bg-cre-accent px-8 py-4 font-semibold text-white shadow-cre-accent/20 shadow-lg transition-colors hover:bg-cre-accent-hover sm:w-auto"
            href="https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer/releases"
            rel="noopener noreferrer"
            target="_blank"
          >
            <svg
              aria-hidden="true"
              className="h-5 w-5"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
              />
            </svg>
            Download Latest Release
            {downloads !== null && ` \u00B7 ${formatCount(downloads)}`}
          </Link>
          <Link
            className="flex w-full items-center justify-center gap-2 rounded-lg border border-cre-border bg-transparent px-8 py-4 font-semibold text-cre-text transition-all hover:border-cre-border-hover hover:bg-cre-bg-container sm:w-auto"
            href="https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer"
            rel="noopener noreferrer"
            target="_blank"
          >
            <svg
              aria-hidden="true"
              className="h-5 w-5"
              fill="currentColor"
              viewBox="0 0 24 24"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path d="M12 .297c-6.63 0-12 5.373-12 12 0 5.303 3.438 9.8 8.205 11.385.6.113.82-.258.82-.577 0-.285-.01-1.04-.015-2.04-3.338.724-4.042-1.61-4.042-1.61C4.422 18.07 3.633 17.7 3.633 17.7c-1.087-.744.084-.729.084-.729 1.205.084 1.838 1.236 1.838 1.236 1.07 1.835 2.809 1.305 3.495.998.108-.776.417-1.305.76-1.605-2.665-.3-5.466-1.332-5.466-5.93 0-1.31.465-2.38 1.235-3.22-.135-.303-.54-1.523.105-3.176 0 0 1.005-.322 3.3 1.23.96-.267 1.98-.399 3-.405 1.02.006 2.04.138 3 .405 2.28-1.552 3.285-1.23 3.285-1.23.645 1.653.24 2.873.12 3.176.765.84 1.23 1.91 1.23 3.22 0 4.61-2.805 5.625-5.475 5.92.42.36.81 1.096.81 2.22 0 1.606-.015 2.896-.015 3.286 0 .315.21.69.825.57C20.565 22.092 24 17.592 24 12.297c0-6.627-5.373-12-12-12" />
            </svg>
            View on GitHub
            {stars !== null && ` \u00B7 \u2605 ${formatCount(stars)}`}
          </Link>
        </FadeIn>
      </section>

      {/* Section 2: The Problem */}
      <section className="w-full border-cre-border-subtle border-t border-b">
        <div className="mx-auto max-w-6xl px-6 pt-24 pb-8 text-center">
          <FadeIn>
            <h2 className="mb-4 font-bold text-3xl text-cre-text md:text-4xl">
              The problem with Chaos Recipe
            </h2>
            <p className="mx-auto max-w-xl text-cre-text-secondary text-lg">
              Without the right tools, farming recipe sets is a nightmare.
            </p>
          </FadeIn>
        </div>
        <ProblemStatementDemo />
      </section>

      {/* Section 3: See it in action */}
      <section className="w-full max-w-6xl px-6 py-24">
        <FadeIn>
          <h2 className="mb-16 text-center font-bold text-3xl text-cre-text md:text-4xl">
            See it in action
          </h2>
        </FadeIn>

        <div className="space-y-32">
          {/* Demo 1: Item Drops */}
          <div className="flex flex-col items-center gap-12 md:flex-row md:gap-16">
            <div className="w-full md:w-1/2">
              <ItemDropDemo />
            </div>
            <div className="w-full max-w-sm md:w-1/2">
              <h3 className="mb-4 font-bold text-2xl text-cre-text">
                Never Miss a Drop
              </h3>
              <p className="text-cre-text-secondary text-lg leading-relaxed">
                Your loot filter highlights recipe items with distinct colors
                and light beams for each equipment slot. Recipe drops are
                impossible to miss.
              </p>
            </div>
          </div>

          {/* Demo 2: Stash Highlight */}
          <div className="flex flex-col-reverse items-center gap-12 md:flex-row md:gap-16">
            <div className="w-full max-w-sm md:w-1/2">
              <h3 className="mb-4 font-bold text-2xl text-cre-text">
                Smart Item Picking
              </h3>
              <p className="text-cre-text-secondary text-lg leading-relaxed">
                When it's time to sell, the stash overlay highlights items in
                the optimal pick order. Fit 2 full sets in one inventory.
              </p>
            </div>
            <div className="w-full md:w-1/2">
              <StashHighlightDemo />
            </div>
          </div>

          {/* Demo 3: Recipe Tracker */}
          <div className="flex flex-col items-center gap-12 md:flex-row md:gap-16">
            <div className="w-full md:w-1/2">
              <RecipeTrackerDemo />
            </div>
            <div className="w-full max-w-sm md:w-1/2">
              <h3 className="mb-4 font-bold text-2xl text-cre-text">
                Track Every Slot
              </h3>
              <p className="text-cre-text-secondary text-lg leading-relaxed">
                Monitor all equipment slots in real-time. Fetch manually with
                one click, or let CRE auto-refresh whenever you enter a new
                zone. The overlay highlights when a full set is ready to vendor.
              </p>
            </div>
          </div>

          {/* Demo 4: Filter Style */}
          <div className="flex flex-col-reverse items-center gap-12 md:flex-row md:gap-16">
            <div className="w-full max-w-sm md:w-1/2">
              <h3 className="mb-4 font-bold text-2xl text-cre-text">
                Extensive Loot Filter Customization
              </h3>
              <p className="text-cre-text-secondary text-lg leading-relaxed">
                The{" "}
                <Link
                  className="text-cre-accent underline underline-offset-2 transition-colors hover:text-cre-accent-hover"
                  href="https://www.filterblade.xyz/"
                  rel="noopener noreferrer"
                  target="_blank"
                >
                  FilterBlade
                </Link>
                -inspired editor lets you customize every color, icon, and beam.
                This allows you to tailor the visuals to your preferences and
                ensure that important recipe items stand out.
              </p>
            </div>
            <div className="w-full md:w-1/2">
              <FilterStyleDemo />
            </div>
          </div>

          {/* Demo 5: League Start Value */}
          <div className="flex flex-col items-center gap-12 md:flex-row md:gap-16">
            <div className="w-full md:w-1/2">
              <LeagueStartDemo />
            </div>
            <div className="w-full max-w-sm md:w-1/2">
              <h3 className="mb-4 font-bold text-2xl text-cre-text">
                Farm While You Map
              </h3>
              <p className="text-cre-text-secondary text-lg leading-relaxed">
                Chaos recipe is your best early-league currency strategy. CRE
                tracks everything passively while you run maps — no stopping, no
                spreadsheets. Bank your first few Divines early, then switch to
                endgame farming when you hit reds.
              </p>
              <p className="mt-3 text-cre-text-muted text-sm italic">
                You're not meant to do this all league — just league start.
              </p>
            </div>
          </div>

          {/* YouTube Video */}
          <FadeIn>
            <div className="flex flex-col items-center border-cre-border-subtle border-t pt-16">
              <Link
                className="group relative block w-full max-w-4xl overflow-hidden rounded-xl border border-cre-border bg-cre-bg-deep shadow-2xl"
                href="https://youtu.be/W_C4Nhla-Xg"
                rel="noopener noreferrer"
                target="_blank"
              >
                <div className="relative aspect-video w-full">
                  <Image
                    alt="Watch the full guide"
                    className="object-cover transition-transform duration-500 group-hover:scale-105"
                    fill
                    src="/images/video-thumbnail.png"
                  />
                  <div className="absolute inset-0 flex items-center justify-center bg-black/40 transition-colors group-hover:bg-black/20">
                    <div className="flex h-20 w-20 transform items-center justify-center rounded-full bg-red-600 shadow-lg transition-transform group-hover:scale-110">
                      <svg
                        aria-hidden="true"
                        className="ml-2 h-10 w-10 text-white"
                        fill="currentColor"
                        viewBox="0 0 24 24"
                      >
                        <path d="M8 5v14l11-7z" />
                      </svg>
                    </div>
                  </div>
                </div>
              </Link>
              <p className="mt-6 font-medium text-cre-text-secondary text-lg">
                Watch the full guide
              </p>
            </div>
          </FadeIn>
        </div>
      </section>

      {/* Section 4: Features Grid */}
      <section className="w-full max-w-6xl border-cre-border-subtle border-t px-6 py-24">
        <FadeIn>
          <h2 className="mb-16 text-center font-bold text-3xl text-cre-text md:text-4xl">
            Everything you need to farm efficiently
          </h2>
        </FadeIn>

        <StaggerChildren className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          <StaggerItem className="group rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover">
            <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-lg bg-cre-bg-float text-cre-gold transition-transform group-hover:scale-110">
              <svg
                aria-hidden="true"
                className="h-6 w-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                />
              </svg>
            </div>
            <h3 className="mb-3 font-semibold text-cre-text text-xl">
              Recipe Tracking
            </h3>
            <p className="text-cre-text-secondary leading-relaxed">
              Track chaos, regal, exalted & chance orb recipes. See exactly
              which items you need at a glance.
            </p>
          </StaggerItem>

          <StaggerItem className="group rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover">
            <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-lg bg-cre-bg-float text-cre-accent transition-transform group-hover:scale-110">
              <svg
                aria-hidden="true"
                className="h-6 w-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M4 5a1 1 0 011-1h14a1 1 0 011 1v2a1 1 0 01-1 1H5a1 1 0 01-1-1V5zM4 13a1 1 0 011-1h6a1 1 0 011 1v6a1 1 0 01-1 1H5a1 1 0 01-1-1v-6zM16 13a1 1 0 011-1h2a1 1 0 011 1v6a1 1 0 01-1 1h-2a1 1 0 01-1-1v-6z"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                />
              </svg>
            </div>
            <h3 className="mb-3 font-semibold text-cre-text text-xl">
              Smart Stash Overlay
            </h3>
            <p className="text-cre-text-secondary leading-relaxed">
              Highlights items in optimal pick order. Fit 2 full sets per
              inventory when vendoring.
            </p>
          </StaggerItem>

          <StaggerItem className="group rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover">
            <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-lg bg-cre-bg-float text-cre-gold transition-transform group-hover:scale-110">
              <svg
                aria-hidden="true"
                className="h-6 w-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                />
              </svg>
            </div>
            <h3 className="mb-3 font-semibold text-cre-text text-xl">
              Loot Filter Manipulation
            </h3>
            <p className="text-cre-text-secondary leading-relaxed">
              Automatically modifies your offline loot filter as recipe needs
              change. FilterBlade-inspired style editor.
            </p>
          </StaggerItem>

          <StaggerItem className="group rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover">
            <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-lg bg-cre-bg-float text-cre-accent transition-transform group-hover:scale-110">
              <svg
                aria-hidden="true"
                className="h-6 w-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                />
              </svg>
            </div>
            <h3 className="mb-3 font-semibold text-cre-text text-xl">
              Secure OAuth
            </h3>
            <p className="text-cre-text-secondary leading-relaxed">
              2-click secure authentication via Path of Exile. No credentials
              stored locally.
            </p>
          </StaggerItem>

          <StaggerItem className="group rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover">
            <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-lg bg-cre-bg-float text-cre-gold transition-transform group-hover:scale-110">
              <svg
                aria-hidden="true"
                className="h-6 w-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                />
              </svg>
            </div>
            <h3 className="mb-3 font-semibold text-cre-text text-xl">
              Auto-Fetch
            </h3>
            <p className="text-cre-text-secondary leading-relaxed">
              Automatically fetches remaining items when you join a new
              instance. Built-in rate limiting.
            </p>
          </StaggerItem>

          <StaggerItem className="group rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover">
            <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-lg bg-cre-bg-float text-cre-accent transition-transform group-hover:scale-110">
              <svg
                aria-hidden="true"
                className="h-6 w-6"
                fill="none"
                stroke="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path
                  d="M15 15l-2 5L9 9l11 4-5 2zm0 0l5 5M7.188 2.239l.777 2.897M5.136 7.965l-2.898-.777M13.95 4.05l-2.122 2.122m-5.657 5.656l-2.12 2.122"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  strokeWidth={2}
                />
              </svg>
            </div>
            <h3 className="mb-3 font-semibold text-cre-text text-xl">
              Customizable Hotkeys
            </h3>
            <p className="text-cre-text-secondary leading-relaxed">
              Bind any key to toggle overlays, start/stop tracking, and control
              fetching.
            </p>
          </StaggerItem>
        </StaggerChildren>
      </section>

      {/* Section 4: Open Source & Community */}
      <section className="w-full max-w-6xl border-cre-border-subtle border-t px-6 py-24">
        <FadeIn>
          <div className="mb-16 text-center">
            <h2 className="mb-6 font-bold text-3xl text-cre-text md:text-4xl">
              Free & Open Source
            </h2>
            <p className="mx-auto max-w-2xl text-cre-text-secondary text-xl">
              CRE is GPL-3.0 licensed and maintained by the community. Join
              thousands of Path of Exile players.
            </p>
          </div>
        </FadeIn>

        <StaggerChildren className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          <StaggerItem>
            <Link
              className="group flex flex-col items-center rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover"
              href="https://discord.gg/hy5xSgw3au"
              rel="noopener noreferrer"
              target="_blank"
            >
              <svg
                aria-hidden="true"
                className="mb-4 h-8 w-8 text-[#5865F2] transition-transform group-hover:scale-110"
                fill="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path d="M20.317 4.3698a19.7913 19.7913 0 00-4.8851-1.5152.0741.0741 0 00-.0785.0371c-.211.3753-.4447.8648-.6083 1.2495-1.8447-.2762-3.68-.2762-5.4868 0-.1636-.3933-.4058-.8742-.6177-1.2495a.077.077 0 00-.0785-.037 19.7363 19.7363 0 00-4.8852 1.515.0699.0699 0 00-.0321.0277C.5334 9.0458-.319 13.5799.0992 18.0578a.0824.0824 0 00.0312.0561c2.0528 1.5076 4.0413 2.4228 5.9929 3.0294a.0777.0777 0 00.0842-.0276c.4616-.6304.8731-1.2952 1.226-1.9942a.076.076 0 00-.0416-.1057c-.6528-.2476-1.2743-.5495-1.8722-.8923a.077.077 0 01-.0076-.1277c.1258-.0943.2517-.1923.3718-.2914a.0743.0743 0 01.0776-.0105c3.9278 1.7933 8.18 1.7933 12.0614 0a.0739.0739 0 01.0785.0095c.1202.099.246.1981.3728.2924a.077.077 0 01-.0066.1276 12.2986 12.2986 0 01-1.873.8914.0766.0766 0 00-.0407.1067c.3604.698.7719 1.3628 1.225 1.9932a.076.076 0 00.0842.0286c1.961-.6067 3.9495-1.5219 6.0023-3.0294a.077.077 0 00.0313-.0552c.5004-5.177-.8382-9.6739-3.5485-13.6604a.061.061 0 00-.0312-.0286zM8.02 15.3312c-1.1825 0-2.1569-1.0857-2.1569-2.419 0-1.3332.9555-2.4189 2.157-2.4189 1.2108 0 2.1757 1.0952 2.1568 2.419 0 1.3332-.9555 2.4189-2.1569 2.4189zm7.9748 0c-1.1825 0-2.1569-1.0857-2.1569-2.419 0-1.3332.9554-2.4189 2.1569-2.4189 1.2108 0 2.1757 1.0952 2.1568 2.419 0 1.3332-.946 2.4189-2.1568 2.4189Z" />
              </svg>
              <span className="font-semibold text-cre-text">
                Join the Community
              </span>
              <span className="mt-1 text-cre-text-muted text-sm">Discord</span>
            </Link>
          </StaggerItem>

          <StaggerItem>
            <Link
              className="group flex flex-col items-center rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover"
              href="https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer"
              rel="noopener noreferrer"
              target="_blank"
            >
              <svg
                aria-hidden="true"
                className="mb-4 h-8 w-8 text-cre-text transition-transform group-hover:scale-110"
                fill="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path d="M12 .297c-6.63 0-12 5.373-12 12 0 5.303 3.438 9.8 8.205 11.385.6.113.82-.258.82-.577 0-.285-.01-1.04-.015-2.04-3.338.724-4.042-1.61-4.042-1.61C4.422 18.07 3.633 17.7 3.633 17.7c-1.087-.744.084-.729.084-.729 1.205.084 1.838 1.236 1.838 1.236 1.07 1.835 2.809 1.305 3.495.998.108-.776.417-1.305.76-1.605-2.665-.3-5.466-1.332-5.466-5.93 0-1.31.465-2.38 1.235-3.22-.135-.303-.54-1.523.105-3.176 0 0 1.005-.322 3.3 1.23.96-.267 1.98-.399 3-.405 1.02.006 2.04.138 3 .405 2.28-1.552 3.285-1.23 3.285-1.23.645 1.653.24 2.873.12 3.176.765.84 1.23 1.91 1.23 3.22 0 4.61-2.805 5.625-5.475 5.92.42.36.81 1.096.81 2.22 0 1.606-.015 2.896-.015 3.286 0 .315.21.69.825.57C20.565 22.092 24 17.592 24 12.297c0-6.627-5.373-12-12-12" />
              </svg>
              <span className="font-semibold text-cre-text">
                Star on GitHub
              </span>
              <span className="mt-1 text-cre-text-muted text-sm">GitHub</span>
            </Link>
          </StaggerItem>

          <StaggerItem>
            <Link
              className="group flex flex-col items-center rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover"
              href="https://youtu.be/W_C4Nhla-Xg"
              rel="noopener noreferrer"
              target="_blank"
            >
              <svg
                aria-hidden="true"
                className="mb-4 h-8 w-8 text-[#FF0000] transition-transform group-hover:scale-110"
                fill="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path d="M23.498 6.186a3.016 3.016 0 0 0-2.122-2.136C19.505 3.545 12 3.545 12 3.545s-7.505 0-9.377.505A3.017 3.017 0 0 0 .502 6.186C0 8.07 0 12 0 12s0 3.93.502 5.814a3.016 3.016 0 0 0 2.122 2.136c1.871.505 9.376.505 9.376.505s7.505 0 9.377-.505a3.015 3.015 0 0 0 2.122-2.136C24 15.93 24 12 24 12s0-3.93-.502-5.814zM9.545 15.568V8.432L15.818 12l-6.273 3.568z" />
              </svg>
              <span className="font-semibold text-cre-text">
                Watch the Guide
              </span>
              <span className="mt-1 text-cre-text-muted text-sm">YouTube</span>
            </Link>
          </StaggerItem>

          <StaggerItem>
            <Link
              className="group flex flex-col items-center rounded-xl border border-cre-border bg-cre-bg-container p-8 transition-colors hover:border-cre-border-hover"
              href="https://www.paypal.com/donate/?hosted_button_id=4NDCV5J5NTEWS"
              rel="noopener noreferrer"
              target="_blank"
            >
              <svg
                aria-hidden="true"
                className="mb-4 h-8 w-8 text-[#00457C] transition-transform group-hover:scale-110"
                fill="currentColor"
                viewBox="0 0 24 24"
                xmlns="http://www.w3.org/2000/svg"
              >
                <path d="M7.076 21.337H2.47a.641.641 0 0 1-.633-.74L4.944.901C5.026.382 5.474 0 5.998 0h7.46c2.57 0 4.578.543 5.69 1.81 1.01 1.15 1.304 2.42 1.012 4.287-.023.143-.047.288-.077.437-.983 5.05-4.349 6.797-8.647 6.797h-2.19c-.524 0-.968.382-1.05.9l-1.12 7.106zm14.146-14.42a3.35 3.35 0 0 0-.607-.541c-.013.076-.026.175-.041.254-.93 4.778-4.005 7.201-9.138 7.201h-2.19a2.008 2.008 0 0 0-1.968 1.671l-1.594 10.111H9.78a1.004 1.004 0 0 0 .984-.835l.156-1.003a.998.998 0 0 1 .984-.834h.61c4.359 0 7.465-1.73 8.436-6.88.438-2.32.332-4.38-.998-5.744z" />
              </svg>
              <span className="font-semibold text-cre-text">
                Support Development
              </span>
              <span className="mt-1 text-cre-text-muted text-sm">PayPal</span>
            </Link>
          </StaggerItem>
        </StaggerChildren>
      </section>

      {/* Section 5: Quick Start */}
      <section className="w-full max-w-4xl border-cre-border-subtle border-t px-6 py-24">
        <FadeIn>
          <h2 className="mb-16 text-center font-bold text-3xl text-cre-text md:text-4xl">
            Get Started in 2 Steps
          </h2>
        </FadeIn>

        <div className="space-y-8">
          <FadeIn delay={0.1}>
            <div className="flex items-start gap-6 rounded-xl border border-cre-border bg-cre-bg-container p-8">
              <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-cre-bg-float font-bold text-cre-gold text-xl">
                1
              </div>
              <div>
                <h3 className="mb-2 font-semibold text-cre-text text-xl">
                  Install .NET 8 Desktop Runtime
                </h3>
                <p className="mb-4 text-cre-text-secondary">
                  Chaos Recipe Enhancer requires the .NET 8 Desktop Runtime to
                  function properly.
                </p>
                <Link
                  className="inline-flex items-center gap-2 font-medium text-cre-accent transition-colors hover:text-cre-accent-hover"
                  href="https://dotnet.microsoft.com/en-us/download/dotnet/8.0"
                  rel="noopener noreferrer"
                  target="_blank"
                >
                  Download .NET 8 Desktop Runtime
                  <svg
                    aria-hidden="true"
                    className="h-4 w-4"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                    />
                  </svg>
                </Link>
              </div>
            </div>
          </FadeIn>

          <FadeIn delay={0.2}>
            <div className="flex items-start gap-6 rounded-xl border border-cre-border bg-cre-bg-container p-8">
              <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-full bg-cre-bg-float font-bold text-cre-accent text-xl">
                2
              </div>
              <div>
                <h3 className="mb-2 font-semibold text-cre-text text-xl">
                  Download ChaosRecipeEnhancer
                </h3>
                <p className="mb-4 text-cre-text-secondary">
                  Get the latest release from GitHub. Extract the files and run
                  the executable.
                </p>
                <Link
                  className="inline-flex items-center gap-2 font-medium text-cre-accent transition-colors hover:text-cre-accent-hover"
                  href="https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer/releases"
                  rel="noopener noreferrer"
                  target="_blank"
                >
                  Download Latest Release
                  <svg
                    aria-hidden="true"
                    className="h-4 w-4"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M10 6H6a2 2 0 00-2 2v10a2 2 0 002 2h10a2 2 0 002-2v-4M14 4h6m0 0v6m0-6L10 14"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth={2}
                    />
                  </svg>
                </Link>
              </div>
            </div>
          </FadeIn>
        </div>

        <FadeIn delay={0.3}>
          <p className="mt-12 text-center text-cre-text-muted text-sm">
            System requirements: Windows 10/11 · .NET 8 · Windowed or Borderless
            Fullscreen
          </p>
        </FadeIn>
      </section>

      {/* Section 6: Footer */}
      <footer className="w-full border-cre-border-subtle border-t bg-cre-bg-deep px-6 py-12">
        <div className="mx-auto flex max-w-6xl flex-col items-center text-center">
          <div className="relative mb-6 h-12 w-12 opacity-50 grayscale transition-all hover:opacity-100 hover:grayscale-0">
            <Image
              alt="CRE Logo"
              className="object-contain"
              fill
              src="/images/cre-logo.png"
            />
          </div>

          <div className="mb-8 flex flex-wrap justify-center gap-6 font-medium text-sm">
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href="https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer"
            >
              GitHub
            </Link>
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href="https://discord.gg/hy5xSgw3au"
            >
              Discord
            </Link>
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href="https://github.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer/releases"
            >
              Releases
            </Link>
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href="https://www.paypal.com/donate/?hosted_button_id=4NDCV5J5NTEWS"
            >
              Support
            </Link>
          </div>

          <p className="mb-2 text-cre-text-muted text-sm">
            © {new Date().getFullYear()} Chaos Recipe Enhancer Team · GPL-3.0
            License
          </p>
          <p className="text-cre-text-muted text-xs opacity-70">
            Not affiliated with or endorsed by Grinding Gear Games.
          </p>
        </div>
      </footer>
    </main>
  );
}
