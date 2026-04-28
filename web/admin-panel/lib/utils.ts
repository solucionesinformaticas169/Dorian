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

export function formatDate(value?: string | null) {
  if (!value) return "-";
  return new Intl.DateTimeFormat("es-EC", {
    dateStyle: "medium",
  }).format(new Date(value));
}

export function formatCurrency(amount: number, currency = "USD") {
  return new Intl.NumberFormat("es-EC", {
    style: "currency",
    currency,
  }).format(amount);
}

export function toDateInput(value?: string | null) {
  if (!value) return "";
  return new Date(value).toISOString().slice(0, 10);
}

export function toDateTimeLocalInput(value?: string | null) {
  if (!value) return "";
  const date = new Date(value);
  const year = date.getFullYear();
  const month = `${date.getMonth() + 1}`.padStart(2, "0");
  const day = `${date.getDate()}`.padStart(2, "0");
  const hours = `${date.getHours()}`.padStart(2, "0");
  const minutes = `${date.getMinutes()}`.padStart(2, "0");
  return `${year}-${month}-${day}T${hours}:${minutes}`;
}

export function dateInputToIso(value?: string, endOfDay = false) {
  if (!value) return undefined;
  const suffix = endOfDay ? "T23:59:59" : "T00:00:00";
  return new Date(`${value}${suffix}`).toISOString();
}

export function dateTimeLocalToIso(value?: string) {
  return value ? new Date(value).toISOString() : undefined;
}

export function getErrorMessage(error: unknown) {
  if (error instanceof Error) return error.message;
  return "Ocurrio un error inesperado.";
}

