import type { Membership } from "@/lib/types";
import { Button } from "@/components/ui/button";
import { formatCurrency } from "@/lib/utils";

export function PlanCard({ plan, branchName }: { plan: Membership; branchName?: string }) {
  return (
    <article className="rounded-[30px] border border-white/10 bg-white/[0.04] p-6 backdrop-blur">
      <p className="text-xs font-semibold uppercase tracking-[0.26em] text-[var(--accent)]">{branchName || "Plan"}</p>
      <h3 className="mt-4 font-heading text-3xl font-semibold text-white">{plan.name}</h3>
      <p className="mt-3 text-4xl font-semibold text-white">{formatCurrency(plan.price, plan.currency)}</p>
      <p className="mt-2 text-sm text-slate-400">Vigencia de {plan.durationInDays} días con acceso a entrenamientos y comunidad.</p>
      <Button href={process.env.NEXT_PUBLIC_APP_DOWNLOAD_URL ?? "https://example.com/app"} className="mt-6 w-full justify-center">Descargar app</Button>
    </article>
  );
}
