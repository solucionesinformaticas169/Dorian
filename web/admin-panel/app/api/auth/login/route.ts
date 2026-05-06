import { NextRequest, NextResponse } from "next/server";
import { getBackendUrl } from "@/lib/api/backend";
import { applyAuthCookies } from "@/lib/auth/cookies";
import type { ApiErrorPayload, AuthResponse } from "@/lib/types";

export async function POST(request: NextRequest) {
  const payload = await request.json();
  const response = await fetch(getBackendUrl("/auth/login"), {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(payload),
    cache: "no-store",
  });

  if (!response.ok) {
    const errorPayload = (await response.json().catch(() => null)) as ApiErrorPayload | null;
    return NextResponse.json(errorPayload ?? { message: "No se pudo iniciar sesión." }, { status: response.status });
  }

  const auth = (await response.json()) as AuthResponse;
  const nextResponse = NextResponse.json({
    user: auth.user,
    accessTokenExpiresAtUtc: auth.accessTokenExpiresAtUtc,
    refreshTokenExpiresAtUtc: auth.refreshTokenExpiresAtUtc,
  });
  applyAuthCookies(nextResponse.cookies, auth);
  return nextResponse;
}


