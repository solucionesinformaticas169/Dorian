"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { EmptyState, LoadingState } from "@/components/admin/page-state";
import { SectionHeading } from "@/components/admin/section-heading";
import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Select } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { branchesApi, classesApi } from "@/lib/api/admin";
import type { ClassSession } from "@/lib/types";
import { classStatusMap } from "@/lib/types";
import { dateTimeLocalToIso, formatDateTime, getErrorMessage, toDateTimeLocalInput } from "@/lib/utils";

type ClassScope = "branch" | "global";

type ClassForm = {
  scope: ClassScope;
  branchId: string;
  trainerUserId: string;
  name: string;
  description: string;
  startTime: string;
  endTime: string;
  capacity: string;
  status: string;
};

const initialForm: ClassForm = {
  scope: "branch",
  branchId: "",
  trainerUserId: "",
  name: "",
  description: "",
  startTime: "",
  endTime: "",
  capacity: "20",
  status: "1",
};

function getOccupancyState(reservedSpots: number, capacity: number) {
  const safeCapacity = Math.max(capacity, 0);
  const safeReserved = Math.max(reservedSpots, 0);
  const occupancyRate = safeCapacity > 0 ? Math.min((safeReserved / safeCapacity) * 100, 100) : 0;

  if (safeCapacity > 0 && safeReserved >= safeCapacity) {
    return { label: "Al tope", tone: "danger" as const, barClassName: "bg-rose-400" };
  }

  if (occupancyRate >= 80) {
    return { label: "Ultimos cupos", tone: "warning" as const, barClassName: "bg-amber-400" };
  }

  return { label: "Disponible", tone: "success" as const, barClassName: "bg-emerald-400" };
}

