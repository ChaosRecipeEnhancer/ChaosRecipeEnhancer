import dynamic from "next/dynamic";
import Image from "next/image";
import Link from "next/link";
import type { ComponentType, SVGProps } from "react";

import { ErrorBoundary } from "@/components/error-boundary";
import {
  DiscordIcon,
  DownloadIcon,
  GitHubIcon,
  PayPalIcon,
  PlayIcon,
  YouTubeIcon,
} from "@/components/icons";
import {
  DISCORD_URL,
  GITHUB_RELEASES_URL,
  GITHUB_REPO_URL,
  PAYPAL_DONATE_URL,
  YOUTUBE_GUIDE_URL,
} from "@/lib/constants";
import { getDiscordMemberCount } from "@/lib/discord";
import { formatCount, getGitHubStats } from "@/lib/github";

const FilterStyleDemo = dynamic(() =>
  import("@/components/demos/filter-style-demo").then((m) => m.FilterStyleDemo)
);
const ItemDropDemo = dynamic(() =>
  import("@/components/demos/item-drop-demo").then((m) => m.ItemDropDemo)
);
const LeagueStartDemo = dynamic(() =>
  import("@/components/demos/league-start-demo").then((m) => m.LeagueStartDemo)
);
const ProblemStatementDemo = dynamic(() =>
  import("@/components/demos/problem-statement-demo").then(
    (m) => m.ProblemStatementDemo
  )
);
const RecipeTrackerDemo = dynamic(() =>
  import("@/components/demos/recipe-tracker-demo").then(
    (m) => m.RecipeTrackerDemo
  )
);
const StashHighlightDemo = dynamic(() =>
  import("@/components/demos/stash-highlight-demo").then(
    (m) => m.StashHighlightDemo
  )
);
const RecipeTextCycle = dynamic(
  () => import("@/components/recipe-text-cycle").then((m) => m.RecipeTextCycle),
  {
    loading: () => <span className="text-[#D4A843]">Chaos</span>,
  }
);

// ---------------------------------------------------------------------------
// Data
// ---------------------------------------------------------------------------

const FEATURES = [
  {
    emoji: "\u{1F50A}",
    title: "Custom Filter Sounds",
    description: "Community sound packs from streamers & NPCs",
  },
  {
    emoji: "\u2328\uFE0F",
    title: "Global Hotkeys",
    description: "Bind any key to fetch, toggle overlays, or reload",
  },
  {
    emoji: "\u{1F504}",
    title: "Auto-Fetch on Rezone",
    description: "Refreshes stash data every time you enter a new zone",
  },
  {
    emoji: "\u{1F514}",
    title: "Sound Notifications",
    description: "Audio alerts when sets complete or items change",
  },
  {
    emoji: "\u{1F48E}",
    title: "4 Recipe Types",
    description: "Chaos, Regal, Chance & Exalted with influence targeting",
  },
  {
    emoji: "\u{1F3A8}",
    title: "8 Overlay Layouts",
    description: "Standard, vertical, minified & buttons-only modes",
  },
  {
    emoji: "\u{1F30D}",
    title: "Multi-Language",
    description: "Supports 8 languages for zone detection",
  },
  {
    emoji: "\u{1F465}",
    title: "Guild Stash Support",
    description: "Query from your guild's shared stash tabs",
  },
  {
    emoji: "\u{1F680}",
    title: "Auto-Updates",
    description: "New versions install seamlessly in the background",
  },
  {
    emoji: "\u{1F510}",
    title: "Secure OAuth Login",
    description: "2-click connection to PoE \u2014 no POESESSID needed",
  },
];

interface CommunityLink {
  href: string;
  icon: ComponentType<SVGProps<SVGSVGElement>>;
  iconClass: string;
  subtitle: string;
  title: string;
}

const COMMUNITY_LINKS: CommunityLink[] = [
  {
    icon: DiscordIcon,
    iconClass: "text-[#5865F2]",
    title: "Join the Community",
    subtitle: "Discord",
    href: DISCORD_URL,
  },
  {
    icon: GitHubIcon,
    iconClass: "text-cre-text",
    title: "Star on GitHub",
    subtitle: "GitHub",
    href: GITHUB_REPO_URL,
  },
  {
    icon: YouTubeIcon,
    iconClass: "text-[#FF0000]",
    title: "Watch the Guide",
    subtitle: "YouTube",
    href: YOUTUBE_GUIDE_URL,
  },
  {
    icon: PayPalIcon,
    iconClass: "text-[#00457C]",
    title: "Support Development",
    subtitle: "PayPal",
    href: PAYPAL_DONATE_URL,
  },
];

