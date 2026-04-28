import type { ApiErrorPayload } from "@/lib/types";

export class ApiError extends Error {
  status: number;
  payload?: ApiErrorPayload;

  constructor(message: string, status: number, payload?: ApiErrorPayload) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.payload = payload;
  }
}

async function parseResponse<T>(response: Response): Promise<T> {
  if (response.status === 204) return undefined as T;
  const contentType = response.headers.get("content-type") ?? "";
  if (contentType.includes("application/json")) {
    return (await response.json()) as T;
  }
  return (await response.text()) as T;
}

export async function apiFetch<T>(path: string, init?: RequestInit) {
  const response = await fetch(`/api/backend${path}`, {
    ...init,
    credentials: "include",
    cache: "no-store",
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    const payload = await parseResponse<ApiErrorPayload>(response).catch(() => undefined);
    throw new ApiError(payload?.message ?? payload?.title ?? "No se pudo completar la solicitud.", response.status, payload);
  }

  return parseResponse<T>(response);
}

export async function authFetch<T>(path: string, init?: RequestInit) {
  const response = await fetch(`/api/auth${path}`, {
    ...init,
    credentials: "include",
    cache: "no-store",
    headers: {
      "Content-Type": "application/json",
      ...(init?.headers ?? {}),
    },
  });

  if (!response.ok) {
    const payload = await parseResponse<ApiErrorPayload>(response).catch(() => undefined);
    throw new ApiError(payload?.message ?? payload?.title ?? "No se pudo completar la autenticacion.", response.status, payload);
  }

  return parseResponse<T>(response);
}