export default function ClassesPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<ClassSession | null>(null);
  const [form, setForm] = useState<ClassForm>(initialForm);
  const [error, setError] = useState<string | null>(null);
  const [branchFilter, setBranchFilter] = useState("all");

  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const classesQuery = useQuery({ queryKey: ["classes"], queryFn: classesApi.list });

  const branchMap = useMemo(() => Object.fromEntries((branchesQuery.data ?? []).map((branch) => [branch.id, branch.name])), [branchesQuery.data]);

  const saveMutation = useMutation({
    mutationFn: async () => {
      const basePayload = {
        trainerUserId: form.trainerUserId || null,
        name: form.name,
        description: form.description || null,
        startTime: dateTimeLocalToIso(form.startTime),
        endTime: dateTimeLocalToIso(form.endTime),
        capacity: Number(form.capacity),
        status: Number(form.status),
      };

      if (editing) {
        return classesApi.update(editing.id, { ...basePayload, branchId: form.branchId });
      }

      if (form.scope === "global") {
        const branches = branchesQuery.data ?? [];
        if (!branches.length) {
          throw new Error("No hay sucursales disponibles para crear una clase global.");
        }

        await Promise.all(branches.map((branch) => classesApi.create({ ...basePayload, branchId: branch.id })));
        return null;
      }

      return classesApi.create({ ...basePayload, branchId: form.branchId });
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["classes"] });
      setEditing(null);
      setForm(initialForm);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const deleteMutation = useMutation({
    mutationFn: classesApi.remove,
    onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["classes"] }),
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const classes = useMemo(() => {
    const items = classesQuery.data ?? [];
    return items
      .map((item) => {
        const occupancy = getOccupancyState(item.reservedSpots, item.capacity);
        return {
          ...item,
          branchLabel: branchMap[item.branchId] ?? item.branchId ?? "Global",
          availableSpots: Math.max(item.capacity - item.reservedSpots, 0),
          occupancyRate: item.capacity > 0 ? Math.min((item.reservedSpots / item.capacity) * 100, 100) : 0,
          ...occupancy,
        };
      })
      .sort((left, right) => new Date(left.startTime).getTime() - new Date(right.startTime).getTime());
  }, [branchMap, classesQuery.data]);

  const filteredClasses = useMemo(() => {
    if (branchFilter === "all") return classes;
    return classes.filter((item) => item.branchId === branchFilter);
  }, [branchFilter, classes]);

  const summary = useMemo(() => {
    const total = filteredClasses.length;
    const full = filteredClasses.filter((item) => item.availableSpots === 0).length;
    const reserved = filteredClasses.reduce((accumulator, item) => accumulator + item.reservedSpots, 0);
    const capacity = filteredClasses.reduce((accumulator, item) => accumulator + item.capacity, 0);
    return {
      total,
      full,
      reserved,
      capacity,
    };
  }, [filteredClasses]);

  const canSubmit =
    !saveMutation.isPending &&
    !!form.name &&
    !!form.startTime &&
    !!form.endTime &&
    !!form.capacity &&
    (editing ? !!form.branchId : form.scope === "global" || !!form.branchId);
  const hasDirtyForm = JSON.stringify(form) !== JSON.stringify(initialForm);

  function resetForm() {
    setEditing(null);
    setForm(initialForm);
    setError(null);
  }

  function handleEdit(classSession: ClassSession) {
    setEditing(classSession);
    setForm({
      scope: "branch",
      branchId: classSession.branchId,
      trainerUserId: classSession.trainerUserId ?? "",
      name: classSession.name,
      description: classSession.description ?? "",
      startTime: toDateTimeLocalInput(classSession.startTime),
      endTime: toDateTimeLocalInput(classSession.endTime),
      capacity: String(classSession.capacity),
      status: String(classSession.status),
    });
    setError(null);
  }

  if (branchesQuery.isLoading || classesQuery.isLoading) return <LoadingState label="Cargando clases..." />;
  if (branchesQuery.error || classesQuery.error) return <Alert>{getErrorMessage(branchesQuery.error ?? classesQuery.error)}</Alert>;

  return (
    <div className="grid gap-6 xl:grid-cols-[0.88fr_1.12fr]">
      <div className="space-y-4">
        <Card>
          <SectionHeading
            eyebrow="Schedule"
            title={editing ? "Editar clase" : "Crear clase"}
            description="Superadmin puede programar por una sola sucursal o replicar la misma clase en todas las sucursales desde un solo formulario."
          />
          <form className="mt-6 space-y-5" onSubmit={(event) => { event.preventDefault(); saveMutation.mutate(); }}>
            {!editing ? (
              <div className="grid gap-3 md:grid-cols-2">
                <button
                  type="button"
                  className={`rounded-[24px] border px-4 py-4 text-left transition ${form.scope === "branch" ? "border-[var(--accent)] bg-[var(--accent)]/10" : "border-white/10 bg-white/[0.03]"}`}
                  onClick={() => setForm((state) => ({ ...state, scope: "branch", branchId: state.branchId }))}
                >
                  <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Modo</p>
                  <p className="mt-2 font-semibold text-white">Por sucursal</p>
                  <p className="mt-1 text-sm text-slate-400">Creas una clase para una sede especifica.</p>
                </button>
                <button
                  type="button"
                  className={`rounded-[24px] border px-4 py-4 text-left transition ${form.scope === "global" ? "border-[var(--accent)] bg-[var(--accent)]/10" : "border-white/10 bg-white/[0.03]"}`}
                  onClick={() => setForm((state) => ({ ...state, scope: "global", branchId: "" }))}
                >
                  <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Modo</p>
                  <p className="mt-2 font-semibold text-white">Global</p>
                  <p className="mt-1 text-sm text-slate-400">Replica la misma clase en todas las sucursales activas.</p>
                </button>
              </div>
            ) : null}

            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <Label>{editing ? "Sucursal asignada" : form.scope === "global" ? "Cobertura" : "Sucursal"}</Label>
                {editing ? (
                  <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-3 text-sm text-white">
                    {branchMap[form.branchId] ?? form.branchId}
                  </div>
                ) : form.scope === "global" ? (
                  <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-3 text-sm text-slate-300">
                    Se creara una clase igual en cada sucursal disponible.
                  </div>
                ) : (
                  <Select value={form.branchId} onChange={(event) => setForm((state) => ({ ...state, branchId: event.target.value }))}>
                    <option value="">Selecciona sucursal</option>
                    {branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}
                  </Select>
                )}
              </div>
              <div>
                <Label>Trainer User ID</Label>
                <Input value={form.trainerUserId} onChange={(event) => setForm((state) => ({ ...state, trainerUserId: event.target.value }))} placeholder="Opcional" />
              </div>
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <Label>Nombre</Label>
                <Input value={form.name} onChange={(event) => setForm((state) => ({ ...state, name: event.target.value }))} placeholder="Ej. Boxfit Express" />
              </div>
              <div>
                <Label>Capacidad</Label>
                <Input type="number" value={form.capacity} onChange={(event) => setForm((state) => ({ ...state, capacity: event.target.value }))} />
              </div>
            </div>

            <div>
              <Label>Descripcion</Label>
              <Textarea value={form.description} onChange={(event) => setForm((state) => ({ ...state, description: event.target.value }))} />
            </div>

            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <Label>Inicio</Label>
                <Input type="datetime-local" value={form.startTime} onChange={(event) => setForm((state) => ({ ...state, startTime: event.target.value }))} />
              </div>
              <div>
                <Label>Fin</Label>
                <Input type="datetime-local" value={form.endTime} onChange={(event) => setForm((state) => ({ ...state, endTime: event.target.value }))} />
              </div>
            </div>

            <div>
              <Label>Estado</Label>
              <Select value={form.status} onChange={(event) => setForm((state) => ({ ...state, status: event.target.value }))}>
                <option value="1">Programada</option>
                <option value="2">Cancelada</option>
                <option value="3">Completada</option>
              </Select>
            </div>

            {form.scope === "global" && !editing ? (
              <div className="rounded-[22px] border border-amber-400/20 bg-amber-400/10 px-4 py-3 text-sm text-amber-100">
                La opcion global crea una copia de la clase en cada sucursal para que aparezca en todas sin cargar mas la operacion manual.
              </div>
            ) : null}

            {error ? <Alert>{error}</Alert> : null}

            <div className="flex flex-wrap gap-3">
              <Button type="submit" disabled={!canSubmit}>
                {editing ? "Guardar cambios" : form.scope === "global" ? "Crear en todas las sucursales" : "Crear clase"}
              </Button>
              {(editing || hasDirtyForm) ? <Button variant="ghost" onClick={resetForm}>Limpiar</Button> : null}
            </div>
          </form>
        </Card>
      </div>

      <div className="space-y-4">
        <Card>
          <SectionHeading eyebrow="Overview" title="Agenda de clases" description="Resumen mas claro del aforo y estado de cada clase, con filtro por sucursal para evitar saturacion visual." />
          <div className="mt-6 grid gap-3 md:grid-cols-4">
            <div className="rounded-[22px] border border-white/10 bg-black/20 px-4 py-4">
              <p className="text-xs uppercase tracking-[0.2em] text-slate-500">Clases</p>
              <p className="mt-2 font-heading text-3xl text-white">{summary.total}</p>
            </div>
            <div className="rounded-[22px] border border-white/10 bg-black/20 px-4 py-4">
              <p className="text-xs uppercase tracking-[0.2em] text-slate-500">Reservas</p>
              <p className="mt-2 font-heading text-3xl text-white">{summary.reserved}</p>
            </div>
            <div className="rounded-[22px] border border-white/10 bg-black/20 px-4 py-4">
              <p className="text-xs uppercase tracking-[0.2em] text-slate-500">Cupos</p>
              <p className="mt-2 font-heading text-3xl text-white">{summary.capacity}</p>
            </div>
            <div className="rounded-[22px] border border-white/10 bg-black/20 px-4 py-4">
              <p className="text-xs uppercase tracking-[0.2em] text-slate-500">Al tope</p>
              <p className="mt-2 font-heading text-3xl text-white">{summary.full}</p>
            </div>
          </div>

          <div className="mt-5 grid gap-4 md:grid-cols-[1fr_auto] md:items-end">
            <div>
              <Label>Filtrar por sucursal</Label>
              <Select value={branchFilter} onChange={(event) => setBranchFilter(event.target.value)}>
                <option value="all">Todas las sucursales</option>
                {branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}
              </Select>
            </div>
            <Badge tone="neutral" className="justify-center px-4 py-3 md:self-center">
              {filteredClasses.length} clases visibles
            </Badge>
          </div>
        </Card>

        {!filteredClasses.length ? <EmptyState title="Sin clases" description="Todavia no existen clases configuradas con el filtro actual." /> : null}

        {filteredClasses.map((item) => (
          <Card key={item.id} className="p-5">
            <div className="flex flex-wrap items-start justify-between gap-4">
              <div>
                <div className="flex flex-wrap items-center gap-2">
                  <h3 className="font-heading text-2xl text-white">{item.name}</h3>
                  <Badge tone={item.tone}>{item.label}</Badge>
                  <Badge tone={item.status === 1 ? "success" : item.status === 2 ? "warning" : "neutral"}>{classStatusMap[item.status]}</Badge>
                </div>
                <p className="mt-2 text-sm text-slate-400">{item.branchLabel}</p>
                <p className="mt-1 text-xs uppercase tracking-[0.2em] text-slate-500">
                  {formatDateTime(item.startTime)} - {formatDateTime(item.endTime)}
                </p>
              </div>
              <div className="flex flex-wrap gap-2">
                <Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(item)}>Editar</Button>
                <Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(item.id)}>Eliminar</Button>
              </div>
            </div>

            <div className="mt-4 grid gap-3 md:grid-cols-4">
              <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-3">
                <p className="text-xs uppercase tracking-[0.18em] text-slate-500">Reservadas</p>
                <p className="mt-2 text-lg font-semibold text-white">{item.reservedSpots}</p>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-3">
                <p className="text-xs uppercase tracking-[0.18em] text-slate-500">Aperturados</p>
                <p className="mt-2 text-lg font-semibold text-white">{item.capacity}</p>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-3">
                <p className="text-xs uppercase tracking-[0.18em] text-slate-500">Disponibles</p>
                <p className="mt-2 text-lg font-semibold text-white">{item.availableSpots}</p>
              </div>
              <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-3">
                <p className="text-xs uppercase tracking-[0.18em] text-slate-500">Trainer</p>
                <p className="mt-2 truncate text-lg font-semibold text-white">{item.trainerUserId || "Sin asignar"}</p>
              </div>
            </div>

            <div className="mt-4">
              <div className="flex items-center justify-between gap-3 text-sm text-slate-300">
                <span>Ocupacion</span>
                <span>{item.occupancyRate.toFixed(0)}%</span>
              </div>
              <div className="mt-2 h-2 overflow-hidden rounded-full bg-white/10">
                <div className={`h-full rounded-full ${item.barClassName}`} style={{ width: `${item.occupancyRate}%` }} />
              </div>
            </div>

            {item.description ? <p className="mt-4 text-sm leading-6 text-slate-400">{item.description}</p> : null}
          </Card>
        ))}
      </div>
    </div>
  );
}
