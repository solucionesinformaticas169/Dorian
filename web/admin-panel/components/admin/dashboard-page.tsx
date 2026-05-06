"use client";

import { BarChart3, TrendingUp, UsersRound } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { dashboardApi } from "@/lib/api/admin";
import { EmptyState, LoadingState } from "@/components/admin/page-state";
import { SectionHeading } from "@/components/admin/section-heading";
import { StatCard } from "@/components/admin/stat-card";
import { Alert } from "@/components/ui/alert";
import { Card } from "@/components/ui/card";
import { formatCurrency, formatDateTime, getErrorMessage } from "@/lib/utils";

export function DashboardPage() {
  const summaryQuery = useQuery({ queryKey: ["dashboard-summary"], queryFn: dashboardApi.summary });

  if (summaryQuery.isLoading) {
    return <LoadingState label="Cargando metricas operativas de Dorian..." />;
  }

  if (summaryQuery.error) {
    return <Alert>{getErrorMessage(summaryQuery.error)}</Alert>;
  }

  const summary = summaryQuery.data;
  if (!summary) {
    return <EmptyState title="No hay métricas disponibles" description="El dashboard se activará cuando existan sucursales, clientes y operación mínima para la demo." />;
  }

  const maxBranchActivity = Math.max(...summary.branchActivity.map((item) => item.activityCount), 1);
  const maxOccupancy = Math.max(...summary.classOccupancy.map((item) => item.occupancyRate), 1);

  return (
    <div className="space-y-6">
      <SectionHeading
        eyebrow="Control room"
        title="Dashboard operativo Dorian"
        description="Vista consolidada del rendimiento actual del gimnasio, con metricas reales de clientes, check-ins y ocupacion para la presentacion comercial."
      />

      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-5">
        <StatCard label="Clientes activos" value={summary.activeCustomersCount} helper="Perfiles activos dentro del alcance del rol actual" />
        <StatCard label="Clases del día" value={summary.todayClassesCount} helper="Sesiones programadas para hoy" />
        <StatCard label="Check-ins del día" value={summary.todayCheckInsCount} helper="Ingresos aceptados durante la jornada" />
        <StatCard label="Ingresos estimados" value={formatCurrency(summary.estimatedRevenue)} helper="Basado en membresías activas asignadas" />
        <StatCard label="Sucursal líder" value={summary.mostActiveBranchName} helper="Mayor actividad operativa en el tablero" />
      </div>

      <div className="grid gap-4 xl:grid-cols-[1.1fr_0.9fr]">
        <Card>
          <div className="flex items-start justify-between gap-4">
            <div>
              <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Actividad por sucursal</p>
              <h3 className="mt-3 font-heading text-2xl text-white">Pulso comercial y operativo</h3>
              <p className="mt-2 max-w-2xl text-sm text-slate-400">El score suma clientes activos, clases de hoy y check-ins aceptados para comparar el movimiento de cada sede.</p>
            </div>
            <span className="inline-flex rounded-2xl border border-white/10 bg-white/[0.04] p-3 text-[var(--accent)]">
              <TrendingUp className="h-5 w-5" />
            </span>
          </div>

          <div className="mt-6 space-y-4">
            {summary.branchActivity.map((item) => (
              <div key={item.branchId} className="rounded-[26px] border border-white/10 bg-white/[0.03] p-4">
                <div className="flex items-center justify-between gap-4">
                  <div>
                    <p className="font-semibold text-white">{item.branchName}</p>
                    <p className="mt-1 text-xs uppercase tracking-[0.24em] text-slate-500">
                      {item.activeCustomersCount} clientes | {item.todayClassesCount} clases | {item.todayCheckInsCount} check-ins
                    </p>
                  </div>
                  <p className="font-heading text-2xl text-white">{item.activityCount}</p>
                </div>
                <div className="mt-4 h-2 rounded-full bg-white/5">
                  <div
                    className="h-2 rounded-full bg-[linear-gradient(90deg,var(--accent),#ffd2b5)]"
                    style={{ width: `${Math.max((item.activityCount / maxBranchActivity) * 100, 8)}%` }}
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
              <p className="mt-2 text-sm text-slate-400">Ideal para recepcion y coordinacion de entrenadores durante la demo operativa.</p>
            </div>
            <span className="inline-flex rounded-2xl border border-white/10 bg-white/[0.04] p-3 text-[var(--accent)]">
              <BarChart3 className="h-5 w-5" />
            </span>
          </div>

          <div className="mt-6 space-y-4">
            {summary.classOccupancy.length ? (
              summary.classOccupancy.map((item) => (
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
            <MiniMetric label="Clases hoy" value={summary.todayClassesCount.toString()} />
            <MiniMetric label="Check-ins hoy" value={summary.todayCheckInsCount.toString()} />
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


