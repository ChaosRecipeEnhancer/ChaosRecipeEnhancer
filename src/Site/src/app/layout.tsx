import type { Metadata } from "next";
import { Inter } from "next/font/google";
import "./globals.css";

const inter = Inter({ subsets: ["latin"], variable: "--font-inter" });

export const metadata: Metadata = {
  title: "Chaos Recipe Enhancer — Streamline Your PoE Recipe Farm",
  description:
    "Track chaos, regal, exalted & chance recipes. Smart stash overlays. Automatic loot filter manipulation.",
  metadataBase: new URL("https://chaos-recipe.com"),
  openGraph: {
    title: "Chaos Recipe Enhancer",
    description: "Streamline your Path of Exile recipe farming",
    url: "https://chaos-recipe.com",
    siteName: "Chaos Recipe Enhancer",
    images: [
      {
        url: "/images/video-thumbnail.png",
        width: 1280,
        height: 720,
        alt: "Chaos Recipe Enhancer",
      },
    ],
    locale: "en_US",
    type: "website",
  },
  alternates: {
    canonical: "https://chaos-recipe.com",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html className="dark" lang="en">
      <body className={`${inter.variable} font-sans`}>{children}</body>
    </html>
  );
}
