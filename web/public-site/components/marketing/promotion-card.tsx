import type { Promotion } from "@/lib/types";
import { Badge } from "@/components/ui/badge";
import { formatDateTime } from "@/lib/utils";

const discountLabels: Record<number, string> = {
  1: "Descuento %",
  2: "Monto fijo",
  3: "Beneficio especial",
};

export function PromotionCard({ promotion, branchName }: { promotion: Promotion; branchName?: string }) {
  return (
    <article className="rounded-[30px] border border-emerald-400/15 bg-gradient-to-br from-emerald-500/10 via-slate-950/80 to-sky-500/5 p-6">
      <div className="flex items-start justify-between gap-4">
        <div>
          <Badge>{branchName || "Global"}</Badge>
          <h3 className="mt-4 font-heading text-2xl font-semibold text-white">{promotion.title}</h3>
        </div>
        <div className="rounded-full bg-white/8 px-4 py-2 text-sm font-semibold text-[var(--accent)]">
          {promotion.discountValue ? `${promotion.discountValue}${promotion.discountType === 1 ? "%" : " USD"}` : discountLabels[promotion.discountType]}
        </div>
      </div>
      <p className="mt-4 text-sm leading-6 text-slate-300">{promotion.description}</p>
      <p className="mt-5 text-xs uppercase tracking-[0.2em] text-slate-500">Vigente hasta {formatDateTime(promotion.endsAt)}</p>
    </article>
  );
}