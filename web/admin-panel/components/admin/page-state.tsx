import { Card } from "@/components/ui/card";

export function LoadingState({ label = "Cargando modulo..." }: { label?: string }) {
  return (
    <Card className="flex min-h-[220px] items-center justify-center text-slate-300">
      <div className="space-y-3 text-center">
        <div className="mx-auto h-10 w-10 animate-spin rounded-full border-2 border-[var(--accent)] border-t-transparent" />
        <p>{label}</p>
      </div>
    </Card>
  );
}

export function EmptyState({ title, description }: { title: string; description: string }) {
  return (
    <Card className="border-dashed border-white/15 text-center">
      <div className="mx-auto max-w-md space-y-3 py-10">
        <h3 className="font-heading text-xl font-semibold text-white">{title}</h3>
        <p className="text-sm text-slate-400">{description}</p>
      </div>
    </Card>
  );
}

