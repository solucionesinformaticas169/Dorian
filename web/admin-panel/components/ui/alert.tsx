import { AlertTriangle } from "lucide-react";
import { cn } from "@/lib/utils";

export function Alert({ className, children }: { className?: string; children: React.ReactNode }) {
  return (
    <div className={cn("flex items-start gap-3 rounded-2xl border border-rose-500/30 bg-rose-500/10 px-4 py-3 text-sm text-rose-100", className)}>
      <AlertTriangle className="mt-0.5 h-4 w-4 shrink-0" />
      <div>{children}</div>
    </div>
  );
}

