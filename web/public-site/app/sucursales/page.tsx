import type { Metadata } from "next";
import { BranchCard } from "@/components/marketing/branch-card";
import { SectionTitle } from "@/components/ui/section-title";
import { getBranches } from "@/lib/backend/api";

export const metadata: Metadata = {
  title: "Sucursales",
  description: "Explora las sucursales activas de Dorian Fitness y encuentra la mas cercana para entrenar.",
};

export default async function BranchesPage() {
  const branches = await getBranches();
  return (
    <main className="mx-auto max-w-7xl px-4 py-16 md:px-6">
      <SectionTitle eyebrow="Nuestra red" title="Sucursales activas" description="Operamos espacios modernos para entrenar con intensidad, comodidad y continuidad." />
      <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{branches.map((branch) => <BranchCard key={branch.id} branch={branch} />)}</div>
    </main>
  );
}