import Image from "next/image";
import {
  ArrowRight,
  Dumbbell,
  Flame,
  MapPin,
  ShieldCheck,
  Smartphone,
  Sparkles,
  Trophy,
  Users,
} from "lucide-react";
import { HeroMediaCarousel, type HeroSlide } from "@/components/marketing/hero-media-carousel";
import { Button } from "@/components/ui/button";
import { getPublicData } from "@/lib/backend/api";

const goals = [
  {
    label: "Dorian Focus",
    title: "Fuerza y musculo",
    description: "Entrena con estructura, peso libre y una energia que empuja progreso real.",
    image: "/marketing/hero/hero-2.png",
    href: "/planes",
  },
  {
    label: "Dorian Focus",
    title: "Rendimiento total",
    description: "Cardio, funcional y sesiones que te mantienen en movimiento con intensidad.",
    image: "/marketing/hero/hero-3.png",
    href: "/clases",
  },
  {
    label: "Dorian Focus",
    title: "Combate y caracter",
    description: "Clases con identidad, foco y un ambiente que exige tu mejor version.",
    image: "/marketing/hero/hero-4.png",
    href: "/clases",
  },
  {
    label: "Dorian Recovery",
    title: "Recuperacion premium",
    description: "Sauna, comodidad y una experiencia de club que completa tu rutina.",
    image: "/marketing/hero/dorian-sauna.jpg",
    href: "/sucursales",
  },
];

const testimonials = [
  {
    name: "Andrea M.",
    role: "Member / Fuerza",
    text: "Dorian me dio una rutina con identidad. La energia del lugar te empuja a volver.",
  },
  {
    name: "Carlos R.",
    role: "Member / Elite",
    text: "Las sedes se sienten premium, las clases tienen nivel y el ambiente te mantiene enfocado.",
  },
  {
    name: "Sofia P.",
    role: "Member / Multi-sede",
    text: "Puedo reservar desde la app y seguir entrenando con la misma experiencia en cualquier sede.",
  },
];

function formatCurrency(value: number | null | undefined) {
  if (value == null) return "Consultar";

  return new Intl.NumberFormat("es-EC", {
    style: "currency",
    currency: "USD",
    maximumFractionDigits: 0,
  }).format(value);
}

function describeMembership(durationInDays: number, branchName?: string) {
  const branchLine = branchName ? `Disponible en ${branchName}.` : "Disponible en sedes Dorian.";

  if (durationInDays >= 180) {
    return `${branchLine} Pensado para constancia, mayor permanencia y una experiencia premium mas completa.`;
  }

  if (durationInDays >= 90) {
    return `${branchLine} Ideal para mantener ritmo, disciplina y acceso frecuente a la experiencia del club.`;
  }

  return `${branchLine} Una forma directa de empezar a entrenar con energia, estructura y acceso premium.`;
}

function getPlanBenefits(durationInDays: number) {
  if (durationInDays >= 180) {
    return ["Prioridad de reserva", "Mejor permanencia", "Experiencia Dorian mas completa"];
  }

  if (durationInDays >= 90) {
    return ["Ritmo sostenido", "Clases frecuentes", "Acceso premium continuo"];
  }

  return ["Entrada directa al club", "Clases y sedes activas", "Comienzo rapido y claro"];
}

function getFeaturedClassImage(name: string, index: number) {
  const normalizedName = name.toLowerCase();

  if (normalizedName.includes("bail")) {
    return "/marketing/hero/bailoterapia.png";
  }

  if (
    normalizedName.includes("spin") ||
    normalizedName.includes("ciclo") ||
    normalizedName.includes("bike")
  ) {
    return "/marketing/hero/spinning.png";
  }

  return index % 3 === 0
    ? "/marketing/hero/hero-4.png"
    : index % 3 === 1
      ? "/marketing/hero/hero-3.png"
      : "/marketing/hero/hero-2.png";
}

function MonoLabel({ children }: { children: React.ReactNode }) {
  return (
    <span className="inline-flex items-center rounded-full border border-[var(--border-soft)] bg-white/[0.04] px-3 py-1 font-mono text-[10px] uppercase tracking-[0.22em] text-[var(--text-tertiary)]">
      {children}
    </span>
  );
}

