import Link from "next/link";

export function SiteFooter() {
  return (
    <footer className="border-t border-white/10 bg-black/30">
      <div className="mx-auto grid max-w-7xl gap-6 px-4 py-10 text-sm text-slate-400 md:grid-cols-3 md:px-6">
        <div>
          <p className="font-heading text-lg text-white">Dorian Fitness</p>
          <p className="mt-2 max-w-sm">Entrenamiento premium, clases dirigidas y experiencia digital conectada con tu progreso.</p>
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
          <p>Quito · Guayaquil</p>
        </div>
      </div>
    </footer>
  );
}