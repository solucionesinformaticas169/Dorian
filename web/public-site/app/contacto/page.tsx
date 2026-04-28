import type { Metadata } from "next";
import { Mail, MapPin, MessageCircle, Smartphone } from "lucide-react";
import { Button } from "@/components/ui/button";
import { SectionTitle } from "@/components/ui/section-title";

export const metadata: Metadata = {
  title: "Contacto",
  description: "Conecta con Dorian Fitness por WhatsApp, correo o descarga la app para comenzar tu experiencia.",
};

export default function ContactPage() {
  const whatsappHref = process.env.NEXT_PUBLIC_WHATSAPP_URL ?? "https://wa.me/593999999999";
  const appHref = process.env.NEXT_PUBLIC_APP_DOWNLOAD_URL ?? "https://example.com/app";

  return (
    <main className="mx-auto max-w-7xl px-4 py-16 md:px-6">
      <SectionTitle eyebrow="Conecta" title="Hablemos de tu siguiente entrenamiento" description="Nuestro equipo puede guiarte para elegir sede, plan o forma de empezar desde la app." />
      <div className="mt-10 grid gap-5 lg:grid-cols-[1.1fr_0.9fr]">
        <div className="rounded-[32px] border border-white/10 bg-white/[0.04] p-8">
          <div className="space-y-5 text-slate-300">
            <p className="flex items-center gap-3"><MessageCircle className="h-5 w-5 text-[var(--accent)]" /> WhatsApp directo para inscripciones y dudas</p>
            <p className="flex items-center gap-3"><Mail className="h-5 w-5 text-[var(--accent)]" /> hola@dorianfitness.com</p>
            <p className="flex items-center gap-3"><MapPin className="h-5 w-5 text-[var(--accent)]" /> Quito y Guayaquil</p>
            <p className="flex items-center gap-3"><Smartphone className="h-5 w-5 text-[var(--accent)]" /> Gestiona reservas y accesos desde la app</p>
          </div>
          <div className="mt-8 flex flex-col gap-4 sm:flex-row">
            <Button href={whatsappHref}>Escribir por WhatsApp</Button>
            <Button href={appHref} variant="secondary">Descargar app</Button>
          </div>
        </div>
        <div className="rounded-[32px] border border-white/10 bg-gradient-to-br from-emerald-500/10 via-slate-950/70 to-sky-500/10 p-8">
          <p className="text-xs font-semibold uppercase tracking-[0.34em] text-[var(--accent)]">Member journey</p>
          <h3 className="mt-4 font-heading text-3xl font-semibold text-white">Empieza hoy y entrena con una experiencia conectada.</h3>
          <p className="mt-4 text-sm leading-7 text-slate-300">Descarga la app, revisa clases, valida tu acceso con QR y mantente cerca de tu progreso desde cualquier sucursal activa.</p>
        </div>
      </div>
    </main>
  );
}