"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { Activity, Building2, CalendarClock, CreditCard, LayoutDashboard, LogOut, Megaphone, QrCode, Users, WalletCards } from "lucide-react";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import type { Session } from "@/lib/types";
import { authApi } from "@/lib/api/admin";

const navigation = [
  { href: "/dashboard", label: "Dashboard", icon: LayoutDashboard },
  { href: "/branches", label: "Sucursales", icon: Building2 },
  { href: "/customers", label: "Clientes", icon: Users },
  { href: "/memberships", label: "Membresias", icon: WalletCards },
  { href: "/classes", label: "Clases", icon: Activity },
  { href: "/bookings", label: "Reservas", icon: CalendarClock },
  { href: "/promotions", label: "Promociones", icon: Megaphone },
  { href: "/access", label: "Check-ins QR", icon: QrCode },
];

export function AdminShell({ session, children }: { session: Session; children: React.ReactNode }) {
  const pathname = usePathname();
  const router = useRouter();

  async function handleLogout() {
    await authApi.logout();
    router.replace("/login");
    router.refresh();
  }

  return (
    <div className="min-h-screen bg-[radial-gradient(circle_at_top,_rgba(35,214,154,0.18),_transparent_24%),linear-gradient(180deg,_#06131f_0%,_#04070d_55%,_#02040a_100%)] text-white">
      <div className="mx-auto grid min-h-screen max-w-[1600px] gap-6 px-4 py-4 lg:grid-cols-[280px_minmax(0,1fr)] lg:px-6">
        <aside className="flex flex-col rounded-[32px] border border-white/10 bg-slate-950/80 p-5 shadow-2xl shadow-black/30 backdrop-blur">
          <div className="mb-8 rounded-[28px] border border-white/10 bg-white/[0.03] p-5">
            <p className="text-xs font-semibold uppercase tracking-[0.32em] text-[var(--accent)]">Dorian</p>
            <h1 className="mt-3 font-heading text-2xl font-semibold">Admin Fitness Suite</h1>
            <p className="mt-2 text-sm text-slate-400">Panel premium para operaciones, membresias, promociones y control de acceso.</p>
          </div>

          <nav className="flex-1 space-y-2">
            {navigation.map((item) => {
              const Icon = item.icon;
              const isActive = pathname === item.href;
              return (
                <Link
                  key={item.href}
                  href={item.href}
                  className={cn(
                    "flex items-center gap-3 rounded-2xl px-4 py-3 text-sm font-medium transition",
                    isActive ? "bg-[var(--accent)]/15 text-[var(--accent)]" : "text-slate-300 hover:bg-white/6 hover:text-white",
                  )}
                >
                  <Icon className="h-4 w-4" />
                  {item.label}
                </Link>
              );
            })}
          </nav>

          <div className="mt-6 rounded-[28px] border border-white/10 bg-white/[0.03] p-4">
            <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Sesion activa</p>
            <p className="mt-3 font-heading text-lg text-white">{session.user.fullName}</p>
            <p className="text-sm text-slate-400">{session.user.email}</p>
            <div className="mt-3 flex flex-wrap gap-2 text-xs text-slate-300">
              {session.user.roles.map((role) => (
                <span key={role} className="rounded-full bg-white/8 px-3 py-1">
                  {role}
                </span>
              ))}
            </div>
            <Button variant="secondary" className="mt-4 w-full justify-center" onClick={handleLogout}>
              <LogOut className="mr-2 h-4 w-4" />
              Cerrar sesion
            </Button>
          </div>
        </aside>

        <div className="space-y-6">
          <header className="flex flex-col gap-4 rounded-[32px] border border-white/10 bg-white/[0.03] px-6 py-5 backdrop-blur md:flex-row md:items-center md:justify-between">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.32em] text-[var(--accent)]">Operacion multi-sucursal</p>
              <h2 className="mt-2 font-heading text-3xl font-semibold">Centro de control del gimnasio</h2>
            </div>
            <div className="grid grid-cols-2 gap-3 md:flex">
              <div className="rounded-2xl border border-white/10 bg-slate-950/70 px-4 py-3">
                <p className="text-xs uppercase tracking-[0.2em] text-slate-500">Rol principal</p>
                <p className="mt-1 text-sm text-white">{session.user.roles[0] ?? "Sin rol"}</p>
              </div>
              <div className="rounded-2xl border border-white/10 bg-slate-950/70 px-4 py-3">
                <p className="text-xs uppercase tracking-[0.2em] text-slate-500">Sucursal</p>
                <p className="mt-1 text-sm text-white">{session.user.branchId ?? "Global"}</p>
              </div>
            </div>
          </header>
          <main>{children}</main>
        </div>
      </div>
    </div>
  );
}

