import { cookies } from "next/headers";
import { NextResponse } from "next/server";
import { SESSION_COOKIE, parseSessionCookie } from "@/lib/auth/cookies";

export async function GET() {
  const cookieStore = await cookies();
  const session = parseSessionCookie(cookieStore.get(SESSION_COOKIE)?.value);

  if (!session) {
    return NextResponse.json({ message: "No active session." }, { status: 401 });
  }

  return NextResponse.json(session);
}

