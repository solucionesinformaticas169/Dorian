import { cookies } from "next/headers";
import { redirect } from "next/navigation";
import { SESSION_COOKIE, parseSessionCookie } from "@/lib/auth/cookies";

export async function getServerSession() {
  const cookieStore = await cookies();
  return parseSessionCookie(cookieStore.get(SESSION_COOKIE)?.value);
}

export async function requireServerSession() {
  const session = await getServerSession();
  if (!session) redirect("/login");
  return session;
}

