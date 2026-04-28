import { cn } from "@/lib/utils";

export function Badge({ className, tone = "neutral", ...props }: React.HTMLAttributes<HTMLSpanElement> & { tone?: "neutral" | "success" | "warning" | "danger" }) {
  const toneMap = {
    neutral: "bg-white/8 text-slate-300",
    success: "bg-emerald-500/15 text-emerald-300",
    warning: "bg-amber-500/15 text-amber-300",
    danger: "bg-rose-500/15 text-rose-300",
  };

  return <span className={cn("inline-flex rounded-full px-3 py-1 text-xs font-semibold", toneMap[tone], className)} {...props} />;
}

