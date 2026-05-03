import Image from "next/image";
import { ArrowRight, ShieldCheck, Sparkles, Trophy, Waves } from "lucide-react";
import { BranchCard } from "@/components/marketing/branch-card";
import { GroupClassCard } from "@/components/marketing/group-class-card";
import { PlanCard } from "@/components/marketing/plan-card";
import { PromotionCard } from "@/components/marketing/promotion-card";
import { Button } from "@/components/ui/button";
import { SectionTitle } from "@/components/ui/section-title";
import { getPublicData } from "@/lib/backend/api";
import { pickTone } from "@/lib/utils";

const benefits = [
  { icon: Trophy, title: "Entrenamiento con criterio", description: "Programas y clases pensados para mejorar rendimiento, salud y consistencia." },
  { icon: ShieldCheck, title: "Acceso inteligente", description: "Check-ins QR, reservas de clases y operacion conectada a tu cuenta." },
  { icon: Sparkles, title: "Experiencia premium", description: "Espacios cuidados, coaches cercanos y una marca local con energia contemporanea." },
];

export default async function HomePage() {
  const { branches, classes, promotions, memberships, groupClasses } = await getPublicData();
  const branchMap = Object.fromEntries(branches.map((branch) => [branch.id, branch.name]));

  return (
    <main>
      <section className="relative overflow-hidden">
        <div className="mx-auto grid max-w-7xl gap-10 px-4 py-20 md:px-6 lg:grid-cols-[1.1fr_0.9fr] lg:py-28">
          <div className="space-y-8">
            <div className="inline-flex items-center gap-3 rounded-full border border-white/10 bg-white/[0.04] px-4 py-2 text-[var(--accent)]">
              <Image src="/brand/dorian-logo.png" alt="Gimnasio Dorian" width={26} height={26} className="h-auto w-[26px]" priority />
              <span className="text-xs font-semibold uppercase tracking-[0.38em]">Gimnasio Dorian</span>
            </div>
            <h1 className="font-heading text-5xl font-semibold leading-none text-white md:text-7xl">Entrena con la energia Dorian en cada sucursal.</h1>
            <p className="max-w-2xl text-lg leading-8 text-slate-300">Dorian combina entrenamiento funcional, clases dirigidas y tecnologia para que tu progreso se vea y se sienta desde el primer dia en Cuenca y Azogues.</p>
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
            <div className="rounded-[36px] border border-white/10 bg-gradient-to-br from-[var(--accent)]/18 via-[#130b08]/90 to-[#3d1a0d]/60 p-6">
              <div className="flex items-center gap-3 text-[var(--accent)]"><Waves className="h-5 w-5" /><span className="text-xs font-semibold uppercase tracking-[0.26em]">Club premium conectado</span></div>
              <h2 className="mt-6 font-heading text-3xl font-semibold text-white">Reserva clases, entra con QR y lleva tu entrenamiento contigo.</h2>
              <p className="mt-4 text-sm leading-7 text-slate-300">Una experiencia creada para miembros que quieren estructura, flexibilidad y una marca local que los motive a volver.</p>
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
        <SectionTitle eyebrow="Nuestras sucursales" title="Encuentra tu sede Dorian" description="Consulta las sedes reales de Dorian y abre cada ubicacion directamente en Google Maps." />
        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{branches.map((branch) => <BranchCard key={branch.id} branch={branch} />)}</div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-16 md:px-6">
        <SectionTitle eyebrow="Clases grupales" title="Disciplinas con identidad propia" description="Dorian combina energia comercial y agenda real para ayudarte a descubrir la clase ideal antes de reservar." />
        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-4">{groupClasses.map((item) => <GroupClassCard key={item.slug} item={item} />)}</div>
        <div className="mt-8 rounded-[30px] border border-white/10 bg-white/[0.03] p-6">
          <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Agenda activa</p>
          <div className="mt-4 grid gap-4 md:grid-cols-3">
            {classes.slice(0, 3).map((item) => (
              <div key={item.id} className="rounded-[24px] border border-white/10 bg-black/20 p-4">
                <p className="font-semibold text-white">{item.name}</p>
                <p className="mt-2 text-sm text-slate-400">{branchMap[item.branchId]}</p>
                <p className="mt-3 text-xs uppercase tracking-[0.22em] text-slate-500">{new Date(item.startTime).toLocaleString("es-EC", { dateStyle: "medium", timeStyle: "short" })}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-16 md:px-6">
        <SectionTitle eyebrow="Planes" title="Membresias claras para entrenar mejor" description="Elige una opcion que acompane tu frecuencia, tu sede y tu forma de progresar." />
        <div className="mt-10 grid gap-5 md:grid-cols-2 xl:grid-cols-3">{memberships.slice(0, 3).map((plan) => <PlanCard key={plan.id} plan={plan} branchName={branchMap[plan.branchId]} />)}</div>
      </section>
    </main>
  );
}
