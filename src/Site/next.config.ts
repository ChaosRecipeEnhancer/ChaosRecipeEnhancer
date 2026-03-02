import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  async rewrites() {
    return [
      {
        source: "/auth/success",
        destination:
          "https://d-rqtq07y5g9.execute-api.us-east-1.amazonaws.com/auth/success",
      },
      {
        source: "/auth/token",
        destination:
          "https://d-rqtq07y5g9.execute-api.us-east-1.amazonaws.com/auth/token",
      },
      {
        source: "/v2/auth/token",
        destination:
          "https://d-rqtq07y5g9.execute-api.us-east-1.amazonaws.com/v2/auth/token",
      },
    ];
  },
};

export default nextConfig;
