import { ArrowRight, ShieldCheck, Sparkles, Trophy, Waves } from "lucide-react";
import { BranchCard } from "@/components/marketing/branch-card";
import { ClassCard } from "@/components/marketing/class-card";
import { PlanCard } from "@/components/marketing/plan-card";
import { PromotionCard } from "@/components/marketing/promotion-card";
import { Button } from "@/components/ui/button";
import { SectionTitle } from "@/components/ui/section-title";
import { getPublicData } from "@/lib/backend/api";
import { pickTone } from "@/lib/utils";

const benefits = [
  { icon: Trophy, title: "Entrenamiento con criterio", description: "Programas y clases pensados para mejorar rendimiento, salud y consistencia." },
  { icon: ShieldCheck, title: "Acceso inteligente", description: "Check-ins QR, reservas de clases y operacion conectada a tu cuenta." },
  { icon: Sparkles, title: "Experiencia premium", description: "Espacios cuidados, coaches cercanos y una marca con energia contemporanea." },
];

export default async function HomePage() {
  const { branches, classes, promotions, memberships } = await getPublicData();
  const branchMap = Object.fromEntries(branches.map((branch) => [branch.id, branch.name]));

  return (
    <main>
      <section className="relative overflow-hidden">
        <div className="mx-auto grid max-w-7xl gap-10 px-4 py-20 md:px-6 lg:grid-cols-[1.1fr_0.9fr] lg:py-28">
          <div className="space-y-8">
            <p className="text-xs font-semibold uppercase tracking-[0.38em] text-[var(--accent)]">Fitness premium, conectado</p>
            <h1 className="font-heading text-5xl font-semibold leading-none text-white md:text-7xl">Fuerza, energia y comunidad en una sola membresia.</h1>
            <p className="max-w-2xl text-lg leading-8 text-slate-300">Dorian Fitness combina entrenamiento funcional, clases dirigidas y tecnologia para que tu progreso se vea y se sienta desde el primer dia.</p>
            <div className="flex flex-col gap-4 sm:flex-row">
              <Button href={process.env.NEXT_PUBLIC_APP_DOWNLOAD_URL ?? "https://example.com/app"} className="gap-2">Descargar app <ArrowRight className="h-4 w-4" /></Button>
              <Button href="/planes" variant="secondary">Ver planes</Button>
            </div>
            <div className="grid gap-4 md:grid-cols-3">
              <div className="rounded-[28px] border border-white/10 bg-white/[0.04] p-5"><p className="text-sm text-slate-400">Sucursales activas</p><p className="mt-2 font-heading text-3xl text-white">{branches.length}</p></div>
              <div className="rounded-[28px] border border-white/10 bg-white/[0.04] p-5"><p className="text-sm text-slate-400">Clases programadas</p><p className="mt-2 font-heading text-3xl text-white">{classes.length}</p></div>
              <div className="rounded-[28px] border border-white/10 bg-white/[0.04] p-5"><p className="text-sm text-slate-400">Promociones vigentes</p><p className="mt-2 font-heading text-3xl text-white">{promotions.length}</p></div>
            </div>
          </div>
          <div className="grid gap-4">
            <div className="rounded-[36px] border border-white/10 bg-gradient-to-br from-emerald-500/15 via-slate-950/70 to-sky-500/10 p-6">
              <div className="flex items-center gap-3 text-[var(--accent)]"><Waves className="h-5 w-5" /><span className="text-xs font-semibold uppercase tracking-[0.26em]">High-performance club</span></div>
              <h2 className="mt-6 font-heading text-3xl font-semibold text-white">Reserva clases, entra con QR y lleva tu entrenamiento contigo.</h2>
              <p className="mt-4 text-sm leading-7 text-slate-300">Una experiencia creada para miembros que quieren estructura, flexibilidad y una marca que los motive a volver.</p>
            </div>
            {promotions.slice(0, 2).map((promotion) => <PromotionCard key={promotion.id} promotion={promotion} branchName={promotion.branchId ? branchMap[promotion.branchId] : undefined} />)}
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-16 md:px-6">
        <SectionTitle eyebrow="Por que Dorian" title="Un gimnasio pensado para quedarse en tu rutina" description="Espacios de entrenamiento, operacion moderna y un ecosistema digital que respalda la experiencia." />
        <div className="mt-10 grid gap-5 md:grid-cols-3">
          {benefits.map((benefit, index) => {
            const Icon = benefit.icon;
            return (
              <article key={benefit.title} className={`rounded-[28px] border border-white/10 bg-gradient-to-br ${pickTone(index)} p-6`}>
                <div className="inline-flex rounded-2xl bg-white/10 p-3 text-[var(--accent)]"><Icon className="h-5 w-5" /></div>
                <h3 className="mt-5 font-heading text-2xl font-semibold text-white">{benefit.title}</h3>
                <p className="mt-3 text-sm leading-7 text-slate-300">{benefit.description}</p>
              </article>
            );
          })}
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-16 md:px-6">
        <SectionTitle eyebrow="Sucursales" title="Entrena donde mejor te quede" description="Consulta las sedes disponibles y elige la que mejor se adapte a tu ritmo de vida." />
        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{branches.slice(0, 3).map((branch) => <BranchCard key={branch.id} branch={branch} />)}</div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-16 md:px-6">
        <SectionTitle eyebrow="Clases" title="Agenda sesiones que elevan tu energia" description="Descubre clases dirigidas con aforo controlado, horarios claros y una experiencia lista para reservar desde la app." />
        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{classes.slice(0, 3).map((item) => <ClassCard key={item.id} item={item} branchName={branchMap[item.branchId]} />)}</div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-16 md:px-6">
        <SectionTitle eyebrow="Planes" title="Membresias claras para entrenar mejor" description="Elige una opcion que acompañe tu frecuencia, tu sede y tu forma de progresar." />
        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{memberships.slice(0, 3).map((plan) => <PlanCard key={plan.id} plan={plan} branchName={branchMap[plan.branchId]} />)}</div>
      </section>
    </main>
  );
}