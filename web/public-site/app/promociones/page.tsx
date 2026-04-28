import type { Metadata } from "next";
import { PromotionCard } from "@/components/marketing/promotion-card";
import { SectionTitle } from "@/components/ui/section-title";
import { getBranches, getPromotions } from "@/lib/backend/api";

export const metadata: Metadata = {
  title: "Promociones",
  description: "Consulta las promociones activas de Dorian Fitness y aprovecha beneficios vigentes por sucursal o globales.",
};

export default async function PromotionsPage() {
  const [promotions, branches] = await Promise.all([getPromotions(), getBranches()]);
  const branchMap = Object.fromEntries(branches.map((branch) => [branch.id, branch.name]));

  return (
    <main className="mx-auto max-w-7xl px-4 py-16 md:px-6">
      <SectionTitle eyebrow="Ofertas vigentes" title="Promociones activas" description="Campañas reales del backend, filtradas por estado y vigencia actual." />
      <div className="mt-10 grid gap-5 md:grid-cols-2">{promotions.map((promotion) => <PromotionCard key={promotion.id} promotion={promotion} branchName={promotion.branchId ? branchMap[promotion.branchId] : undefined} />)}</div>
    </main>
  );
}