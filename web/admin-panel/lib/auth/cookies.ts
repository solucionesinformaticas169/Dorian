import type { ResponseCookies } from "next/dist/compiled/@edge-runtime/cookies";
import type { AuthResponse, Session } from "@/lib/types";

export const ACCESS_TOKEN_COOKIE = "dorian_access_token";
export const REFRESH_TOKEN_COOKIE = "dorian_refresh_token";
export const SESSION_COOKIE = "dorian_session";

const baseCookieOptions = {
  httpOnly: true,
  sameSite: "lax" as const,
  secure: process.env.NODE_ENV === "production",
  path: "/",
};

export function applyAuthCookies(cookies: ResponseCookies, auth: AuthResponse) {
  cookies.set(ACCESS_TOKEN_COOKIE, auth.accessToken, {
    ...baseCookieOptions,
    expires: new Date(auth.accessTokenExpiresAtUtc),
  });

  cookies.set(REFRESH_TOKEN_COOKIE, auth.refreshToken, {
    ...baseCookieOptions,
    expires: new Date(auth.refreshTokenExpiresAtUtc),
  });

  const session: Session = {
    user: auth.user,
    accessTokenExpiresAtUtc: auth.accessTokenExpiresAtUtc,
    refreshTokenExpiresAtUtc: auth.refreshTokenExpiresAtUtc,
  };

  cookies.set(SESSION_COOKIE, Buffer.from(JSON.stringify(session)).toString("base64url"), {
    ...baseCookieOptions,
    expires: new Date(auth.refreshTokenExpiresAtUtc),
  });
}

export function clearAuthCookies(cookies: ResponseCookies) {
  cookies.delete(ACCESS_TOKEN_COOKIE);
  cookies.delete(REFRESH_TOKEN_COOKIE);
  cookies.delete(SESSION_COOKIE);
}

export function parseSessionCookie(value?: string) {
  if (!value) return null;

  try {
    return JSON.parse(Buffer.from(value, "base64url").toString("utf-8")) as Session;
  } catch {
    return null;
  }
}

