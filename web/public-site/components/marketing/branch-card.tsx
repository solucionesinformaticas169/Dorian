import { Clock3, MapPin, Phone } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import type { Branch } from "@/lib/types";

function getFallbackMapUrl(branch: Branch) {
  const query = [branch.address, branch.city, "Ecuador"].filter(Boolean).join(", ");
  return `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(query)}`;
}

export function BranchCard({ branch }: { branch: Branch }) {
  return (
    <article className="rounded-[28px] border border-white/10 bg-white/[0.04] p-6 backdrop-blur">
      <Badge>{branch.code}</Badge>
      <h3 className="mt-4 font-heading text-2xl font-semibold text-white">{branch.name}</h3>
      <div className="mt-4 space-y-3 text-sm text-slate-300">
        <p className="flex items-start gap-2">
          <MapPin className="mt-0.5 h-4 w-4 text-[var(--accent)]" />
          {branch.address ? `${branch.address}, ${branch.city}` : branch.city}
        </p>
        <p className="flex items-center gap-2">
          <Phone className="h-4 w-4 text-[var(--accent)]" />
          {branch.phoneNumber || "Contacto en actualizacion"}
        </p>
        <p className="flex items-center gap-2">
          <Clock3 className="h-4 w-4 text-[var(--accent)]" />
          {branch.openingHours || "Horario por confirmar"}
        </p>
      </div>
      <div className="mt-5">
        <Button href={branch.mapUrl || getFallbackMapUrl(branch)} className="w-full justify-center">
          Ver en mapa
        </Button>
      </div>
    </article>
  );
}
