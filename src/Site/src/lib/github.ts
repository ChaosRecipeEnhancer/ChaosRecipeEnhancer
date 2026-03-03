const TRAILING_ZERO = /\.0$/;

export const GITHUB_API =
  "https://api.github.com/repos/ChaosRecipeEnhancer/ChaosRecipeEnhancer";
export const GITHUB_RELEASES_API = `${GITHUB_API}/releases?per_page=100`;

export function formatCount(count: number): string {
  if (count >= 1_000_000) {
    return `${(count / 1_000_000).toFixed(1).replace(TRAILING_ZERO, "")}M`;
  }
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1).replace(TRAILING_ZERO, "")}K`;
  }
  return count.toLocaleString();
}

export async function getGitHubStats(): Promise<{
  downloads: number | null;
  stars: number | null;
}> {
  try {
    const [repoRes, releasesRes] = await Promise.all([
      fetch(GITHUB_API, { next: { revalidate: 3600 } }),
      fetch(GITHUB_RELEASES_API, { next: { revalidate: 3600 } }),
    ]);

    const stars = repoRes.ok
      ? ((await repoRes.json()) as { stargazers_count: number })
          .stargazers_count
      : null;

    let downloads: number | null = null;
    if (releasesRes.ok) {
      const releases: { assets: { download_count: number }[] }[] =
        await releasesRes.json();
      downloads = releases.reduce(
        (total, release) =>
          total +
          release.assets.reduce((sum, asset) => sum + asset.download_count, 0),
        0
      );
    }

    return { downloads, stars };
  } catch {
    return { downloads: null, stars: null };
  }
}
