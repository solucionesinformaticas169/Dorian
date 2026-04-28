import { cn } from "@/lib/utils";

export function Badge({ children, className }: { children: React.ReactNode; className?: string }) {
  return <span className={cn("inline-flex rounded-full bg-white/8 px-3 py-1 text-xs font-semibold uppercase tracking-[0.2em] text-slate-200", className)}>{children}</span>;
}