import "server-only";
import type { AuthResponse, Branch, ClassSession, GroupClass, Membership, Promotion } from "@/lib/types";

const apiUrl = (process.env.BACKEND_API_URL ?? "http://localhost:5000").trim();
const serviceEmail = process.env.BACKEND_SERVICE_EMAIL ?? "superadmin@dorian.test";
const servicePassword = process.env.BACKEND_SERVICE_PASSWORD ?? "Pass1234!";

let tokenCache: { token: string; expiresAt: number } | null = null;

async function loginServiceUser() {
  const response = await fetch(`${apiUrl}/auth/login`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ email: serviceEmail, password: servicePassword }),
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error("No se pudo autenticar la cuenta tecnica contra el backend.");
  }

  const payload = (await response.json()) as AuthResponse;
  tokenCache = {
    token: payload.accessToken,
    expiresAt: new Date(payload.accessTokenExpiresAtUtc).getTime() - 15_000,
  };

  return payload.accessToken;
}

async function getAccessToken() {
  if (tokenCache && tokenCache.expiresAt > Date.now()) {
    return tokenCache.token;
  }

  return loginServiceUser();
}

async function backendFetch<T>(path: string) {
  const token = await getAccessToken();
  const response = await fetch(`${apiUrl}${path}`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
    cache: "no-store",
  });

  if (!response.ok) {
    throw new Error(`No se pudo obtener ${path} desde el backend.`);
  }

  return (await response.json()) as T;
}

export async function getBranches() {
  const branches = await backendFetch<Branch[]>("/branches");
  return branches.filter((branch) => branch.isActive);
}

export async function getClasses() {
  const classes = await backendFetch<ClassSession[]>("/classes");
  return classes.filter((item) => item.status === 1);
}

export async function getPromotions() {
  const promotions = await backendFetch<Promotion[]>("/promotions");
  const now = Date.now();
  return promotions.filter((promotion) => promotion.status === 2 && new Date(promotion.startsAt).getTime() <= now && new Date(promotion.endsAt).getTime() >= now);
}

export async function getMemberships() {
  const memberships = await backendFetch<Membership[]>("/memberships");
  return memberships.filter((membership) => membership.isActive);
}

export async function getGroupClasses() {
  return backendFetch<GroupClass[]>("/group-classes");
}

export async function getPublicData() {
  const [branches, classes, promotions, memberships, groupClasses] = await Promise.all([
    getBranches(),
    getClasses(),
    getPromotions(),
    getMemberships(),
    getGroupClasses(),
  ]);

  return { branches, classes, promotions, memberships, groupClasses };
}
