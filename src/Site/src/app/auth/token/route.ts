const API_GATEWAY_BASE =
  "https://d-rqtq07y5g9.execute-api.us-east-1.amazonaws.com";

export async function POST(request: Request) {
  const body = await request.text();
  const upstream = `${API_GATEWAY_BASE}/auth/token`;

  const response = await fetch(upstream, {
    method: "POST",
    headers: {
      "Content-Type":
        request.headers.get("content-type") ??
        "application/x-www-form-urlencoded",
      "User-Agent": request.headers.get("user-agent") ?? "",
    },
    body,
  });

  const responseBody = await response.text();

  return new Response(responseBody, {
    status: response.status,
    headers: {
      "Content-Type":
        response.headers.get("content-type") ?? "application/json",
    },
  });
}