function SectionHeading({
  index,
  title,
  description,
  actionLabel,
  actionHref,
}: {
  index: string;
  title: string;
  description: string;
  actionLabel?: string;
  actionHref?: string;
}) {
  return (
    <div className="flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
      <div className="max-w-3xl">
        <p className="font-mono text-[11px] uppercase tracking-[0.26em] text-[var(--accent)]/78">
          {index}
        </p>
        <h2 className="mt-5 font-heading text-[2.1rem] font-black uppercase leading-[0.96] text-white md:text-[3rem]">
          {title}
        </h2>
        <p className="mt-5 max-w-2xl text-base leading-8 text-[var(--text-secondary)]">
          {description}
        </p>
      </div>
      {actionLabel && actionHref ? (
        <Button
          href={actionHref}
          variant="secondary"
          className="w-fit gap-2 border border-white/10 px-6 py-3 font-mono text-[11px] uppercase tracking-[0.22em]"
        >
          {actionLabel} <ArrowRight className="h-4 w-4" />
        </Button>
      ) : null}
    </div>
  );
}

function GoalCard({ goal }: { goal: (typeof goals)[number] }) {
  return (
    <a
      href={goal.href}
      className="group relative min-h-[380px] overflow-hidden rounded-[32px] border border-white/8 bg-[var(--surface)]"
    >
      <Image
        src={goal.image}
        alt={goal.title}
        fill
        sizes="(max-width: 768px) 100vw, 25vw"
        className="object-cover transition duration-700 group-hover:scale-105"
      />
      <div className="absolute inset-0 bg-[linear-gradient(180deg,rgba(0,0,0,0.04)_0%,rgba(0,0,0,0.24)_28%,rgba(0,0,0,0.94)_100%)]" />
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_left,rgba(255,95,0,0.18),transparent_28%)]" />
      <div className="absolute bottom-0 p-6">
        <MonoLabel>{goal.label}</MonoLabel>
        <h3 className="mt-5 max-w-[12rem] font-heading text-3xl font-black uppercase leading-[0.96] text-white">
          {goal.title}
        </h3>
        <p className="mt-3 max-w-[18rem] text-sm leading-7 text-[var(--text-secondary)]">
          {goal.description}
        </p>
        <span className="mt-6 inline-flex items-center gap-2 font-mono text-[11px] uppercase tracking-[0.2em] text-white transition group-hover:text-[var(--accent)]">
          Explorar <ArrowRight className="h-4 w-4" />
        </span>
      </div>
    </a>
  );
}

