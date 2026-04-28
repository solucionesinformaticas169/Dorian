"use client";

import { useMemo } from "react";
import { useQueries, useQuery } from "@tanstack/react-query";
import { accessApi, bookingsApi, branchesApi, classesApi, customersApi, membershipsApi, promotionsApi } from "@/lib/api/admin";
import { EmptyState, LoadingState } from "@/components/admin/page-state";
import { SectionHeading } from "@/components/admin/section-heading";
import { StatCard } from "@/components/admin/stat-card";
import { Alert } from "@/components/ui/alert";
import { Card } from "@/components/ui/card";
import { getErrorMessage } from "@/lib/utils";

export function DashboardPage() {
  const { data: branches, isLoading: branchesLoading, error: branchesError } = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const statsQueries = useQueries({
    queries: [
      { queryKey: ["customers"], queryFn: customersApi.list },
      { queryKey: ["memberships"], queryFn: membershipsApi.list },
      { queryKey: ["classes"], queryFn: classesApi.list },
      { queryKey: ["bookings"], queryFn: bookingsApi.list },
      { queryKey: ["promotions"], queryFn: promotionsApi.list },
    ],
  });

  const selectedBranchId = branches?.[0]?.id;
  const { data: checkIns } = useQuery({
    queryKey: ["checkins", selectedBranchId],
    queryFn: () => accessApi.getCheckInsByBranch(selectedBranchId!),
    enabled: Boolean(selectedBranchId),
  });

  const isLoading = branchesLoading || statsQueries.some((query) => query.isLoading);
  const error = branchesError ?? statsQueries.find((query) => query.error)?.error;

  const metrics = useMemo(() => {
    const [customers, memberships, classes, bookings, promotions] = statsQueries.map((query) => query.data ?? []);
    return {
      branches: branches?.length ?? 0,
      customers: customers.length,
      memberships: memberships.length,
      classes: classes.length,
      bookings: bookings.length,
      promotions: promotions.length,
      acceptedCheckIns: (checkIns ?? []).filter((checkIn) => checkIn.status === 1).length,
      rejectedCheckIns: (checkIns ?? []).filter((checkIn) => checkIn.status === 2).length,
    };
  }, [branches, checkIns, statsQueries]);

  if (isLoading) return <LoadingState label="Construyendo el tablero ejecutivo..." />;
  if (error) return <Alert>{getErrorMessage(error)}</Alert>;
  if (!branches?.length) return <EmptyState title="No hay sucursales registradas" description="Crea la primera sucursal para comenzar a operar el gimnasio desde el panel." />;

  return (
    <div className="space-y-6">
      <SectionHeading
        eyebrow="Executive overview"
        title="Dashboard operacional"
        description="Monitorea en una sola vista el pulso comercial y operativo del gimnasio multi-sucursal."
      />

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <StatCard label="Sucursales" value={metrics.branches} helper="Red activa en la plataforma" />
        <StatCard label="Clientes" value={metrics.customers} helper="Perfiles disponibles para operacion" />
        <StatCard label="Membresias" value={metrics.memberships} helper="Catalogo activo para venta y renovacion" />
        <StatCard label="Clases" value={metrics.classes} helper="Sesiones configuradas para agenda" />
        <StatCard label="Reservas" value={metrics.bookings} helper="Movimientos recientes del calendario" />
        <StatCard label="Promociones" value={metrics.promotions} helper="Campanas visibles en web y app" />
        <StatCard label="Check-ins OK" value={metrics.acceptedCheckIns} helper={`Primer filtro sobre ${branches[0].name}`} />
        <StatCard label="Check-ins rechazados" value={metrics.rejectedCheckIns} helper="Casos para seguimiento en recepcion" />
      </div>

      <div className="grid gap-4 xl:grid-cols-[1.1fr_0.9fr]">
        <Card>
          <SectionHeading
            eyebrow="Insights"
            title="Ritmo del negocio"
            description="El tablero usa endpoints reales del backend, sin mocks, para que la demo refleje el estado actual del MVP."
          />
          <div className="mt-6 grid gap-4 md:grid-cols-2">
            <div className="rounded-3xl border border-white/10 bg-white/[0.03] p-5">
              <p className="text-sm text-slate-400">Sucursal destacada</p>
              <p className="mt-2 font-heading text-2xl text-white">{branches[0].name}</p>
              <p className="mt-1 text-sm text-slate-400">{branches[0].city} · {branches[0].phoneNumber || "Sin telefono"}</p>
            </div>
            <div className="rounded-3xl border border-white/10 bg-white/[0.03] p-5">
              <p className="text-sm text-slate-400">Promociones activas</p>
              <p className="mt-2 font-heading text-2xl text-white">{(statsQueries[4].data ?? []).filter((promotion) => promotion.status === 2).length}</p>
              <p className="mt-1 text-sm text-slate-400">Listas para impulsar captacion y retencion</p>
            </div>
          </div>
        </Card>

        <Card>
          <SectionHeading
            eyebrow="Access live"
            title="Control de ingreso"
            description="Resumen inicial del flujo QR para la primera sucursal disponible."
          />
          <div className="mt-6 space-y-4">
            {(checkIns ?? []).slice(0, 5).map((checkIn) => (
              <div key={checkIn.id} className="rounded-3xl border border-white/10 bg-white/[0.03] p-4">
                <p className="text-sm text-white">Cliente {checkIn.customerId.slice(0, 8)}</p>
                <p className="mt-1 text-xs uppercase tracking-[0.24em] text-slate-500">{checkIn.status === 1 ? "Aceptado" : "Rechazado"}</p>
                <p className="mt-2 text-sm text-slate-400">{checkIn.rejectionReason || "Ingreso validado correctamente."}</p>
              </div>
            ))}
            {!checkIns?.length ? <p className="text-sm text-slate-400">Aun no hay registros de ingreso para esta sucursal.</p> : null}
          </div>
        </Card>
      </div>
    </div>
  );
}

