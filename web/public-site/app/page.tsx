import Image from "next/image";
import {
  ArrowRight,
  Dumbbell,
  Flame,
  MapPin,
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
    title: "Fuerza y musculo",
    description: "Entrena con estructura, peso libre y una energia que empuja progreso real.",
    image: "/marketing/hero/hero-2.png",
    href: "/planes",
  },
  {
    title: "Rendimiento total",
    description: "Cardio, funcional y sesiones que te mantienen en movimiento con intensidad.",
    image: "/marketing/hero/hero-3.png",
    href: "/clases",
  },
  {
    title: "Combate y caracter",
    description: "Clases con identidad, foco y un ambiente que exige tu mejor version.",
    image: "/marketing/hero/hero-4.png",
    href: "/clases",
  },
  {
    title: "Recuperacion premium",
    description: "Sauna, comodidad y una experiencia de club que completa tu rutina.",
    image: "/marketing/hero/dorian-sauna.jpg",
    href: "/sucursales",
  },
];

const testimonials = [
  {
    name: "Andrea M.",
    text: "Dorian me dio una rutina con identidad. La energia del lugar te empuja a volver.",
  },
  {
    name: "Carlos R.",
    text: "Las sedes se sienten premium, las clases tienen nivel y el ambiente te mantiene enfocado.",
  },
  {
    name: "Sofia P.",
    text: "Me gusta que puedo reservar desde la app y seguir entrenando con la misma experiencia en cualquier sede.",
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

function GoalCard({ goal }: { goal: (typeof goals)[number] }) {
  return (
    <a
      href={goal.href}
      className="group relative min-h-[360px] overflow-hidden rounded-[28px] border border-white/10 bg-zinc-950"
    >
      <Image
        src={goal.image}
        alt={goal.title}
        fill
        sizes="(max-width: 768px) 100vw, 25vw"
        className="object-cover transition duration-700 group-hover:scale-105"
      />
      <div className="absolute inset-0 bg-[linear-gradient(180deg,rgba(0,0,0,0.08)_0%,rgba(0,0,0,0.34)_34%,rgba(0,0,0,0.92)_100%)]" />
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_left,rgba(255,95,0,0.16),transparent_28%)]" />
      <div className="absolute bottom-0 p-6">
        <p className="text-[11px] font-semibold uppercase tracking-[0.34em] text-[var(--accent)]">
          Dorian Focus
        </p>
        <h3 className="mt-4 max-w-[14rem] font-heading text-3xl font-black text-white">
          {goal.title}
        </h3>
        <p className="mt-3 max-w-[18rem] text-sm leading-6 text-white/70">{goal.description}</p>
        <span className="mt-6 inline-flex items-center gap-2 text-sm font-semibold text-white transition group-hover:text-[var(--accent)]">
          Explorar <ArrowRight className="h-4 w-4" />
        </span>
      </div>
    </a>
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
        <p className="text-[11px] font-semibold uppercase tracking-[0.38em] text-[var(--accent)]">
          {index}
        </p>
        <h2 className="mt-4 font-heading text-4xl font-black leading-[1.02] text-white md:text-6xl">
          {title}
        </h2>
        <p className="mt-5 max-w-2xl text-base leading-7 text-white/62">{description}</p>
      </div>
      {actionLabel && actionHref ? (
        <Button href={actionHref} variant="secondary" className="w-fit gap-2">
          {actionLabel} <ArrowRight className="h-4 w-4" />
        </Button>
      ) : null}
    </div>
  );
}

