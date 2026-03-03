import { DISCORD_URL } from "./constants";

/**
 * Extract the invite code from a discord.gg URL.
 * e.g. "https://discord.gg/hy5xSgw3au" → "hy5xSgw3au"
 */
function getInviteCode(url: string): string {
  return url.split("/").at(-1) ?? "";
}

/**
 * Fetch approximate member count via Discord's public invite API.
 * No auth required — results are cached/revalidated once per day.
 */
export async function getDiscordMemberCount(): Promise<number | null> {
  try {
    const code = getInviteCode(DISCORD_URL);
    const res = await fetch(
      `https://discord.com/api/v10/invites/${code}?with_counts=true`,
      { next: { revalidate: 86_400 } }
    );

    if (!res.ok) {
      return null;
    }

    const data: { approximate_member_count?: number } = await res.json();
    return data.approximate_member_count ?? null;
  } catch {
    return null;
  }
}
