import type { Metadata } from "next";
import { ClassCard } from "@/components/marketing/class-card";
import { GroupClassCard } from "@/components/marketing/group-class-card";
import { SectionTitle } from "@/components/ui/section-title";
import { getBranches, getClasses, getGroupClasses } from "@/lib/backend/api";

export const metadata: Metadata = {
  title: "Clases",
  description: "Revisa las clases programadas en Dorian Fitness y descubre sesiones listas para reservar desde la app.",
};

export default async function ClassesPage() {
  const [classes, branches, groupClasses] = await Promise.all([getClasses(), getBranches(), getGroupClasses()]);
  const branchMap = Object.fromEntries(branches.map((branch) => [branch.id, branch.name]));

  return (
    <main className="mx-auto max-w-7xl px-4 py-16 md:px-6">
      <SectionTitle eyebrow="Catalogo comercial" title="Clases grupales Dorian" description="Conoce el enfoque de cada disciplina antes de revisar horarios, cupos y sucursales." />
      <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-4">{groupClasses.map((item) => <GroupClassCard key={item.slug} item={item} />)}</div>

      <section className="mt-16">
        <SectionTitle eyebrow="Horario activo" title="Clases disponibles hoy y proximas" description="Cada sesion combina energia, estructura y aforo controlado para una mejor experiencia." />
        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{classes.map((item) => <ClassCard key={item.id} item={item} branchName={branchMap[item.branchId]} />)}</div>
      </section>
    </main>
  );
}
