import type { Metadata } from "next";
import { PlanCard } from "@/components/marketing/plan-card";
import { SectionTitle } from "@/components/ui/section-title";
import { getBranches, getMemberships } from "@/lib/backend/api";

export const metadata: Metadata = {
  title: "Planes",
  description: "Descubre los planes y membresías activas de Dorian Fitness para comenzar a entrenar.",
};

export default async function PlansPage() {
  const [memberships, branches] = await Promise.all([getMemberships(), getBranches()]);
  const branchMap = Object.fromEntries(branches.map((branch) => [branch.id, branch.name]));

  return (
    <main className="mx-auto max-w-7xl px-4 py-16 md:px-6">
      <SectionTitle eyebrow="Membresías" title="Planes activos" description="Opciones pensadas para sostener constancia, acceso y progreso real." />
      <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{memberships.map((plan) => <PlanCard key={plan.id} plan={plan} branchName={branchMap[plan.branchId]} />)}</div>
    </main>
  );
}