export default async function HomePage() {
  const { branches, classes, promotions, memberships } = await getPublicData();
  const appUrl = process.env.NEXT_PUBLIC_APP_DOWNLOAD_URL ?? "/";
  const branchNames = new Map(branches.map((branch) => [branch.id, branch.name]));

  const heroSlides: HeroSlide[] = [
    {
      eyebrow: "Disciplina, fuerza y comunidad",
      title: "FORJADO POR DISCIPLINA.",
      description:
        "Entrena en un club premium con varias sedes, clases de alto nivel y una experiencia pensada para que vuelvas cada dia.",
      image: "/marketing/hero/hero-2.png",
      imagePosition: "center center",
      primaryCta: "Ver planes",
      primaryHref: "/planes",
      secondaryCta: "Descargar app",
      secondaryHref: appUrl,
    },
    {
      eyebrow: "Publico mixto premium",
      title: "UNA MISMA ENERGIA. DISTINTAS FORMAS DE ENTRENAR.",
      description:
        "Fuerza, funcional y comunidad en una experiencia que acompana tanto a quienes empiezan como a quienes quieren ir mas lejos.",
      image: "/marketing/hero/hero-3.png",
      imagePosition: "center center",
      primaryCta: "Descargar app",
      primaryHref: appUrl,
      secondaryCta: "Explorar clases",
      secondaryHref: "/clases",
    },
    {
      eyebrow: "Club Dorian",
      title: "VARIAS SEDES. UNA MARCA QUE SE SIENTE ELITE.",
      description:
        "Descubre un entorno de entrenamiento que combina intensidad, recuperacion y una identidad visual fuerte en cada sede.",
      image: "/marketing/hero/hero-4.png",
      imagePosition: "center center",
      primaryCta: "Ver sedes",
      primaryHref: "/sucursales",
      secondaryCta: "Ver planes",
      secondaryHref: "/planes",
    },
  ];

  const featuredBranches = branches.slice(0, 5);
  const featuredClasses = classes.slice(0, 4);
  const featuredPlans = memberships.slice(0, 3);
  const activePromotions = promotions.slice(0, 2);

  return (
    <main className="bg-black text-white">
      <HeroMediaCarousel
        slides={heroSlides}
        stats={[
          { value: branches.length, label: "Sedes activas" },
          { value: classes.length, label: "Clases" },
          { value: promotions.length, label: "Promos" },
        ]}
      />

      <section className="mx-auto max-w-7xl px-4 py-24 md:px-6 md:py-32">
        <SectionHeading
          index="01 / OBJETIVOS"
          title="Entrena por objetivo."
          description="Elige la energia que quieres vivir dentro del club y entra a una experiencia que mezcla disciplina, fuerza y una comunidad que empuja."
          actionLabel="Ver planes"
          actionHref="/planes"
        />

        <div className="mt-14 grid gap-5 md:grid-cols-2 xl:grid-cols-4">
          {goals.map((goal) => (
            <GoalCard key={goal.title} goal={goal} />
          ))}
        </div>
      </section>

      <section className="border-y border-white/8 bg-[linear-gradient(180deg,#101010_0%,#080808_100%)]">
        <div className="mx-auto grid max-w-7xl gap-12 px-4 py-24 md:px-6 lg:grid-cols-[0.78fr_1.22fr] lg:py-32">
          <div>
            <SectionHeading
              index="02 / UBICACIONES"
              title="Varias sedes. Una misma atmosfera."
              description="Dorian se vive con la misma identidad premium en cada sucursal: entrenamiento serio, espacios cuidados y una experiencia que se siente consistente."
            />

            <div className="mt-10 rounded-[32px] border border-white/8 bg-white/[0.03] p-6 backdrop-blur-xl">
              <div className="flex items-center justify-between gap-4 border-b border-white/8 pb-5">
                <MonoLabel>Dorian Elite Standard</MonoLabel>
                <span className="font-mono text-[11px] uppercase tracking-[0.2em] text-[var(--text-tertiary)]">
                  Club premium
                </span>
              </div>

              <ul className="mt-6 space-y-4 text-sm text-[var(--text-secondary)]">
                <li className="flex items-start gap-3">
                  <Sparkles className="mt-1 h-4 w-4 text-[var(--accent)]" />
                  Varias sedes para entrenar con una experiencia de marca consistente.
                </li>
                <li className="flex items-start gap-3">
                  <ShieldCheck className="mt-1 h-4 w-4 text-[var(--accent)]" />
                  Sauna, recuperacion y detalles de club que elevan el entrenamiento.
                </li>
                <li className="flex items-start gap-3">
                  <Smartphone className="mt-1 h-4 w-4 text-[var(--accent)]" />
                  App propia para reservas, clases y seguimiento sin friccion.
                </li>
              </ul>
            </div>
          </div>

          <div className="grid gap-5 lg:grid-cols-2">
            {featuredBranches.map((branch, index) => {
              const branchImage =
                index % 3 === 0
                  ? "/marketing/hero/hero-2.png"
                  : index % 3 === 1
                    ? "/marketing/hero/hero-3.png"
                    : "/marketing/hero/hero-4.png";
              const isFeaturedBranch = index === 2;

              return (
                <article
                  key={branch.id}
                  className={`group relative overflow-hidden rounded-[32px] border border-white/8 ${
                    isFeaturedBranch
                      ? "min-h-[420px] lg:col-span-2"
                      : "min-h-[320px]"
                  }`}
                >
                  <Image
                    src={branchImage}
                    alt={branch.name}
                    fill
                    sizes="(max-width: 1024px) 100vw, 33vw"
                    className="object-cover transition duration-700 group-hover:scale-105"
                  />
                  <div className="absolute inset-0 bg-[linear-gradient(180deg,rgba(0,0,0,0.12)_0%,rgba(0,0,0,0.34)_32%,rgba(0,0,0,0.96)_100%)]" />
                  <div className="absolute inset-0 bg-[radial-gradient(circle_at_bottom_left,rgba(255,95,0,0.18),transparent_30%)]" />
                  <div className="absolute bottom-0 flex h-full flex-col justify-end p-6">
                    <MonoLabel>Sede Dorian</MonoLabel>
                    <h3
                      className={`mt-5 font-heading font-black uppercase leading-[0.94] text-white ${
                        isFeaturedBranch
                          ? "text-4xl md:text-5xl"
                          : "text-4xl"
                      }`}
                    >
                      {branch.name}
                    </h3>
                    <p
                      className={`mt-3 text-sm leading-7 text-[var(--text-secondary)] ${
                        isFeaturedBranch ? "max-w-xl" : "max-w-xs"
                      }`}
                    >
                      {branch.address ??
                        "Ubicacion premium lista para entrenar con foco, identidad y constancia."}
                    </p>
                    <a
                      href="/sucursales"
                      className="mt-6 inline-flex items-center gap-2 font-mono text-[11px] uppercase tracking-[0.2em] text-white transition group-hover:text-[var(--accent)]"
                    >
                      Ver sede <ArrowRight className="h-4 w-4" />
                    </a>
                  </div>
                </article>
              );
            })}
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-24 md:px-6 md:py-32">
        <SectionHeading
          index="03 / CLASES"
          title="Clases con identidad y energia propia."
          description="De fuerza y funcional a sesiones de alto impacto, Dorian mezcla intensidad, tecnica y un entorno que convierte cada clase en una experiencia."
          actionLabel="Explorar clases"
          actionHref="/clases"
        />

        <div className="mt-14 grid gap-5 lg:grid-cols-2">
          {featuredClasses.map((item, index) => {
            const cardImage = getFeaturedClassImage(item.name, index);

            return (
              <article
                key={item.id}
                className="group relative overflow-hidden rounded-[32px] border border-white/8 bg-[var(--surface)]"
              >
                <div className="grid min-h-[340px] lg:grid-cols-[1.05fr_0.95fr]">
                  <div className="relative min-h-[260px]">
                    <Image
                      src={cardImage}
                      alt={item.name}
                      fill
                      sizes="(max-width: 1024px) 100vw, 40vw"
                      className="object-cover transition duration-700 group-hover:scale-105"
                    />
                    <div className="absolute inset-0 bg-[linear-gradient(90deg,rgba(0,0,0,0.08)_0%,rgba(0,0,0,0.12)_30%,rgba(0,0,0,0.82)_100%)] lg:hidden" />
                  </div>
                  <div className="relative flex flex-col justify-between bg-[linear-gradient(180deg,#141414_0%,#090909_100%)] p-7">
                    <div>
                      <div className="flex items-center gap-3">
                        <div className="flex h-11 w-11 items-center justify-center rounded-full border border-white/8 bg-white/[0.04] text-[var(--accent)]">
                          {index % 2 === 0 ? (
                            <Flame className="h-5 w-5" />
                          ) : (
                            <Dumbbell className="h-5 w-5" />
                          )}
                        </div>
                        <MonoLabel>Dorian Class</MonoLabel>
                      </div>
                      <h3 className="mt-6 font-heading text-4xl font-black uppercase leading-[0.95] text-white">
                        {item.name}
                      </h3>
                      <p className="mt-4 text-sm leading-7 text-[var(--text-secondary)]">
                        {item.description ??
                          "Una clase pensada para entrenar con ritmo, enfoque y progresion real."}
                      </p>
                    </div>
                    <a
                      href="/clases"
                      className="mt-8 inline-flex items-center gap-2 font-mono text-[11px] uppercase tracking-[0.2em] text-white transition group-hover:text-[var(--accent)]"
                    >
                      Reservar clase <ArrowRight className="h-4 w-4" />
                    </a>
                  </div>
                </div>
              </article>
            );
          })}
        </div>
      </section>

      <section className="bg-[linear-gradient(180deg,#0d0d0d_0%,#050505_100%)]">
        <div className="mx-auto max-w-7xl px-4 py-24 md:px-6 md:py-32">
          <SectionHeading
            index="04 / MEMBRESIAS"
            title="Elige tu nivel."
            description="Planes para entrenar con libertad, reservar tus clases y vivir Dorian con el nivel de acceso que mejor encaje contigo."
          />

          <div className="mt-14 grid gap-6 lg:grid-cols-3">
            {featuredPlans.map((plan, index) => {
              const featured = index === 1;
              const benefits = getPlanBenefits(plan.durationInDays);

              return (
                <article
                  key={plan.id}
                  className="group rounded-[32px] border border-white/8 bg-[var(--surface)] p-8 transition duration-300 hover:border-[var(--accent)]/55 hover:bg-[linear-gradient(180deg,rgba(255,95,0,0.14)_0%,rgba(12,12,12,0.98)_34%,rgba(5,5,5,1)_100%)] hover:shadow-[0_0_32px_rgba(255,95,0,0.14)]"
                >
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <MonoLabel>{featured ? "Dorian Elite" : "Membresia Dorian"}</MonoLabel>
                      <h3 className="mt-5 font-heading text-4xl font-black uppercase leading-[0.95] text-white">
                        {plan.name}
                      </h3>
                    </div>
                    <span
                      className={`rounded-full border px-3 py-1 font-mono text-[10px] uppercase tracking-[0.2em] transition ${
                        featured
                          ? "border-white/10 bg-white/[0.03] text-[var(--text-tertiary)] group-hover:border-[var(--accent)]/30 group-hover:bg-[var(--accent)]/10 group-hover:text-[var(--accent)]"
                          : "border-white/10 bg-white/[0.03] text-[var(--text-tertiary)] group-hover:border-[var(--accent)]/30 group-hover:bg-[var(--accent)]/10 group-hover:text-[var(--accent)]"
                      }`}
                    >
                      {featured ? "Recomendado" : "Disponible"}
                    </span>
                  </div>

                  <p className="mt-4 min-h-[84px] text-sm leading-7 text-[var(--text-secondary)]">
                    {describeMembership(plan.durationInDays, branchNames.get(plan.branchId))}
                  </p>

                  <div className="mt-8 border-t border-white/8 pt-8">
                    <p className="font-heading text-5xl font-black text-white">
                      {formatCurrency(plan.price)}
                    </p>
                    <p className="mt-2 font-mono text-[11px] uppercase tracking-[0.24em] text-[var(--text-tertiary)]">
                      Plan premium
                    </p>
                  </div>

                  <div className="mt-8 space-y-3">
                    {benefits.map((benefit) => (
                      <div
                        key={benefit}
                        className="flex items-start gap-3 text-sm text-[var(--text-secondary)]"
                      >
                        <ShieldCheck className="mt-1 h-4 w-4 text-[var(--accent)]" />
                        <span>{benefit}</span>
                      </div>
                    ))}
                  </div>

                  <Button
                    href="/planes"
                    className="mt-8 w-full justify-center gap-2 px-6 py-3.5 font-mono text-[11px] uppercase tracking-[0.2em]"
                  >
                    Ver planes <ArrowRight className="h-4 w-4" />
                  </Button>
                </article>
              );
            })}
          </div>
        </div>
      </section>

      {activePromotions.length ? (
        <section className="mx-auto max-w-7xl px-4 py-24 md:px-6 md:py-28">
          <SectionHeading
            index="05 / PROMOCIONES"
            title="Promos para entrar con fuerza."
            description="Ofertas activas pensadas para que el primer paso hacia Dorian se sienta mas facil, mas claro y mas atractivo."
          />

          <div className="mt-12 grid gap-5 lg:grid-cols-2">
            {activePromotions.map((promo, index) => (
              <article
                key={promo.id}
                className="relative overflow-hidden rounded-[32px] border border-white/8 bg-[linear-gradient(135deg,#141414_0%,#0a0a0a_58%,rgba(255,95,0,0.16)_100%)] p-8"
              >
                <div className="absolute right-6 top-6 rounded-full border border-white/10 bg-white/5 px-3 py-1 font-mono text-[10px] uppercase tracking-[0.2em] text-white/60">
                  {index === 0 ? "Oferta actual" : "Beneficio especial"}
                </div>
                <Trophy className="h-9 w-9 text-[var(--accent)]" />
                <h3 className="mt-6 max-w-md font-heading text-4xl font-black uppercase leading-[0.96] text-white">
                  {promo.title}
                </h3>
                <p className="mt-4 max-w-lg text-sm leading-7 text-[var(--text-secondary)]">
                  {promo.description}
                </p>
                <Button
                  href="/promociones"
                  className="mt-8 gap-2 px-6 py-3.5 font-mono text-[11px] uppercase tracking-[0.2em]"
                >
                  Ver promocion <ArrowRight className="h-4 w-4" />
                </Button>
              </article>
            ))}
          </div>
        </section>
      ) : null}

      <section className="border-y border-white/8 bg-[linear-gradient(180deg,#101010_0%,#060606_100%)]">
        <div className="mx-auto grid max-w-7xl gap-12 px-4 py-24 md:px-6 lg:grid-cols-[0.95fr_1.05fr] lg:items-center lg:py-32">
          <div>
            <p className="font-mono text-[11px] uppercase tracking-[0.26em] text-[var(--accent)]/78">
              06 / APP DORIAN
            </p>
            <h2 className="mt-5 max-w-3xl font-heading text-[2.1rem] font-black uppercase leading-[0.96] text-white md:text-[3rem]">
              Tu entrenamiento, tu plan y tus reservas en una sola app.
            </h2>
            <p className="mt-5 max-w-2xl text-base leading-8 text-[var(--text-secondary)]">
              Descarga la app de Dorian para revisar tus clases, gestionar tu plan y moverte entre
              sedes con una experiencia mucho mas conectada.
            </p>

            <div className="mt-10 grid gap-4 sm:grid-cols-2">
              {[
                "Reservas en tiempo real",
                "Planes y pagos seguros",
                "Acceso a clases por sede",
                "Seguimiento de tu progreso",
              ].map((feature) => (
                <div
                  key={feature}
                  className="rounded-[24px] border border-white/8 bg-white/[0.03] p-5 text-sm text-[var(--text-secondary)]"
                >
                  <div className="mb-4 flex h-10 w-10 items-center justify-center rounded-full border border-white/8 bg-[var(--accent)]/10 text-[var(--accent)]">
                    <Smartphone className="h-4 w-4" />
                  </div>
                  {feature}
                </div>
              ))}
            </div>

            <div className="mt-8 flex flex-col gap-3 sm:flex-row">
              <Button
                href={appUrl}
                className="gap-2 px-6 py-3.5 font-mono text-[11px] uppercase tracking-[0.2em]"
              >
                Descargar app <ArrowRight className="h-4 w-4" />
              </Button>
              <Button
                href="/planes"
                variant="secondary"
                className="border border-white/10 px-6 py-3.5 font-mono text-[11px] uppercase tracking-[0.2em]"
              >
                Ver planes
              </Button>
            </div>
          </div>

          <div className="group relative overflow-hidden rounded-[34px] border border-white/8 bg-[linear-gradient(180deg,#161616_0%,#090909_100%)] p-6 transition duration-300 hover:border-[var(--accent)]/55 hover:bg-[linear-gradient(180deg,rgba(255,95,0,0.14)_0%,rgba(12,12,12,0.98)_34%,rgba(5,5,5,1)_100%)] hover:shadow-[0_0_32px_rgba(255,95,0,0.14)]">
            <div className="absolute inset-0 bg-[radial-gradient(circle_at_bottom_right,rgba(255,95,0,0.18),transparent_30%)]" />
            <div className="relative mx-auto max-w-[380px] rounded-[32px] border border-white/8 bg-black/80 p-4 shadow-[0_30px_80px_rgba(0,0,0,0.45)]">
              <div className="rounded-[26px] border border-white/8 bg-[#111111] p-4">
                <div className="flex items-center justify-between font-mono text-[11px] uppercase tracking-[0.2em] text-white/48">
                  <span>Dorian App</span>
                  <span>Elite access</span>
                </div>
                <div className="mt-6 rounded-[24px] border border-white/8 bg-[linear-gradient(180deg,rgba(255,95,0,0.18)_0%,rgba(16,16,16,0.92)_100%)] p-5">
                  <p className="font-mono text-[11px] uppercase tracking-[0.2em] text-[var(--accent)]">
                    Plan activo
                  </p>
                  <h3 className="mt-4 font-heading text-3xl font-black uppercase text-white">
                    Dorian Elite
                  </h3>
                  <p className="mt-3 text-sm text-[var(--text-secondary)]">
                    Clases, reservas y experiencia premium desde tu bolsillo.
                  </p>
                </div>
                <div className="mt-5 grid gap-3">
                  {["Mis reservas", "Entrenamiento", "Nutricion", "Actividades"].map(
                    (item, index) => (
                      <div
                        key={item}
                        className="rounded-[20px] border border-white/8 bg-white/[0.03] p-4"
                      >
                        <div className="flex items-center justify-between">
                          <span className="font-medium text-white">{item}</span>
                          <ArrowRight className="h-4 w-4 text-[var(--accent)]" />
                        </div>
                      </div>
                    ),
                  )}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-24 md:px-6 md:py-28">
        <SectionHeading
          index="07 / TESTIMONIOS"
          title="Disciplina, fuerza y comunidad que se siente."
          description="La energia del club se construye con personas que entrenan con enfoque y encuentran en Dorian una experiencia mas completa."
        />

        <div className="mt-12 grid gap-5 lg:grid-cols-3">
          {testimonials.map((testimonial) => (
            <article
              key={testimonial.name}
              className="rounded-[32px] border border-white/8 bg-[linear-gradient(180deg,#141414_0%,#0a0a0a_100%)] p-7"
            >
              <Users className="h-8 w-8 text-[var(--accent)]" />
              <p className="mt-6 font-heading text-2xl font-black uppercase leading-[0.94] text-white">
                "{testimonial.text}"
              </p>
              <div className="mt-8 border-t border-white/8 pt-6">
                <p className="font-mono text-[10px] uppercase tracking-[0.22em] text-[var(--accent)]">
                  {testimonial.role}
                </p>
                <div className="mt-2 flex items-center gap-2 text-white">
                  <Trophy className="h-4 w-4 text-[var(--accent)]" />
                  <p className="text-sm font-semibold uppercase tracking-[0.18em]">
                    {testimonial.name}
                  </p>
                </div>
              </div>
            </article>
          ))}
        </div>
      </section>

      <section className="px-4 pb-20 md:px-6 md:pb-28">
        <div className="mx-auto max-w-7xl overflow-hidden rounded-[36px] border border-white/8 bg-[linear-gradient(120deg,#161616_0%,#0b0b0b_58%,rgba(255,95,0,0.16)_100%)] p-8 md:p-12">
          <div className="flex flex-col gap-10 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-3xl">
              <p className="font-mono text-[11px] uppercase tracking-[0.26em] text-[var(--accent)]/78">
                08 / EMPIEZA HOY
              </p>
              <h2 className="mt-5 font-heading text-[2.1rem] font-black uppercase leading-[0.96] text-white md:text-[3rem]">
                Entrena en un club premium que vive de disciplina, fuerza y comunidad.
              </h2>
              <p className="mt-5 max-w-2xl text-base leading-8 text-[var(--text-secondary)]">
                Descarga la app, revisa los planes y elige la sede donde quieres empezar a
                transformar tu rutina.
              </p>
            </div>

            <div className="flex flex-col gap-3 sm:flex-row">
              <Button
                href="/planes"
                className="gap-2 px-6 py-3.5 font-mono text-[11px] uppercase tracking-[0.2em]"
              >
                Ver planes <ArrowRight className="h-4 w-4" />
              </Button>
              <Button
                href={appUrl}
                variant="secondary"
                className="border border-white/10 px-6 py-3.5 font-mono text-[11px] uppercase tracking-[0.2em]"
              >
                Descargar app
              </Button>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
