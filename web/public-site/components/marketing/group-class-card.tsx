import { Badge } from "@/components/ui/badge";
import type { GroupClass } from "@/lib/types";

export function GroupClassCard({ item }: { item: GroupClass }) {
  return (
    <article className="rounded-[30px] border border-white/10 bg-[linear-gradient(180deg,rgba(255,106,31,0.14),rgba(255,255,255,0.03))] p-6 backdrop-blur">
      <div className="flex items-center justify-between gap-4">
        <Badge>{item.type}</Badge>
        <span className="text-3xl" aria-hidden="true">{item.emoji}</span>
      </div>
      <p className="mt-4 text-xs font-semibold uppercase tracking-[0.26em] text-[var(--accent)]">{item.subtitle}</p>
      <h3 className="mt-3 font-heading text-3xl font-semibold text-white">{item.name}</h3>
      <p className="mt-3 text-sm leading-7 text-slate-300">{item.description}</p>
      <p className="mt-4 text-sm font-medium text-[var(--accent-strong)]">{item.tagline}</p>
      <div className="mt-5 flex flex-wrap gap-2">
        {item.benefits.map((benefit) => (
          <span key={benefit} className="rounded-full border border-white/10 bg-black/30 px-3 py-1 text-xs uppercase tracking-[0.18em] text-slate-200">
            {benefit}
          </span>
        ))}
      </div>
    </article>
  );
}
