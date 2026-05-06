import type { ClassSession } from "@/lib/types";
import { CalendarClock, Users } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { formatDateTime } from "@/lib/utils";

export function ClassCard({ item, branchName }: { item: ClassSession; branchName?: string }) {
  return (
    <article className="rounded-[28px] border border-white/10 bg-white/[0.04] p-6 backdrop-blur">
      <Badge>{branchName || "Clase"}</Badge>
      <h3 className="mt-4 font-heading text-2xl font-semibold text-white">{item.name}</h3>
      <p className="mt-3 text-sm leading-6 text-slate-400">{item.description || "Sesiones guiadas para elevar rendimiento, técnica y consistencia."}</p>
      <div className="mt-5 space-y-3 text-sm text-slate-300">
        <p className="flex items-start gap-2"><CalendarClock className="mt-0.5 h-4 w-4 text-[var(--accent)]" /> {formatDateTime(item.startTime)} → {formatDateTime(item.endTime)}</p>
        <p className="flex items-center gap-2"><Users className="h-4 w-4 text-[var(--accent)]" /> {item.reservedSpots}/{item.capacity} reservados</p>
      </div>
    </article>
  );
}
