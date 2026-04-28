import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { ACCESS_TOKEN_COOKIE, REFRESH_TOKEN_COOKIE, clearAuthCookies } from "@/lib/auth/cookies";
import { callBackend } from "@/lib/api/backend";

export async function POST() {
  const cookieStore = await cookies();
  const accessToken = cookieStore.get(ACCESS_TOKEN_COOKIE)?.value;
  const refreshToken = cookieStore.get(REFRESH_TOKEN_COOKIE)?.value;

  if (refreshToken) {
    await callBackend("/auth/logout", {
      method: "POST",
      body: JSON.stringify({ refreshToken }),
    }, accessToken).catch(() => null);
  }

  const response = NextResponse.json({ ok: true });
  clearAuthCookies(response.cookies);
  return response;
}

