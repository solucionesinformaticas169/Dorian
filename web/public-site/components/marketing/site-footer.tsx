import Image from "next/image";
import Link from "next/link";

export function SiteFooter() {
  return (
    <footer className="border-t border-white/10 bg-black/30">
      <div className="mx-auto grid max-w-7xl gap-6 px-4 py-10 text-sm text-slate-400 md:grid-cols-3 md:px-6">
        <div>
          <div className="flex items-center gap-3">
            <div className="flex h-12 w-12 items-center justify-center rounded-2xl border border-white/10 bg-white/5 p-2">
              <Image src="/brand/dorian-logo.png" alt="Gimnasio Dorian" width={36} height={36} className="h-auto w-full" />
            </div>
            <div>
              <p className="font-heading text-lg text-white">Gimnasio Dorian</p>
              <p className="text-xs uppercase tracking-[0.22em] text-slate-500">Marca oficial</p>
            </div>
          </div>
          <p className="mt-3 max-w-sm">Entrenamiento premium, clases dirigidas y experiencia digital conectada con tu progreso.</p>
        </div>
        <div>
          <p className="font-semibold uppercase tracking-[0.24em] text-slate-500">Navegacion</p>
          <div className="mt-3 flex flex-col gap-2">
            <Link href="/sucursales">Sucursales</Link>
            <Link href="/clases">Clases</Link>
            <Link href="/promociones">Promociones</Link>
            <Link href="/planes">Planes</Link>
          </div>
        </div>
        <div>
          <p className="font-semibold uppercase tracking-[0.24em] text-slate-500">Contacto</p>
          <p className="mt-3">hola@dorianfitness.com</p>
          <p>Cuenca · Azogues</p>
        </div>
      </div>
    </footer>
  );
}
