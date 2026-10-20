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

function encodeBase64Url(value: string) {
  const bytes = new TextEncoder().encode(value);
  let binary = "";
  for (const byte of bytes) {
    binary += String.fromCharCode(byte);
  }

  return btoa(binary).replace(/\+/g, "-").replace(/\//g, "_").replace(/=+$/g, "");
}

function decodeBase64Url(value: string) {
  const normalized = value.replace(/-/g, "+").replace(/_/g, "/");
  const padding = normalized.length % 4 === 0 ? "" : "=".repeat(4 - (normalized.length % 4));
  const binary = atob(`${normalized}${padding}`);
  const bytes = Uint8Array.from(binary, (character) => character.charCodeAt(0));
  return new TextDecoder().decode(bytes);
}

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

  cookies.set(SESSION_COOKIE, encodeBase64Url(JSON.stringify(session)), {
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
    return JSON.parse(decodeBase64Url(value)) as Session;
  } catch {
    return null;
  }
}

