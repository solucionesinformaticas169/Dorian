import Link from "next/link";
import { Dumbbell } from "lucide-react";
import { Button } from "@/components/ui/button";

const items = [
  { href: "/", label: "Home" },
  { href: "/sucursales", label: "Sucursales" },
  { href: "/clases", label: "Clases" },
  { href: "/promociones", label: "Promociones" },
  { href: "/planes", label: "Planes" },
  { href: "/contacto", label: "Contacto" },
];

export function SiteHeader() {
  return (
    <header className="sticky top-0 z-40 border-b border-white/10 bg-slate-950/70 backdrop-blur">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-4 md:px-6">
        <Link href="/" className="flex items-center gap-3 text-white">
          <div className="rounded-2xl bg-[var(--accent)]/15 p-3 text-[var(--accent)]">
            <Dumbbell className="h-5 w-5" />
          </div>
          <div>
            <p className="font-heading text-lg font-semibold">Dorian Fitness</p>
            <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Premium training club</p>
          </div>
        </Link>
        <nav className="hidden items-center gap-6 lg:flex">
          {items.map((item) => (
            <Link key={item.href} href={item.href} className="text-sm text-slate-300 transition hover:text-white">
              {item.label}
            </Link>
          ))}
        </nav>
        <Button href={process.env.NEXT_PUBLIC_APP_DOWNLOAD_URL ?? "https://example.com/app"}>Descargar app</Button>
      </div>
    </header>
  );
}