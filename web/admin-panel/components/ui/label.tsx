import { cn } from "@/lib/utils";

export function Label({ className, ...props }: React.LabelHTMLAttributes<HTMLLabelElement>) {
  return <label className={cn("mb-2 block text-xs font-semibold uppercase tracking-[0.24em] text-slate-400", className)} {...props} />;
}

