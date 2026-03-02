const API_GATEWAY_BASE =
  "https://d-rqtq07y5g9.execute-api.us-east-1.amazonaws.com";

export async function GET(request: Request) {
  const { search } = new URL(request.url);
  const upstream = `${API_GATEWAY_BASE}/auth/success${search}`;

  const response = await fetch(upstream, {
    method: "GET",
    headers: {
      "User-Agent": request.headers.get("user-agent") ?? "",
    },
  });

  return new Response(response.body, {
    status: response.status,
    headers: {
      "Content-Type": response.headers.get("content-type") ?? "text/html",
    },
  });
}
