import { ImageResponse } from "next/og";

export const alt = "Chaos Recipe Enhancer — Streamline your Chaos Recipe gains";
export const size = { width: 1200, height: 630 };
export const contentType = "image/png";
export const revalidate = 3600;

const GITHUB_API =
  "https://api.github.com/repos/ChaosRecipeEnhancer/ChaosRecipeEnhancer";
const GITHUB_RELEASES_API = `${GITHUB_API}/releases?per_page=100`;

function formatCount(count: number): string {
  if (count >= 1_000_000) {
    return `${(count / 1_000_000).toFixed(1).replace(/\.0$/, "")}M`;
  }
  if (count >= 1000) {
    return `${(count / 1000).toFixed(1).replace(/\.0$/, "")}K`;
  }
  return count.toLocaleString();
}

async function getStats(): Promise<{
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
          release.assets.reduce(
            (sum, asset) => sum + asset.download_count,
            0,
          ),
        0,
      );
    }

    return { downloads, stars };
  } catch {
    return { downloads: null, stars: null };
  }
}

export default async function Image() {
  const [{ downloads, stars }, fontData, logoBuffer] = await Promise.all([
    getStats(),
    fetch(
      "https://fonts.gstatic.com/s/inter/v20/UcCO3FwrK3iLTeHuS_nVMrMxCp50SjIw2boKoduKmMEVuGKYMZg.ttf",
    ).then((res) => res.arrayBuffer()),
    fetch(
      "https://raw.githubusercontent.com/ChaosRecipeEnhancer/ChaosRecipeEnhancer/main/src/Site/public/images/cre-logo.png",
    ).then((res) => res.arrayBuffer()),
  ]);

  const logoBase64 = `data:image/png;base64,${Buffer.from(logoBuffer).toString("base64")}`;

  const hasStats = downloads !== null || stars !== null;

  return new ImageResponse(
    (
      <div
        style={{
          width: "100%",
          height: "100%",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
          justifyContent: "center",
          background: "#141414",
          position: "relative",
          overflow: "hidden",
        }}
      >
        {/* Top accent bar */}
        <div
          style={{
            position: "absolute",
            top: 0,
            left: 0,
            right: 0,
            height: "4px",
            display: "flex",
            background:
              "linear-gradient(90deg, #3494e1 0%, #d4a843 50%, #3494e1 100%)",
          }}
        />

        {/* Subtle radial glow behind content */}
        <div
          style={{
            position: "absolute",
            top: "-200px",
            left: "50%",
            width: "800px",
            height: "600px",
            display: "flex",
            borderRadius: "50%",
            background:
              "radial-gradient(ellipse, rgba(52,148,225,0.08) 0%, transparent 70%)",
            transform: "translateX(-50%)",
          }}
        />

        {/* Main content */}
        <div
          style={{
            display: "flex",
            flexDirection: "column",
            alignItems: "center",
            justifyContent: "center",
            gap: "0px",
            position: "relative",
          }}
        >
          {/* Logo */}

          <img
            src={logoBase64}
            width="120"
            height="120"
            style={{
              marginBottom: "24px",
              filter: "drop-shadow(0 4px 24px rgba(212, 168, 67, 0.3))",
            }}
          />

          {/* Title */}
          <div
            style={{
              fontSize: "64px",
              fontWeight: 600,
              color: "#f0f0f0",
              fontFamily: "Inter",
              letterSpacing: "-1px",
              lineHeight: 1.1,
              marginBottom: "12px",
            }}
          >
            Chaos Recipe Enhancer
          </div>

          {/* Tagline */}
          <div
            style={{
              fontSize: "28px",
              color: "#dcdcdc",
              fontFamily: "Inter",
              marginBottom: "40px",
            }}
          >
            Streamline your Chaos Recipe gains
          </div>

          {/* Stats row */}
          <div
            style={{
              display: "flex",
              alignItems: "center",
              gap: "48px",
            }}
          >
            {hasStats ? (
              <>
                {downloads !== null && (
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "12px",
                    }}
                  >
                    <div
                      style={{
                        fontSize: "28px",
                        color: "#bebebe",
                        fontFamily: "Inter",
                      }}
                    >
                      ↓
                    </div>
                    <div
                      style={{
                        fontSize: "36px",
                        fontWeight: 600,
                        color: "#d4a843",
                        fontFamily: "Inter",
                      }}
                    >
                      {formatCount(downloads)}
                    </div>
                    <div
                      style={{
                        fontSize: "22px",
                        color: "#bebebe",
                        fontFamily: "Inter",
                      }}
                    >
                      downloads
                    </div>
                  </div>
                )}
                {downloads !== null && stars !== null && (
                  <div
                    style={{
                      width: "2px",
                      height: "36px",
                      background: "#313131",
                      display: "flex",
                    }}
                  />
                )}
                {stars !== null && (
                  <div
                    style={{
                      display: "flex",
                      alignItems: "center",
                      gap: "12px",
                    }}
                  >
                    <div
                      style={{
                        fontSize: "28px",
                        color: "#d4a843",
                        fontFamily: "Inter",
                      }}
                    >
                      ★
                    </div>
                    <div
                      style={{
                        fontSize: "36px",
                        fontWeight: 600,
                        color: "#d4a843",
                        fontFamily: "Inter",
                      }}
                    >
                      {formatCount(stars)}
                    </div>
                    <div
                      style={{
                        fontSize: "22px",
                        color: "#bebebe",
                        fontFamily: "Inter",
                      }}
                    >
                      stars
                    </div>
                  </div>
                )}
              </>
            ) : (
              <>
                <div
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: "12px",
                  }}
                >
                  <div
                    style={{
                      fontSize: "28px",
                      color: "#d4a843",
                      fontFamily: "Inter",
                    }}
                  >
                    ✦
                  </div>
                  <div
                    style={{
                      fontSize: "28px",
                      color: "#bebebe",
                      fontFamily: "Inter",
                    }}
                  >
                    Open Source
                  </div>
                </div>
                <div
                  style={{
                    width: "2px",
                    height: "36px",
                    background: "#313131",
                    display: "flex",
                  }}
                />
                <div
                  style={{
                    display: "flex",
                    alignItems: "center",
                    gap: "12px",
                  }}
                >
                  <div
                    style={{
                      fontSize: "28px",
                      color: "#d4a843",
                      fontFamily: "Inter",
                    }}
                  >
                    ✦
                  </div>
                  <div
                    style={{
                      fontSize: "28px",
                      color: "#bebebe",
                      fontFamily: "Inter",
                    }}
                  >
                    Free Forever
                  </div>
                </div>
              </>
            )}
          </div>
        </div>

        {/* Bottom border accent */}
        <div
          style={{
            position: "absolute",
            bottom: 0,
            left: 0,
            right: 0,
            height: "4px",
            display: "flex",
            background:
              "linear-gradient(90deg, #d4a843 0%, #3494e1 50%, #d4a843 100%)",
          }}
        />

        {/* Bottom-right subtle branding */}
        <div
          style={{
            position: "absolute",
            bottom: "20px",
            right: "32px",
            fontSize: "18px",
            color: "#555555",
            fontFamily: "Inter",
            display: "flex",
          }}
        >
          chaos-recipe.com
        </div>
      </div>
    ),
    {
      ...size,
      fonts: [
        {
          name: "Inter",
          data: fontData,
          weight: 600,
          style: "normal" as const,
        },
      ],
    },
  );
}
