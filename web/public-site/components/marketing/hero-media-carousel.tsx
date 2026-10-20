"use client";

import Image from "next/image";
import { useEffect, useState } from "react";
import { ArrowRight, ChevronLeft, ChevronRight } from "lucide-react";
import { Button } from "@/components/ui/button";

export type HeroSlide = {
  eyebrow: string;
  title: string;
  description: string;
  image?: string;
  imagePosition?: string;
  video?: string;
  primaryCta: string;
  primaryHref: string;
  secondaryCta: string;
  secondaryHref: string;
};

type HeroStat = {
  value: number;
  label: string;
};

type HeroMediaCarouselProps = {
  slides: HeroSlide[];
  stats?: HeroStat[];
};


function SlideFallback({ title }: { title: string }) {
  return (
    <div className="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,rgba(255,95,0,0.28),transparent_22%),radial-gradient(circle_at_80%_25%,rgba(255,255,255,0.08),transparent_18%),linear-gradient(135deg,#171717_0%,#080808_50%,#120904_100%)]">
      <div className="absolute inset-0 bg-[linear-gradient(120deg,rgba(255,255,255,0.06),transparent_28%,rgba(255,95,0,0.12)_100%)]" />
      <div className="absolute bottom-10 right-10 hidden rounded-full border border-white/10 bg-black/25 px-4 py-2 text-xs font-semibold uppercase tracking-[0.28em] text-white/60 md:block">
        {title}
      </div>
    </div>
  );
}

