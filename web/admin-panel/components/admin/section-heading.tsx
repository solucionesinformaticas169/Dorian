import { CardDescription, CardTitle } from "@/components/ui/card";

export function SectionHeading({ eyebrow, title, description }: { eyebrow?: string; title: string; description?: string }) {
  return (
    <div className="space-y-2">
      {eyebrow ? <p className="text-xs font-semibold uppercase tracking-[0.32em] text-[var(--accent)]">{eyebrow}</p> : null}
      <CardTitle className="text-2xl">{title}</CardTitle>
      {description ? <CardDescription>{description}</CardDescription> : null}
    </div>
  );
}

