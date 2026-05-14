import {
  ArrowRight,
  Dumbbell,
  Flame,
  MapPin,
  Smartphone,
  Star,
  Trophy,
  Users,
} from "lucide-react";
import { HeroMediaCarousel, type HeroSlide } from "@/components/marketing/hero-media-carousel";
import { Button } from "@/components/ui/button";
import { getPublicData } from "@/lib/backend/api";

const goals = [
  {
    title: "Gana musculo",
    description: "Zonas de fuerza, maquinas, peso libre y rutinas enfocadas en progreso real.",
    image: "/marketing/goals/muscle.jpg",
    href: "/planes",
  },
  {
    title: "Baja grasa",
    description: "Entrenamiento funcional, cardio y clases para moverte con intensidad.",
    image: "/marketing/goals/cardio.jpg",
    href: "/clases",
  },
  {
    title: "Entrena funcional",
    description: "Sesiones dinamicas para mejorar resistencia, movilidad y rendimiento.",
    image: "/marketing/goals/functional.jpg",
    href: "/clases",
  },
  {
    title: "Vive la comunidad",
    description: "Clases grupales, coaches cercanos y una energia que te hace volver.",
    image: "/marketing/goals/community.jpg",
    href: "/sucursales",
  },
];

const testimonials = [
  {
    name: "Andrea M.",
    text: "Dorian me ayudo a crear una rutina real. El ambiente te motiva desde que entras.",
  },
  {
    name: "Carlos R.",
    text: "Las clases son intensas, los coaches estan pendientes y las sedes se sienten modernas.",
  },
  {
    name: "Mateo G.",
    text: "Me gusta que puedo entrenar, reservar y revisar todo desde la app.",
  },
];

function GoalCard({ goal }: { goal: (typeof goals)[number] }) {
  return (
    <a
      href={goal.href}
      className="group relative min-h-[420px] overflow-hidden rounded-[32px] bg-zinc-900"
    >
      <div
        className="absolute inset-0 bg-cover bg-center transition duration-700 group-hover:scale-110"
        style={{ backgroundImage: `url(${goal.image})` }}
      />
      <div className="absolute inset-0 bg-gradient-to-t from-black via-black/45 to-transparent" />
      <div className="absolute bottom-0 p-6">
        <h3 className="font-heading text-3xl font-black text-white">{goal.title}</h3>
        <p className="mt-3 text-sm leading-6 text-white/70">{goal.description}</p>
        <span className="mt-5 inline-flex items-center gap-2 text-sm font-bold text-[var(--accent)]">
          Explorar <ArrowRight className="h-4 w-4" />
        </span>
      </div>
    </a>
  );
}

function FeatureSectionTitle({
  eyebrow,
  title,
  description,
}: {
  eyebrow: string;
  title: string;
  description?: string;
}) {
  return (
    <div>
      <p className="text-xs font-bold uppercase tracking-[0.45em] text-[var(--accent)]">
        {eyebrow}
      </p>
      <h2 className="mt-4 max-w-3xl font-heading text-4xl font-black leading-tight text-white md:text-6xl">
        {title}
      </h2>
      {description ? <p className="mt-5 max-w-2xl text-white/65">{description}</p> : null}
    </div>
  );
}