export function HeroMediaCarousel({ slides, stats = [] }: HeroMediaCarouselProps) {
  const [activeIndex, setActiveIndex] = useState(0);

  useEffect(() => {
    if (slides.length <= 1) return;

    const timer = window.setInterval(() => {
      setActiveIndex((current) => (current + 1) % slides.length);
    }, 7000);

    return () => window.clearInterval(timer);
  }, [slides.length]);

  if (!slides.length) return null;

  const slide = slides[activeIndex];

  function previous() {
    setActiveIndex((current) => (current - 1 + slides.length) % slides.length);
  }

  function next() {
    setActiveIndex((current) => (current + 1) % slides.length);
  }

  return (
    <section className="relative min-h-screen overflow-hidden bg-black">
      {slide.video ? (
        <video
          key={slide.video}
          className="absolute inset-0 h-full w-full object-cover"
          autoPlay
          muted
          loop
          playsInline
          poster={slide.image}
        >
          <source src={slide.video} type="video/mp4" />
        </video>
      ) : slide.image ? (
        <Image
          key={slide.image}
          src={slide.image}
          alt={slide.title}
          fill
          priority
          sizes="100vw"
          className="object-cover transition duration-700"
          style={{ objectPosition: slide.imagePosition ?? "center center" }}
        />
      ) : (
        <SlideFallback title={slide.title} />
      )}

      <div className="absolute inset-0 bg-black/52" />
      <div className="absolute inset-0 bg-[linear-gradient(90deg,rgba(0,0,0,0.95)_0%,rgba(0,0,0,0.74)_36%,rgba(0,0,0,0.18)_100%)]" />
      <div className="absolute inset-0 bg-[linear-gradient(180deg,rgba(0,0,0,0.56)_0%,rgba(0,0,0,0.16)_32%,rgba(0,0,0,0.92)_100%)]" />
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_top_left,rgba(255,95,0,0.14),transparent_18%)]" />
      <div className="pointer-events-none absolute left-[-10%] top-[18%] z-10 h-64 w-64 rounded-full bg-[var(--accent)]/20 blur-[90px]" />

      <div className="relative z-20 mx-auto flex min-h-screen max-w-7xl flex-col px-4 pb-8 pt-6 md:px-6 md:pb-12 md:pt-8 lg:pb-16">
        <div className="flex items-center justify-between gap-6">
          <a
            href="/"
            className="rounded-full border border-white/10 bg-white/[0.05] px-4 py-2 font-mono text-[11px] font-medium uppercase tracking-[0.24em] text-white/84 backdrop-blur-xl"
          >
            Dorian Public Club
          </a>
          <div className="hidden rounded-full border border-white/10 bg-black/28 px-4 py-2 font-mono text-[11px] uppercase tracking-[0.24em] text-white/54 backdrop-blur-xl md:flex">
            {String(activeIndex + 1).padStart(2, "0")} / {String(slides.length).padStart(2, "0")}
          </div>
        </div>

        <div className="flex flex-1 items-end pb-12 pt-20 md:pb-16 lg:pb-20">
          <div key={slide.title} className="max-w-4xl">
            <div className="mb-6 inline-flex items-center gap-3 rounded-full border border-[var(--accent)]/25 bg-black/35 px-4 py-2 backdrop-blur-xl">
              <span className="h-2 w-2 rounded-full bg-[var(--accent)] shadow-[0_0_20px_rgba(255,95,0,0.65)]" />
              <p className="font-mono text-[11px] font-medium uppercase tracking-[0.28em] text-[var(--accent)]">
                {slide.eyebrow}
              </p>
            </div>

            <h1 className="max-w-4xl font-heading text-[2.4rem] font-black uppercase leading-[0.92] tracking-[-0.03em] text-white md:text-[3.6rem] lg:text-[3.2rem]">
              {slide.title}
            </h1>

            <p className="mt-6 max-w-xl text-base leading-8 text-[var(--text-secondary)] md:text-lg">
              {slide.description}
            </p>

            <div className="mt-9 flex flex-col gap-3 sm:flex-row">
              <Button href={slide.primaryHref} className="gap-2 px-7 py-3.5 font-mono text-[11px] uppercase tracking-[0.22em] shadow-[0_0_32px_rgba(255,95,0,0.22)]">
                {slide.primaryCta}
                <ArrowRight className="h-4 w-4" />
              </Button>

              <Button href={slide.secondaryHref} variant="secondary" className="border border-white/10 px-7 py-3.5 font-mono text-[11px] uppercase tracking-[0.22em]">
                {slide.secondaryCta}
              </Button>
            </div>
          </div>
        </div>

        <div className="mt-auto flex flex-col gap-8 border-t border-white/12 pt-6 md:flex-row md:items-end md:justify-between">
          <div className="grid grid-cols-3 gap-4 md:gap-6">
            {stats.map((stat) => (
              <div
                key={stat.label}
                className="rounded-[24px] border border-white/10 bg-white/[0.04] px-4 py-4 backdrop-blur-xl"
              >
                <p className="font-heading text-3xl font-black text-white md:text-4xl">{stat.value}</p>
                <p className="mt-2 font-mono text-[10px] font-medium uppercase tracking-[0.24em] text-white/42">
                  {stat.label}
                </p>
              </div>
            ))}
          </div>

          <div className="flex items-center gap-4">
            <button
              type="button"
              onClick={previous}
              className="grid h-11 w-11 place-items-center rounded-full border border-white/16 bg-white/8 text-white transition hover:bg-white/14"
              aria-label="Slide anterior"
            >
              <ChevronLeft className="h-5 w-5" />
            </button>

            <button
              type="button"
              onClick={next}
              className="grid h-11 w-11 place-items-center rounded-full border border-white/16 bg-white/8 text-white transition hover:bg-white/14"
              aria-label="Siguiente slide"
            >
              <ChevronRight className="h-5 w-5" />
            </button>

            <div className="flex items-center gap-2">
              {slides.map((item, index) => (
                <button
                  key={`${item.title}-${index}`}
                  type="button"
                  onClick={() => setActiveIndex(index)}
                  className={`h-1.5 rounded-full transition-all ${
                    index === activeIndex ? "w-12 bg-[var(--accent)]" : "w-5 bg-white/24"
                  }`}
                  aria-label={`Ir al slide ${index + 1}`}
                />
              ))}
            </div>

            <p className="hidden font-mono text-[11px] font-medium uppercase tracking-[0.24em] text-white/44 md:block">
              {String(activeIndex + 1).padStart(2, "0")} / {String(slides.length).padStart(2, "0")}
            </p>
          </div>
        </div>
      </div>
    </section>
  );
}


