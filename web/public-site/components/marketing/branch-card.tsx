import type { Branch } from "@/lib/types";
import { MapPin, Phone } from "lucide-react";
import { Badge } from "@/components/ui/badge";

export function BranchCard({ branch }: { branch: Branch }) {
  return (
    <article className="rounded-[28px] border border-white/10 bg-white/[0.04] p-6 backdrop-blur">
      <Badge>{branch.code}</Badge>
      <h3 className="mt-4 font-heading text-2xl font-semibold text-white">{branch.name}</h3>
      <div className="mt-4 space-y-3 text-sm text-slate-300">
        <p className="flex items-start gap-2"><MapPin className="mt-0.5 h-4 w-4 text-[var(--accent)]" /> {branch.city}{branch.address ? ` · ${branch.address}` : ""}</p>
        <p className="flex items-center gap-2"><Phone className="h-4 w-4 text-[var(--accent)]" /> {branch.phoneNumber || "Contacto en actualizacion"}</p>
      </div>
    </article>
  );
}