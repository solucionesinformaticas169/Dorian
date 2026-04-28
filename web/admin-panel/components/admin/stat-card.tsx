import { Card, CardDescription, CardTitle } from "@/components/ui/card";

export function StatCard({ label, value, helper }: { label: string; value: string | number; helper: string }) {
  return (
    <Card className="bg-gradient-to-br from-white/[0.08] to-white/[0.03]">
      <p className="text-xs font-semibold uppercase tracking-[0.24em] text-slate-500">{label}</p>
      <CardTitle className="mt-4 text-3xl">{value}</CardTitle>
      <CardDescription className="mt-2">{helper}</CardDescription>
    </Card>
  );
}

