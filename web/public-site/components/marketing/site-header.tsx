import Image from "next/image";
import Link from "next/link";
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
    <header className="sticky top-0 z-40 border-b border-white/10 bg-black/70 backdrop-blur">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-4 py-4 md:px-6">
        <Link href="/" className="flex items-center gap-3 text-white">
          <div className="flex h-14 w-14 items-center justify-center rounded-2xl border border-white/10 bg-white/5 p-2">
            <Image src="/brand/dorian-logo.png" alt="Gimnasio Dorian" width={40} height={40} className="h-auto w-full" priority />
          </div>
          <div>
            <p className="font-heading text-lg font-semibold">Gimnasio Dorian</p>
            <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Cuenca y Azogues</p>
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
