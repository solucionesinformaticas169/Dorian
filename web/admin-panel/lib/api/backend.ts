import type { NextRequest } from "next/server";
import { NextResponse } from "next/server";
import { ACCESS_TOKEN_COOKIE, REFRESH_TOKEN_COOKIE, applyAuthCookies, clearAuthCookies } from "@/lib/auth/cookies";
import type { ApiErrorPayload, AuthResponse } from "@/lib/types";

const backendBaseUrl = process.env.BACKEND_API_URL ?? "http://localhost:5000";

export function getBackendUrl(path: string, search = "") {
  return `${backendBaseUrl}${path}${search}`;
}

export async function callBackend(path: string, init: RequestInit = {}, accessToken?: string) {
  const headers = new Headers(init.headers);
  if (accessToken) headers.set("Authorization", `Bearer ${accessToken}`);
  if (init.body && !headers.has("Content-Type")) headers.set("Content-Type", "application/json");
  return fetch(getBackendUrl(path), {
    ...init,
    headers,
    cache: "no-store",
  });
}

export async function refreshTokens(refreshToken: string) {
  const response = await callBackend("/auth/refresh", {
    method: "POST",
    body: JSON.stringify({ refreshToken }),
  });

  if (!response.ok) return null;
  return (await response.json()) as AuthResponse;
}

export async function getRequestBody(request: NextRequest) {
  if (request.method === "GET" || request.method === "HEAD") return undefined;
  const text = await request.text();
  return text || undefined;
}

export async function proxyBackendRequest(request: NextRequest, pathSegments: string[]) {
  const cookieStore = request.cookies;
  const accessToken = cookieStore.get(ACCESS_TOKEN_COOKIE)?.value;
  const refreshToken = cookieStore.get(REFRESH_TOKEN_COOKIE)?.value;
  const backendPath = `/${pathSegments.join("/")}`;
  const body = await getRequestBody(request);

  let response = await callBackend(`${backendPath}${request.nextUrl.search}`, {
    method: request.method,
    body,
  }, accessToken);

  let refreshedAuth: AuthResponse | null = null;
  if (response.status === 401 && refreshToken) {
    refreshedAuth = await refreshTokens(refreshToken);
    if (refreshedAuth) {
      response = await callBackend(`${backendPath}${request.nextUrl.search}`, {
        method: request.method,
        body,
      }, refreshedAuth.accessToken);
    }
  }

  const responseText = await response.text();
  const nextResponse = new NextResponse(responseText, {
    status: response.status,
    headers: {
      "Content-Type": response.headers.get("Content-Type") ?? "application/json",
    },
  });

  if (refreshedAuth) {
    applyAuthCookies(nextResponse.cookies, refreshedAuth);
  }

  if (response.status === 401 && !refreshedAuth) {
    clearAuthCookies(nextResponse.cookies);
  }

  return nextResponse;
}

export function buildErrorResponse(response: Response, payload: ApiErrorPayload | null) {
  return NextResponse.json(payload ?? { message: "Request failed." }, { status: response.status });
}