export default async function HomePage() {
  const { branches, classes, promotions, memberships } = await getPublicData();
  const appUrl = process.env.NEXT_PUBLIC_APP_DOWNLOAD_URL ?? "/";
  const branchNames = new Map(branches.map((branch) => [branch.id, branch.name]));

  const heroSlides: HeroSlide[] = [
    {
      eyebrow: "Disciplina, fuerza y comunidad",
      title: "Tu transformacion comienza aqui.",
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
      title: "La misma energia. Distintas formas de entrenar.",
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
      title: "Varias sedes, sauna y una marca que se siente premium.",
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

  const featuredBranches = branches.slice(0, 3);
  const featuredClasses = classes.slice(0, 4);
  const featuredPlans = memberships.slice(0, 3);
  const activePromotions = promotions.slice(0, 2);

  return (
    <main className="bg-[#0c0c0c] text-white">
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
          index="01 / Objetivos"
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

      <section className="border-y border-white/8 bg-[#101010]">
        <div className="mx-auto grid max-w-7xl gap-12 px-4 py-24 md:px-6 lg:grid-cols-[0.85fr_1.15fr] lg:py-32">
          <div>
            <SectionHeading
              index="02 / Ubicaciones"
              title="Varias sedes. Una misma atmosfera."
              description="Dorian se vive con la misma identidad premium en cada sucursal: entrenamiento serio, espacios cuidados y una experiencia que se siente consistente."
            />

            <div className="mt-10 rounded-[28px] border border-white/10 bg-black/40 p-6">
              <p className="text-[11px] font-semibold uppercase tracking-[0.34em] text-[var(--accent)]">
                Diferenciales
              </p>
              <ul className="mt-5 space-y-4 text-sm text-white/74">
                <li className="flex items-start gap-3">
                  <Sparkles className="mt-0.5 h-4 w-4 text-[var(--accent)]" />
                  Varias sedes para entrenar con la misma energia de marca.
                </li>
                <li className="flex items-start gap-3">
                  <Sparkles className="mt-0.5 h-4 w-4 text-[var(--accent)]" />
                  Sauna y recuperacion para completar la experiencia premium.
                </li>
                <li className="flex items-start gap-3">
                  <Sparkles className="mt-0.5 h-4 w-4 text-[var(--accent)]" />
                  App propia para planes, clases y reservas en tiempo real.
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

              return (
                <article
                  key={branch.id}
                  className="group relative min-h-[420px] overflow-hidden rounded-[28px] border border-white/10"
                >
                  <Image
                    src={branchImage}
                    alt={branch.name}
                    fill
                    sizes="(max-width: 1024px) 100vw, 33vw"
                    className="object-cover transition duration-700 group-hover:scale-105"
                  />
                  <div className="absolute inset-0 bg-[linear-gradient(180deg,rgba(0,0,0,0.18)_0%,rgba(0,0,0,0.38)_34%,rgba(0,0,0,0.94)_100%)]" />
                  <div className="absolute inset-0 bg-[radial-gradient(circle_at_bottom_left,rgba(255,95,0,0.18),transparent_30%)]" />
                  <div className="absolute bottom-0 flex h-full flex-col justify-end p-6">
                    <div className="inline-flex w-fit items-center gap-2 rounded-full border border-white/12 bg-black/40 px-3 py-1 text-[11px] font-semibold uppercase tracking-[0.28em] text-white/80">
                      <MapPin className="h-3.5 w-3.5 text-[var(--accent)]" />
                      Sede Dorian
                    </div>
                    <h3 className="mt-5 font-heading text-3xl font-black text-white">{branch.name}</h3>
                    <p className="mt-3 max-w-xs text-sm leading-6 text-white/68">
                      {branch.address ?? "Ubicacion premium lista para entrenar con foco y constancia."}
                    </p>
                    <a
                      href="/sucursales"
                      className="mt-6 inline-flex items-center gap-2 text-sm font-semibold text-white transition group-hover:text-[var(--accent)]"
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
          index="03 / Clases"
          title="Clases con identidad y energia propia."
          description="De fuerza y funcional a sesiones de alto impacto, Dorian mezcla intensidad, tecnica y un entorno que convierte cada clase en una experiencia."
          actionLabel="Explorar clases"
          actionHref="/clases"
        />

        <div className="mt-14 grid gap-5 lg:grid-cols-2">
          {featuredClasses.map((item, index) => {
            const cardImage =
              index % 3 === 0
                ? "/marketing/hero/hero-4.png"
                : index % 3 === 1
                  ? "/marketing/hero/hero-3.png"
                  : "/marketing/hero/hero-2.png";

            return (
              <article
                key={item.id}
                className="group relative overflow-hidden rounded-[28px] border border-white/10 bg-zinc-950"
              >
                <div className="grid min-h-[320px] lg:grid-cols-[1.1fr_0.9fr]">
                  <div className="relative min-h-[240px]">
                    <Image
                      src={cardImage}
                      alt={item.name}
                      fill
                      sizes="(max-width: 1024px) 100vw, 40vw"
                      className="object-cover transition duration-700 group-hover:scale-105"
                    />
                    <div className="absolute inset-0 bg-[linear-gradient(90deg,rgba(0,0,0,0.15)_0%,rgba(0,0,0,0.1)_30%,rgba(0,0,0,0.86)_100%)] lg:hidden" />
                  </div>
                  <div className="relative flex flex-col justify-between bg-[linear-gradient(180deg,#151515_0%,#0b0b0b_100%)] p-6">
                    <div>
                      <div className="flex items-center gap-3">
                        <div className="flex h-11 w-11 items-center justify-center rounded-full bg-white/8 text-[var(--accent)]">
                          {index % 2 === 0 ? (
                            <Flame className="h-5 w-5" />
                          ) : (
                            <Dumbbell className="h-5 w-5" />
                          )}
                        </div>
                        <p className="text-[11px] font-semibold uppercase tracking-[0.34em] text-[var(--accent)]">
                          Dorian Class
                        </p>
                      </div>
                      <h3 className="mt-6 font-heading text-3xl font-black text-white">{item.name}</h3>
                      <p className="mt-4 text-sm leading-7 text-white/66">
                        {item.description ??
                          "Una clase pensada para entrenar con ritmo, enfoque y progresion real."}
                      </p>
                    </div>
                    <a
                      href="/clases"
                      className="mt-8 inline-flex items-center gap-2 text-sm font-semibold text-white transition group-hover:text-[var(--accent)]"
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

      <section className="bg-[#111111]">
        <div className="mx-auto max-w-7xl px-4 py-24 md:px-6 md:py-32">
          <SectionHeading
            index="04 / Membresias"
            title="Elige tu nivel."
            description="Planes para entrenar con libertad, reservar tus clases y vivir Dorian con el nivel de acceso que mejor encaje contigo."
          />

          <div className="mt-14 grid gap-6 lg:grid-cols-3">
            {featuredPlans.map((plan, index) => {
              const featured = index === 1;
              return (
                <article
                  key={plan.id}
                  className={`rounded-[30px] border p-8 transition ${
                    featured
                      ? "border-[var(--accent)] bg-[linear-gradient(180deg,rgba(255,95,0,0.1)_0%,rgba(14,14,14,0.98)_38%,rgba(10,10,10,1)_100%)] shadow-[0_0_0_1px_rgba(255,95,0,0.08)]"
                      : "border-white/10 bg-black/55"
                  }`}
                >
                  <div className="flex items-start justify-between gap-4">
                    <div>
                      <p className="text-[11px] font-semibold uppercase tracking-[0.32em] text-[var(--accent)]">
                        {featured ? "Mas elegido" : "Membresia Dorian"}
                      </p>
                      <h3 className="mt-4 font-heading text-3xl font-black text-white">{plan.name}</h3>
                    </div>
                    {featured ? (
                      <span className="rounded-full border border-[var(--accent)]/40 bg-[var(--accent)]/12 px-3 py-1 text-[10px] font-semibold uppercase tracking-[0.28em] text-[var(--accent)]">
                        Recomendado
                      </span>
                    ) : null}
                  </div>

                  <p className="mt-4 min-h-[72px] text-sm leading-7 text-white/64">
                    {describeMembership(plan.durationInDays, branchNames.get(plan.branchId))}
                  </p>

                  <div className="mt-8 border-t border-white/8 pt-8">
                    <p className="font-heading text-5xl font-black text-white">
                      {formatCurrency(plan.price)}
                    </p>
                    <p className="mt-2 text-[11px] uppercase tracking-[0.3em] text-white/42">
                      Plan premium
                    </p>
                  </div>

                  <Button href="/planes" className="mt-8 w-full justify-center gap-2">
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
            index="05 / Promociones"
            title="Promos para entrar con fuerza."
            description="Ofertas activas pensadas para que el primer paso hacia Dorian se sienta mas facil, mas claro y mas atractivo."
          />

          <div className="mt-12 grid gap-5 lg:grid-cols-2">
            {activePromotions.map((promo, index) => (
              <article
                key={promo.id}
                className="relative overflow-hidden rounded-[30px] border border-white/10 bg-[linear-gradient(135deg,#141414_0%,#0c0c0c_58%,rgba(255,95,0,0.14)_100%)] p-8"
              >
                <div className="absolute right-6 top-6 rounded-full border border-white/10 bg-white/5 px-3 py-1 text-[10px] font-semibold uppercase tracking-[0.28em] text-white/60">
                  {index === 0 ? "Oferta actual" : "Beneficio especial"}
                </div>
                <Trophy className="h-9 w-9 text-[var(--accent)]" />
                <h3 className="mt-6 max-w-md font-heading text-4xl font-black text-white">
                  {promo.title}
                </h3>
                <p className="mt-4 max-w-lg text-sm leading-7 text-white/66">{promo.description}</p>
                <Button href="/promociones" className="mt-8 gap-2">
                  Ver promocion <ArrowRight className="h-4 w-4" />
                </Button>
              </article>
            ))}
          </div>
        </section>
      ) : null}

      <section className="border-y border-white/8 bg-[#101010]">
        <div className="mx-auto grid max-w-7xl gap-12 px-4 py-24 md:px-6 lg:grid-cols-[1fr_0.95fr] lg:items-center lg:py-32">
          <div>
            <p className="text-[11px] font-semibold uppercase tracking-[0.38em] text-[var(--accent)]">
              06 / App Dorian
            </p>
            <h2 className="mt-4 max-w-3xl font-heading text-4xl font-black leading-[1.02] text-white md:text-6xl">
              Tu entrenamiento, tu plan y tus reservas en una sola app.
            </h2>
            <p className="mt-5 max-w-2xl text-base leading-7 text-white/62">
              Descarga la app de Dorian para revisar tus clases, gestionar tu plan y moverte entre sedes con una experiencia mucho mas conectada.
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
                  className="rounded-[22px] border border-white/10 bg-black/45 p-5 text-sm text-white/76"
                >
                  <div className="mb-4 flex h-10 w-10 items-center justify-center rounded-full bg-[var(--accent)]/12 text-[var(--accent)]">
                    <Smartphone className="h-4 w-4" />
                  </div>
                  {feature}
                </div>
              ))}
            </div>

            <div className="mt-8 flex flex-col gap-3 sm:flex-row">
              <Button href={appUrl} className="gap-2">
                Descargar app <ArrowRight className="h-4 w-4" />
              </Button>
              <Button href="/planes" variant="secondary">
                Ver planes
              </Button>
            </div>
          </div>

          <div className="relative overflow-hidden rounded-[34px] border border-white/10 bg-[linear-gradient(180deg,#171717_0%,#0a0a0a_100%)] p-6">
            <div className="absolute inset-0 bg-[radial-gradient(circle_at_bottom_right,rgba(255,95,0,0.18),transparent_30%)]" />
            <div className="relative mx-auto max-w-[360px] rounded-[30px] border border-white/10 bg-black/80 p-4 shadow-[0_30px_80px_rgba(0,0,0,0.45)]">
              <div className="rounded-[24px] border border-white/8 bg-[#111111] p-4">
                <div className="flex items-center justify-between text-[11px] uppercase tracking-[0.28em] text-white/50">
                  <span>Dorian App</span>
                  <span>Premium</span>
                </div>
                <div className="mt-6 rounded-[20px] border border-white/8 bg-[linear-gradient(180deg,rgba(255,95,0,0.18)_0%,rgba(16,16,16,0.92)_100%)] p-5">
                  <p className="text-sm uppercase tracking-[0.28em] text-[var(--accent)]">Plan activo</p>
                  <h3 className="mt-4 font-heading text-3xl font-black text-white">Dorian Elite</h3>
                  <p className="mt-3 text-sm text-white/64">
                    Clases, reservas y experiencia premium desde tu bolsillo.
                  </p>
                </div>
                <div className="mt-5 grid gap-3">
                  {[
                    "Mis reservas",
                    "Entrenamiento",
                    "Nutricion",
                    "Actividades",
                  ].map((item, index) => (
                    <div
                      key={item}
                      className={`rounded-[18px] border p-4 ${
                        index === 0
                          ? "border-[var(--accent)]/30 bg-[var(--accent)]/10"
                          : "border-white/8 bg-white/[0.03]"
                      }`}
                    >
                      <div className="flex items-center justify-between">
                        <span className="font-medium text-white">{item}</span>
                        <ArrowRight className="h-4 w-4 text-[var(--accent)]" />
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-24 md:px-6 md:py-28">
        <SectionHeading
          index="07 / Testimonios"
          title="Disciplina, fuerza y comunidad que se siente."
          description="La energia del club se construye con personas que entrenan con enfoque y encuentran en Dorian una experiencia mas completa."
        />

        <div className="mt-12 grid gap-5 lg:grid-cols-3">
          {testimonials.map((testimonial) => (
            <article
              key={testimonial.name}
              className="rounded-[28px] border border-white/10 bg-[linear-gradient(180deg,#141414_0%,#0d0d0d_100%)] p-7"
            >
              <Users className="h-8 w-8 text-[var(--accent)]" />
              <p className="mt-6 text-lg leading-8 text-white/78">â€œ{testimonial.text}â€</p>
              <div className="mt-8 flex items-center gap-2 text-[var(--accent)]">
                <Trophy className="h-4 w-4" />
                <p className="text-sm font-semibold uppercase tracking-[0.24em]">{testimonial.name}</p>
              </div>
            </article>
          ))}
        </div>
      </section>

      <section className="px-4 pb-20 md:px-6 md:pb-28">
        <div className="mx-auto max-w-7xl overflow-hidden rounded-[34px] border border-white/10 bg-[linear-gradient(120deg,#161616_0%,#0b0b0b_58%,rgba(255,95,0,0.14)_100%)] p-8 md:p-12">
          <div className="flex flex-col gap-10 lg:flex-row lg:items-end lg:justify-between">
            <div className="max-w-3xl">
              <p className="text-[11px] font-semibold uppercase tracking-[0.38em] text-[var(--accent)]">
                08 / Empieza hoy
              </p>
              <h2 className="mt-4 font-heading text-4xl font-black leading-[1.02] text-white md:text-6xl">
                Entrena en un club premium que vive de disciplina, fuerza y comunidad.
              </h2>
              <p className="mt-5 max-w-2xl text-base leading-7 text-white/64">
                Descarga la app, revisa los planes y elige la sede donde quieres empezar a transformar tu rutina.
              </p>
            </div>

            <div className="flex flex-col gap-3 sm:flex-row">
              <Button href="/planes" className="gap-2">
                Ver planes <ArrowRight className="h-4 w-4" />
              </Button>
              <Button href={appUrl} variant="secondary">
                Descargar app
              </Button>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}

