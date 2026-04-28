import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

export function formatDateTime(value?: string | null) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("es-EC", {
    dateStyle: "medium",
    timeStyle: "short",
  }).format(new Date(value));
}

export function formatCurrency(amount: number, currency = "USD") {
  return new Intl.NumberFormat("es-EC", {
    style: "currency",
    currency,
  }).format(amount);
}

export function pickTone(index: number) {
  const tones = ["from-emerald-500/20 to-cyan-500/10", "from-orange-500/20 to-amber-500/10", "from-sky-500/20 to-indigo-500/10"];
  return tones[index % tones.length];
}