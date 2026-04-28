export function SectionTitle({ eyebrow, title, description }: { eyebrow?: string; title: string; description?: string }) {
  return (
    <div className="max-w-3xl space-y-3">
      {eyebrow ? <p className="text-xs font-semibold uppercase tracking-[0.34em] text-[var(--accent)]">{eyebrow}</p> : null}
      <h2 className="font-heading text-4xl font-semibold tracking-tight text-white md:text-5xl">{title}</h2>
      {description ? <p className="text-base leading-7 text-slate-400 md:text-lg">{description}</p> : null}
    </div>
  );
}