// ---------------------------------------------------------------------------
// Page
// ---------------------------------------------------------------------------

export default async function Home() {
  const [{ downloads, stars }, discordMembers] = await Promise.all([
    getGitHubStats(),
    getDiscordMemberCount(),
  ]);
  return (
    <main className="flex min-h-screen flex-col items-center overflow-hidden">
      {/* Section 1: Hero */}
      <section className="relative flex w-full max-w-6xl flex-col items-center px-6 pt-20 pb-16 text-center md:pt-32 md:pb-24">
        <div className="absolute inset-0 -z-10 bg-[radial-gradient(ellipse_at_top,_var(--tw-gradient-stops))] from-cre-bg-float/20 via-cre-bg-deepest to-cre-bg-deepest" />

        <div className="relative mb-6 h-24 w-24 md:mb-8 md:h-32 md:w-32">
          <Image
            alt="Chaos Recipe Enhancer Logo"
            className="object-contain drop-shadow-2xl"
            fetchPriority="high"
            fill
            priority
            sizes="(min-width: 768px) 128px, 96px"
            src="/images/cre-logo.png"
          />
        </div>

        <h1 className="mb-4 font-bold text-3xl text-cre-text tracking-tight md:mb-6 md:text-5xl lg:text-6xl">
          Chaos Recipe Enhancer
        </h1>

        <p className="mb-4 font-medium text-cre-text-secondary text-xl md:text-2xl">
          Streamline your <RecipeTextCycle /> recipe gains.
        </p>
        <p className="mx-auto mb-8 max-w-2xl text-balance text-cre-text-secondary text-lg md:mb-10">
          Track chaos, regal, exalted & chance recipes. Smart stash overlays.
          Automatic loot filter manipulation.
        </p>

        <div className="flex w-full flex-col items-center justify-center gap-4 sm:w-auto sm:flex-row">
          <Link
            className="flex w-full items-center justify-center gap-2 rounded-lg bg-cre-accent px-8 py-4 font-semibold text-white shadow-cre-accent/20 shadow-lg transition-colors hover:bg-cre-accent-hover sm:w-auto"
            href={GITHUB_RELEASES_URL}
            rel="noopener noreferrer"
            target="_blank"
          >
            <DownloadIcon className="h-5 w-5" />
            Download Latest Release
            {downloads !== null && ` \u00B7 ${formatCount(downloads)}`}
          </Link>
          <Link
            className="flex w-full items-center justify-center gap-2 rounded-lg border border-cre-border bg-transparent px-8 py-4 font-semibold text-cre-text transition-all hover:border-cre-border-hover hover:bg-cre-bg-container sm:w-auto"
            href={GITHUB_REPO_URL}
            rel="noopener noreferrer"
            target="_blank"
          >
            <GitHubIcon className="h-5 w-5" />
            View on GitHub
            {stars !== null && ` \u00B7 \u2605 ${formatCount(stars)}`}
          </Link>
          <Link
            className="flex w-full items-center justify-center gap-2 rounded-lg border border-[#5865F2]/40 bg-transparent px-8 py-4 font-semibold text-[#5865F2] transition-all hover:border-[#5865F2] hover:bg-[#5865F2]/10 sm:w-auto"
            href={DISCORD_URL}
            rel="noopener noreferrer"
            target="_blank"
          >
            <DiscordIcon className="h-5 w-5" />
            Join Discord
            {discordMembers !== null && ` · ${formatCount(discordMembers)}`}
          </Link>
        </div>
      </section>

      {/* Section 2: The Problem */}
      <section className="w-full border-cre-border-subtle border-t border-b">
        <div className="mx-auto max-w-6xl px-6 pt-16 pb-6 text-center md:pt-24 md:pb-8">
          <h2 className="mb-4 font-bold text-3xl text-cre-text md:text-4xl">
            The problem with Chaos Recipe
          </h2>
          <p className="mx-auto max-w-xl text-cre-text-secondary text-lg">
            Without the right tools, farming recipe sets is a nightmare.
          </p>
        </div>
        <ErrorBoundary>
          <ProblemStatementDemo />
        </ErrorBoundary>
      </section>

      {/* Section 3: See it in action */}
      <section className="w-full max-w-6xl px-6 py-16 md:py-24">
        <h2 className="mb-10 text-center font-bold text-3xl text-cre-text md:mb-16 md:text-4xl">
          See it in action
        </h2>

        <div className="space-y-16 md:space-y-32">
          {/* Demo 1: Item Drops */}
          <div className="flex flex-col-reverse items-center gap-6 md:flex-row md:gap-16">
            <div className="w-full md:w-1/2">
              <ErrorBoundary>
                <ItemDropDemo />
              </ErrorBoundary>
            </div>
            <div className="w-full text-center md:w-1/2 md:text-left">
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
          <div className="flex flex-col items-center gap-6 md:flex-row md:gap-16">
            <div className="w-full text-center md:w-1/2 md:text-left">
              <h3 className="mb-4 font-bold text-2xl text-cre-text">
                Smart Item Picking
              </h3>
              <p className="text-cre-text-secondary text-lg leading-relaxed">
                When it&apos;s time to sell, the stash overlay highlights items
                in the optimal pick order. Fit 2 full sets in one inventory.
              </p>
            </div>
            <div className="w-full md:w-1/2">
              <ErrorBoundary>
                <StashHighlightDemo />
              </ErrorBoundary>
            </div>
          </div>

          {/* Demo 3: Recipe Tracker */}
          <div className="flex flex-col-reverse items-center gap-6 md:flex-row md:gap-16">
            <div className="w-full md:w-1/2">
              <ErrorBoundary>
                <RecipeTrackerDemo />
              </ErrorBoundary>
            </div>
            <div className="w-full text-center md:w-1/2 md:text-left">
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
          <div className="flex flex-col items-center gap-6 md:flex-row md:gap-16">
            <div className="w-full text-center md:w-1/2 md:text-left">
              <h3 className="mb-4 font-bold text-2xl text-cre-text">
                Extensive Loot Filter Customization
              </h3>
              <p className="text-cre-text-secondary text-lg leading-relaxed">
                The{" "}
                <Link
                  className="text-cre-link underline underline-offset-2 transition-colors hover:text-cre-link-hover"
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
              <ErrorBoundary>
                <FilterStyleDemo />
              </ErrorBoundary>
            </div>
          </div>

          {/* Demo 5: League Start Value */}
          <div className="flex flex-col-reverse items-center gap-6 md:flex-row md:gap-16">
            <div className="w-full md:w-1/2">
              <ErrorBoundary>
                <LeagueStartDemo />
              </ErrorBoundary>
            </div>
            <div className="w-full text-center md:w-1/2 md:text-left">
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
                You&apos;re not meant to do this all league — just league start.
              </p>
            </div>
          </div>

          {/* Feature Grid */}
          <div className="border-cre-border-subtle border-t pt-10 md:pt-16">
            <h3 className="mb-2 text-center font-bold text-2xl text-cre-text md:text-3xl">
              Packed with Features
            </h3>
            <p className="mx-auto mb-8 max-w-xl text-center text-cre-text-secondary text-lg md:mb-12">
              Everything you need to optimize your recipe farming.
            </p>

            <div className="grid grid-cols-2 gap-3 sm:grid-cols-3 md:gap-4 lg:grid-cols-5">
              {FEATURES.map((feature) => (
                <div
                  className="flex h-full flex-col items-center gap-2 rounded-lg border border-cre-border bg-cre-bg-container p-4 text-center transition-colors hover:border-cre-border-hover md:p-5"
                  key={feature.title}
                >
                  <span aria-hidden="true" className="text-2xl">
                    {feature.emoji}
                  </span>
                  <span className="font-semibold text-cre-text text-sm">
                    {feature.title}
                  </span>
                  <span className="text-cre-text-muted text-xs leading-snug">
                    {feature.description}
                  </span>
                </div>
              ))}
            </div>

            <p className="mt-8 text-center text-sm italic md:mt-10">
              <span className="bg-gradient-to-r from-cre-accent to-cre-gold bg-clip-text text-transparent">
                ...and I&apos;m always willing to add more features based on
                community feedback
              </span>
            </p>
          </div>

          {/* YouTube Video */}
          <div className="flex flex-col items-center border-cre-border-subtle border-t pt-10 md:pt-16">
            <Link
              className="group relative block w-full max-w-4xl overflow-hidden rounded-xl border border-cre-border bg-cre-bg-deep shadow-2xl"
              href={YOUTUBE_GUIDE_URL}
              rel="noopener noreferrer"
              target="_blank"
            >
              <div className="relative aspect-video w-full">
                <Image
                  alt="Watch the full guide"
                  className="object-cover transition-transform duration-500 group-hover:scale-105"
                  fill
                  sizes="(min-width: 768px) 896px, 100vw"
                  src="/images/video-thumbnail.png"
                />
                <div className="absolute inset-0 flex items-center justify-center bg-black/40 transition-colors group-hover:bg-black/20">
                  <div className="flex h-20 w-20 transform items-center justify-center rounded-full bg-red-600 shadow-lg transition-transform group-hover:scale-110">
                    <PlayIcon className="ml-1 h-10 w-10 text-white" />
                  </div>
                </div>
              </div>
            </Link>
            <p className="mt-6 font-medium text-cre-text-secondary text-lg">
              Watch the full guide
            </p>
          </div>
        </div>
      </section>

      {/* Section 4: Open Source & Community */}
      <section className="w-full max-w-6xl border-cre-border-subtle border-t px-6 py-16 md:py-24">
        <div className="mb-10 text-center md:mb-16">
          <h2 className="mb-6 font-bold text-3xl text-cre-text md:text-4xl">
            Free & Open Source
          </h2>
          <p className="mx-auto max-w-2xl text-cre-text-secondary text-xl">
            CRE is GPL-3.0 licensed and maintained by the community. Join
            thousands of Path of Exile players.
          </p>
        </div>

        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-4">
          {COMMUNITY_LINKS.map((link) => (
            <Link
              className="group flex flex-col items-center rounded-xl border border-cre-border bg-cre-bg-container p-6 transition-colors hover:border-cre-border-hover md:p-8"
              href={link.href}
              key={link.subtitle}
              rel="noopener noreferrer"
              target="_blank"
            >
              <link.icon
                className={`mb-4 h-8 w-8 transition-transform group-hover:scale-110 ${link.iconClass}`}
              />
              <span className="font-semibold text-cre-text">{link.title}</span>
              <span className="mt-1 text-cre-text-muted text-sm">
                {link.subtitle}
              </span>
            </Link>
          ))}
        </div>
      </section>

      {/* Section 5: Get Started */}
      <section className="w-full max-w-4xl border-cre-border-subtle border-t px-6 py-16 md:py-24">
        <div className="flex flex-col items-center text-center">
          <h2 className="mb-4 font-bold text-3xl text-cre-text md:text-4xl">
            Get Started in 1 Step
          </h2>
          <p className="mx-auto mb-8 max-w-xl text-cre-text-secondary text-lg md:mb-10">
            Download the installer, run it, and you&apos;re ready to go.
            Everything you need is bundled — no extra downloads required.
          </p>
          <Link
            className="flex items-center justify-center gap-3 rounded-lg bg-cre-accent px-10 py-5 font-semibold text-lg text-white shadow-cre-accent/20 shadow-lg transition-colors hover:bg-cre-accent-hover"
            href={GITHUB_RELEASES_URL}
            rel="noopener noreferrer"
            target="_blank"
          >
            <DownloadIcon className="h-6 w-6" />
            Download Chaos Recipe Enhancer
          </Link>
          <p className="mt-6 text-cre-text-muted text-sm">
            Windows 10/11 · Path of Exile in Windowed or Borderless
          </p>
        </div>
      </section>

      {/* Section 6: Footer */}
      <footer className="w-full border-cre-border-subtle border-t bg-cre-bg-deep px-6 py-12">
        <div className="mx-auto flex max-w-6xl flex-col items-center text-center">
          <div className="relative mb-6 h-12 w-12 opacity-50 grayscale transition-all hover:opacity-100 hover:grayscale-0">
            <Image
              alt="CRE Logo"
              className="object-contain"
              fill
              sizes="48px"
              src="/images/cre-logo.png"
            />
          </div>

          <div className="mb-8 flex flex-wrap justify-center gap-6 font-medium text-sm">
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href={GITHUB_REPO_URL}
            >
              GitHub
            </Link>
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href={DISCORD_URL}
            >
              Discord
            </Link>
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href={GITHUB_RELEASES_URL}
            >
              Releases
            </Link>
            <Link
              className="text-cre-text-secondary transition-colors hover:text-cre-text"
              href={PAYPAL_DONATE_URL}
            >
              Donate
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
