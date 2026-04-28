import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { REFRESH_TOKEN_COOKIE, applyAuthCookies, clearAuthCookies } from "@/lib/auth/cookies";
import { refreshTokens } from "@/lib/api/backend";

export async function POST() {
  const cookieStore = await cookies();
  const refreshToken = cookieStore.get(REFRESH_TOKEN_COOKIE)?.value;

  if (!refreshToken) {
    const response = NextResponse.json({ message: "No refresh token available." }, { status: 401 });
    clearAuthCookies(response.cookies);
    return response;
  }

  const auth = await refreshTokens(refreshToken);
  if (!auth) {
    const response = NextResponse.json({ message: "Refresh token is invalid." }, { status: 401 });
    clearAuthCookies(response.cookies);
    return response;
  }

  const response = NextResponse.json({
    user: auth.user,
    accessTokenExpiresAtUtc: auth.accessTokenExpiresAtUtc,
    refreshTokenExpiresAtUtc: auth.refreshTokenExpiresAtUtc,
  });
  applyAuthCookies(response.cookies, auth);
  return response;
}

