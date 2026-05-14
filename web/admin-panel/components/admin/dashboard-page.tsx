"use client";

import { BarChart3, TrendingUp, UsersRound } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { branchesApi, classesApi, dashboardApi } from "@/lib/api/admin";
import { EmptyState, LoadingState } from "@/components/admin/page-state";
import { SectionHeading } from "@/components/admin/section-heading";
import { StatCard } from "@/components/admin/stat-card";
import { Alert } from "@/components/ui/alert";
import { Card } from "@/components/ui/card";
import { formatCurrency, formatDateTime, getErrorMessage } from "@/lib/utils";

function getLocalDayKey(value: Date | string) {
  return new Intl.DateTimeFormat("en-CA", {
    timeZone: "America/Guayaquil",
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
  }).format(new Date(value));
}

export function DashboardPage() {
  const summaryQuery = useQuery({ queryKey: ["dashboard-summary"], queryFn: dashboardApi.summary });
  const branchesQuery = useQuery({ queryKey: ["dashboard-branches"], queryFn: branchesApi.list });
  const classesQuery = useQuery({ queryKey: ["dashboard-classes"], queryFn: classesApi.list });

  if (summaryQuery.isLoading || branchesQuery.isLoading || classesQuery.isLoading) {
    return <LoadingState label="Cargando metricas operativas de Dorian..." />;
  }

  if (summaryQuery.error || branchesQuery.error || classesQuery.error) {
    return <Alert>{getErrorMessage(summaryQuery.error ?? branchesQuery.error ?? classesQuery.error)}</Alert>;
  }

  const summary = summaryQuery.data;
  if (!summary) {
    return <EmptyState title="No hay métricas disponibles" description="El dashboard se activará cuando existan sucursales, clientes y operación mínima para la demo." />;
  }

  const branches = branchesQuery.data ?? [];
  const classes = classesQuery.data ?? [];
  const todayKey = getLocalDayKey(new Date());

  const computedBranchActivity = branches
    .map((branch) => {
      const branchClasses = classes.filter((item) => item.branchId === branch.id);
      const todayClassesCount = branchClasses.filter((item) => getLocalDayKey(item.startTime) === todayKey && item.status === 1).length;
      return {
        branchId: branch.id,
        branchName: branch.name,
        createdClassesCount: branchClasses.length,
        todayClassesCount,
      };
    })
    .sort((left, right) => right.createdClassesCount - left.createdClassesCount || left.branchName.localeCompare(right.branchName));

  const computedClassOccupancy = classes
    .filter((item) => item.status === 1 && getLocalDayKey(item.startTime) === todayKey)
    .map((item) => ({
      classSessionId: item.id,
      className: item.name,
      branchName: branches.find((branch) => branch.id === item.branchId)?.name ?? item.branchId,
      startTime: item.startTime,
      reservedSpots: item.reservedSpots,
      capacity: item.capacity,
      occupancyRate: item.capacity > 0 ? (item.reservedSpots / item.capacity) * 100 : 0,
    }))
    .sort((left, right) => right.occupancyRate - left.occupancyRate || new Date(left.startTime).getTime() - new Date(right.startTime).getTime());

  const maxBranchActivity = Math.max(...computedBranchActivity.map((item) => item.createdClassesCount), 1);
  const maxOccupancy = Math.max(...computedClassOccupancy.map((item) => item.occupancyRate), 1);
  const todayClassesCount = computedClassOccupancy.length;
  const mostActiveBranchName = computedBranchActivity[0]?.branchName ?? "Sin actividad";

  return (
    <div className="space-y-6">
      <SectionHeading
        eyebrow="Control room"
        title="Dashboard operativo Dorian"
        description="Vista consolidada del rendimiento actual del gimnasio, con clientes, clases y ocupación en el modelo multi-sucursal."
      />

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <StatCard label="Clientes activos" value={summary.activeCustomersCount} helper="Miembros activos de Dorian, ya no ligados a una sola sucursal" />
        <StatCard label="Clases del día" value={todayClassesCount} helper="Sesiones programadas hoy en horario local de Ecuador" />
        <StatCard label="Ingresos estimados" value={formatCurrency(summary.estimatedRevenue)} helper="Basado en planes activos registrados" />
        <StatCard label="Sucursal con más clases" value={mostActiveBranchName} helper="Sede con mayor cantidad de clases creadas" />
      </div>

      <div className="grid gap-4 xl:grid-cols-[1.1fr_0.9fr]">
        <Card>
          <div className="flex items-start justify-between gap-4">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Clases por sucursal</p>
              <h3 className="mt-3 font-heading text-2xl text-white">Capacidad operativa por sede</h3>
              <p className="mt-2 max-w-2xl text-sm text-slate-400">Cada tarjeta muestra cuántas clases tiene creadas la sucursal y cuántas corren hoy.</p>
            </div>
            <span className="inline-flex rounded-2xl border border-white/10 bg-white/[0.04] p-3 text-[var(--accent)]">
              <TrendingUp className="h-5 w-5" />
            </span>
          </div>

          <div className="mt-6 space-y-4">
            {computedBranchActivity.map((item) => (
              <div key={item.branchId} className="rounded-[26px] border border-white/10 bg-white/[0.03] p-4">
                <div className="flex items-center justify-between gap-4">
                  <div>
                    <p className="font-semibold text-white">{item.branchName}</p>
                    <p className="mt-1 text-xs uppercase tracking-[0.24em] text-slate-500">
                      {item.createdClassesCount} creadas | {item.todayClassesCount} hoy
                    </p>
                  </div>
                  <p className="font-heading text-2xl text-white">{item.createdClassesCount}</p>
                </div>
                <div className="mt-4 h-2 rounded-full bg-white/5">
                  <div
                    className="h-2 rounded-full bg-[linear-gradient(90deg,var(--accent),#ffd2b5)]"
                    style={{ width: `${Math.max((item.createdClassesCount / maxBranchActivity) * 100, 8)}%` }}
                  />
                </div>
              </div>
            ))}
          </div>
        </Card>

        <Card>
          <div className="flex items-start justify-between gap-4">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Ocupación de clases</p>
              <h3 className="mt-3 font-heading text-2xl text-white">Aforo de hoy</h3>
              <p className="mt-2 text-sm text-slate-400">Refleja cuantas reservas activas tiene cada clase de hoy frente a su capacidad, usando horario local de Ecuador.</p>
            </div>
            <span className="inline-flex rounded-2xl border border-white/10 bg-white/[0.04] p-3 text-[var(--accent)]">
              <BarChart3 className="h-5 w-5" />
            </span>
          </div>

          <div className="mt-6 space-y-4">
            {computedClassOccupancy.length ? (
              computedClassOccupancy.map((item) => (
                <div key={item.classSessionId} className="rounded-[24px] border border-white/10 bg-white/[0.03] p-4">
                  <div className="flex items-center justify-between gap-3">
                    <div>
                      <p className="font-semibold text-white">{item.className}</p>
                      <p className="mt-1 text-sm text-slate-400">{item.branchName} | {formatDateTime(item.startTime)}</p>
                    </div>
                    <p className="font-heading text-xl text-white">{item.occupancyRate.toFixed(0)}%</p>
                  </div>
                  <div className="mt-4 h-2 rounded-full bg-white/5">
                    <div
                      className="h-2 rounded-full bg-[linear-gradient(90deg,#ff8a3d,var(--accent))]"
                      style={{ width: `${Math.max((item.occupancyRate / maxOccupancy) * 100, item.occupancyRate > 0 ? 10 : 0)}%` }}
                    />
                  </div>
                  <p className="mt-2 text-xs uppercase tracking-[0.24em] text-slate-500">{item.reservedSpots}/{item.capacity} cupos reservados</p>
                </div>
              ))
            ) : (
              <div className="rounded-[24px] border border-dashed border-white/10 bg-white/[0.03] p-5 text-sm text-slate-400">
                Aún no hay clases programadas para hoy dentro del alcance de este usuario.
              </div>
            )}
          </div>
        </Card>
      </div>

      <div className="grid gap-4 lg:grid-cols-[0.95fr_1.05fr]">
        <Card>
          <div className="flex items-start justify-between gap-4">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Ingresos estimados</p>
              <h3 className="mt-3 font-heading text-2xl text-white">{formatCurrency(summary.estimatedRevenue)}</h3>
              <p className="mt-2 text-sm text-slate-400">{summary.estimatedRevenueFormula}</p>
            </div>
            <span className="inline-flex rounded-2xl border border-white/10 bg-white/[0.04] p-3 text-[var(--accent)]">
              <UsersRound className="h-5 w-5" />
            </span>
          </div>
        </Card>

        <Card>
          <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Lectura rapida</p>
          <div className="mt-4 grid gap-3 md:grid-cols-3">
            <MiniMetric label="Clientes activos" value={summary.activeCustomersCount.toString()} />
            <MiniMetric label="Clases hoy" value={todayClassesCount.toString()} />
            <MiniMetric label="Ingresos estimados" value={formatCurrency(summary.estimatedRevenue)} />
          </div>
        </Card>
      </div>
    </div>
  );
}

function MiniMetric({ label, value }: { label: string; value: string }) {
  return (
    <div className="rounded-[22px] border border-white/10 bg-white/[0.03] p-4">
      <p className="text-xs uppercase tracking-[0.24em] text-slate-500">{label}</p>
      <p className="mt-2 font-heading text-2xl text-white">{value}</p>
    </div>
  );
}