export default async function HomePage() {
  const { branches, classes, promotions, memberships } = await getPublicData();
  const appUrl = process.env.NEXT_PUBLIC_APP_DOWNLOAD_URL ?? "/";

  const heroSlides: HeroSlide[] = [
    {
      eyebrow: "Dorian Gym",
      title: "Entrena con la energia de una marca que se siente grande.",
      description:
        "Musculacion, clases dirigidas y una identidad visual fuerte para que Dorian se sienta premium desde el primer vistazo.",
      image: "/marketing/hero/dorian-brand.jpg",
      imagePosition: "center center",
      primaryCta: "Conoce Dorian",
      primaryHref: "/planes",
      secondaryCta: "Ver planes",
      secondaryHref: "/planes",
    },
    {
      eyebrow: "Sedes Dorian",
      title: "Varias sucursales. La misma energia en cada entrenamiento.",
      description:
        "Explora nuestras sedes y elige la que mejor se adapte a tu ritmo sin perder la experiencia de marca Dorian.",
      image: "/marketing/hero/dorian-branches.jpg",
      imagePosition: "center 24%",
      primaryCta: "Ver sedes",
      primaryHref: "/sucursales",
      secondaryCta: "Explorar planes",
      secondaryHref: "/planes",
    },
    {
      eyebrow: "Experiencia premium",
      title: "Entrena fuerte y recuperate mejor dentro del club.",
      description:
        "Dorian combina entrenamiento, comunidad y espacios que elevan la experiencia completa del gimnasio.",
      image: "/marketing/hero/dorian-sauna.jpg",
      imagePosition: "center 38%",
      primaryCta: "Ver experiencia",
      primaryHref: "/clases",
      secondaryCta: "Descargar app",
      secondaryHref: appUrl,
    },
  ];

  const featuredClasses = classes.slice(0, 4);
  const featuredBranches = branches.slice(0, 3);
  const featuredPlans = memberships.slice(0, 3);
  const activePromotions = promotions.slice(0, 2);

  return (
    <main className="bg-black text-white">
      <HeroMediaCarousel
        slides={heroSlides}
        stats={[
          { value: branches.length, label: "Sedes" },
          { value: classes.length, label: "Clases" },
          { value: promotions.length, label: "Promos" },
        ]}
      />

      <section className="mx-auto max-w-7xl px-4 py-20 md:px-6">
        <div className="flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <FeatureSectionTitle
            eyebrow="Entrena por objetivo"
            title="Elige como quieres transformar tu cuerpo."
          />
          <Button href="/planes" variant="secondary" className="w-fit gap-2">
            Ver planes <ArrowRight className="h-4 w-4" />
          </Button>
        </div>

        <div className="mt-12 grid gap-5 md:grid-cols-2 lg:grid-cols-4">
          {goals.map((goal) => (
            <GoalCard key={goal.title} goal={goal} />
          ))}
        </div>
      </section>

      <section className="border-y border-white/10 bg-white/[0.03]">
        <div className="mx-auto grid max-w-7xl gap-10 px-4 py-20 md:px-6 lg:grid-cols-[0.8fr_1.2fr]">
          <FeatureSectionTitle
            eyebrow="Sedes"
            title="Tu club, cerca de ti."
            description="Espacios disenados para entrenar con comodidad, intensidad y una experiencia premium."
          />

          <div className="grid gap-5 md:grid-cols-3">
            {featuredBranches.map((branch) => (
              <article
                key={branch.id}
                className="rounded-[30px] border border-white/10 bg-black p-5"
              >
                <div className="mb-6 flex h-12 w-12 items-center justify-center rounded-full bg-[var(--accent)] text-black">
                  <MapPin className="h-5 w-5" />
                </div>
                <h3 className="font-heading text-2xl font-bold text-white">{branch.name}</h3>
                <p className="mt-3 text-sm leading-6 text-white/60">
                  {branch.address ?? "Direccion disponible proximamente."}
                </p>
                <a
                  href="/sucursales"
                  className="mt-6 inline-flex items-center gap-2 text-sm font-bold text-[var(--accent)]"
                >
                  Ver sede <ArrowRight className="h-4 w-4" />
                </a>
              </article>
            ))}
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-20 md:px-6">
        <div className="flex flex-col gap-6 md:flex-row md:items-end md:justify-between">
          <FeatureSectionTitle
            eyebrow="Clases destacadas"
            title="Muevete con proposito."
          />
          <Button href="/clases" variant="secondary" className="w-fit">
            Ver todas
          </Button>
        </div>

        <div className="mt-12 grid gap-5 md:grid-cols-2 lg:grid-cols-4">
          {featuredClasses.map((item, index) => (
            <article
              key={item.id}
              className="rounded-[30px] border border-white/10 bg-zinc-950 p-6 transition hover:-translate-y-1 hover:border-[var(--accent)]/60"
            >
              <div className="mb-8 flex h-12 w-12 items-center justify-center rounded-full bg-white/10 text-[var(--accent)]">
                {index % 2 === 0 ? <Flame className="h-5 w-5" /> : <Dumbbell className="h-5 w-5" />}
              </div>
              <h3 className="font-heading text-2xl font-bold text-white">{item.name}</h3>
              <p className="mt-3 text-sm leading-6 text-white/60">
                {item.description ?? "Clase disenada para mejorar tu condicion fisica."}
              </p>
              <a
                href="/clases"
                className="mt-6 inline-flex items-center gap-2 text-sm font-bold text-[var(--accent)]"
              >
                Reservar <ArrowRight className="h-4 w-4" />
              </a>
            </article>
          ))}
        </div>
      </section>

      <section className="bg-zinc-950">
        <div className="mx-auto max-w-7xl px-4 py-20 md:px-6">
          <div className="text-center">
            <p className="text-xs font-bold uppercase tracking-[0.45em] text-[var(--accent)]">
              Membresias
            </p>
            <h2 className="mx-auto mt-4 max-w-3xl font-heading text-4xl font-black text-white md:text-6xl">
              Un plan para cada forma de entrenar.
            </h2>
          </div>

          <div className="mt-12 grid gap-5 md:grid-cols-3">
            {featuredPlans.map((plan, index) => (
              <article
                key={plan.id}
                className={`rounded-[34px] border p-7 ${
                  index === 1
                    ? "border-[var(--accent)] bg-[var(--accent)] text-black"
                    : "border-white/10 bg-black text-white"
                }`}
              >
                <div className="flex items-center justify-between">
                  <h3 className="font-heading text-3xl font-black">{plan.name}</h3>
                  {index === 1 ? (
                    <span className="rounded-full bg-black px-3 py-1 text-xs font-bold uppercase tracking-widest text-white">
                      Popular
                    </span>
                  ) : null}
                </div>
                <p className={`mt-4 text-sm leading-6 ${index === 1 ? "text-black/70" : "text-white/60"}`}>
                  Acceso a una experiencia de entrenamiento completa.
                </p>
                <div className="mt-8">
                  <p className="font-heading text-5xl font-black">
                    ${plan.price ?? "—"}
                  </p>
                </div>
                <Button
                  href="/planes"
                  variant={index === 1 ? "secondary" : "primary"}
                  className="mt-8 w-full"
                >
                  Elegir plan
                </Button>
              </article>
            ))}
          </div>
        </div>
      </section>

      {activePromotions.length ? (
        <section className="mx-auto max-w-7xl px-4 py-20 md:px-6">
          <div className="grid gap-5 md:grid-cols-2">
            {activePromotions.map((promo) => (
              <article
                key={promo.id}
                className="rounded-[36px] border border-[var(--accent)]/30 bg-gradient-to-br from-[var(--accent)]/20 to-white/[0.03] p-8"
              >
                <Trophy className="h-10 w-10 text-[var(--accent)]" />
                <h3 className="mt-6 font-heading text-4xl font-black text-white">
                  {promo.title}
                </h3>
                <p className="mt-4 text-white/65">{promo.description}</p>
                <Button href="/promociones" className="mt-8">
                  Ver promocion
                </Button>
              </article>
            ))}
          </div>
        </section>
      ) : null}

      <section className="mx-auto max-w-7xl px-4 py-20 md:px-6">
        <div className="relative overflow-hidden rounded-[44px] bg-[var(--accent)] p-8 text-black md:p-14">
          <div className="grid gap-10 lg:grid-cols-[1.1fr_0.9fr] lg:items-center">
            <div>
              <Smartphone className="h-12 w-12" />
              <h2 className="mt-6 max-w-2xl font-heading text-4xl font-black leading-tight md:text-6xl">
                Lleva Dorian en tu bolsillo.
              </h2>
              <p className="mt-5 max-w-xl text-black/70">
                Reserva clases, revisa tu progreso, gestiona tu membresia y accede a tu club desde la app.
              </p>
              <Button href={appUrl} variant="secondary" className="mt-8">
                Descargar app
              </Button>
            </div>

            <div className="rounded-[34px] bg-black p-8 text-white">
              <div className="grid gap-5">
                {["Reservas de clases", "Check-in digital", "Promos y membresias"].map((item) => (
                  <div key={item} className="flex items-center gap-4">
                    <div className="flex h-10 w-10 items-center justify-center rounded-full bg-[var(--accent)] text-black">
                      <Star className="h-4 w-4" />
                    </div>
                    <p className="font-bold">{item}</p>
                  </div>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>

      <section className="mx-auto max-w-7xl px-4 py-20 md:px-6">
        <div className="text-center">
          <p className="text-xs font-bold uppercase tracking-[0.45em] text-[var(--accent)]">
            Comunidad
          </p>
          <h2 className="mx-auto mt-4 max-w-3xl font-heading text-4xl font-black text-white md:text-6xl">
            La energia se contagia.
          </h2>
        </div>

        <div className="mt-12 grid gap-5 md:grid-cols-3">
          {testimonials.map((testimonial) => (
            <article
              key={testimonial.name}
              className="rounded-[32px] border border-white/10 bg-white/[0.03] p-7"
            >
              <Users className="h-8 w-8 text-[var(--accent)]" />
              <p className="mt-6 text-lg leading-8 text-white/80">
                “{testimonial.text}”
              </p>
              <p className="mt-6 font-bold text-[var(--accent)]">{testimonial.name}</p>
            </article>
          ))}
        </div>
      </section>

      <section className="px-4 pb-20 md:px-6">
        <div className="mx-auto max-w-7xl rounded-[44px] bg-white p-8 text-black md:p-14">
          <div className="flex flex-col gap-8 md:flex-row md:items-center md:justify-between">
            <div>
              <p className="text-xs font-bold uppercase tracking-[0.45em] text-[var(--accent)]">
                Empieza hoy
              </p>
              <h2 className="mt-4 max-w-3xl font-heading text-4xl font-black md:text-6xl">
                Tu proxima rutina comienza en Dorian.
              </h2>
            </div>
            <div className="flex flex-col gap-3 sm:flex-row">
              <Button href="/planes" className="gap-2">
                Unete ahora <ArrowRight className="h-4 w-4" />
              </Button>
              <Button href="/sucursales" variant="secondary">
                Ver sedes
              </Button>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
