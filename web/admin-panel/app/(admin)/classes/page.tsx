"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { DataCell, DataRow, DataTable } from "@/components/admin/data-table";
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

type ClassForm = { branchId: string; trainerUserId: string; name: string; description: string; startTime: string; endTime: string; capacity: string; status: string };
const initialForm: ClassForm = { branchId: "", trainerUserId: "", name: "", description: "", startTime: "", endTime: "", capacity: "20", status: "1" };

export default function ClassesPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<ClassSession | null>(null);
  const [form, setForm] = useState<ClassForm>(initialForm);
  const [error, setError] = useState<string | null>(null);
  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const classesQuery = useQuery({ queryKey: ["classes"], queryFn: classesApi.list });
  const branchMap = useMemo(() => Object.fromEntries((branchesQuery.data ?? []).map((branch) => [branch.id, branch.name])), [branchesQuery.data]);

  const saveMutation = useMutation({ mutationFn: async () => { const payload = { branchId: form.branchId, trainerUserId: form.trainerUserId || null, name: form.name, description: form.description || null, startTime: dateTimeLocalToIso(form.startTime), endTime: dateTimeLocalToIso(form.endTime), capacity: Number(form.capacity), status: Number(form.status) }; return editing ? classesApi.update(editing.id, payload) : classesApi.create(payload); }, onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ["classes"] }); setEditing(null); setForm(initialForm); setError(null); }, onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const deleteMutation = useMutation({ mutationFn: classesApi.remove, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["classes"] }), onError: (mutationError) => setError(getErrorMessage(mutationError)) });

  function handleEdit(classSession: ClassSession) {
    setEditing(classSession);
    setForm({ branchId: classSession.branchId, trainerUserId: classSession.trainerUserId ?? "", name: classSession.name, description: classSession.description ?? "", startTime: toDateTimeLocalInput(classSession.startTime), endTime: toDateTimeLocalInput(classSession.endTime), capacity: String(classSession.capacity), status: String(classSession.status) });
  }

  if (branchesQuery.isLoading || classesQuery.isLoading) return <LoadingState label="Cargando clases..." />;
  if (branchesQuery.error || classesQuery.error) return <Alert>{getErrorMessage(branchesQuery.error ?? classesQuery.error)}</Alert>;

  const classes = classesQuery.data ?? [];

  return (
    <div className="grid gap-6 xl:grid-cols-[0.92fr_1.08fr]">
      <Card>
        <SectionHeading eyebrow="Schedule" title={editing ? "Editar clase" : "Nueva clase"} description="Programa sesiones por sucursal y asigna opcionalmente el `trainerUserId`." />
        <form className="mt-6 space-y-4" onSubmit={(event) => { event.preventDefault(); saveMutation.mutate(); }}>
          <div className="grid gap-4 md:grid-cols-2"><div><Label>Sucursal</Label><Select value={form.branchId} onChange={(event) => setForm((state) => ({ ...state, branchId: event.target.value }))}><option value="">Selecciona sucursal</option>{branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}</Select></div><div><Label>Trainer User ID</Label><Input value={form.trainerUserId} onChange={(event) => setForm((state) => ({ ...state, trainerUserId: event.target.value }))} placeholder="Opcional" /></div></div>
          <div><Label>Nombre</Label><Input value={form.name} onChange={(event) => setForm((state) => ({ ...state, name: event.target.value }))} /></div>
          <div><Label>Descripcion</Label><Textarea value={form.description} onChange={(event) => setForm((state) => ({ ...state, description: event.target.value }))} /></div>
          <div className="grid gap-4 md:grid-cols-2"><div><Label>Inicio</Label><Input type="datetime-local" value={form.startTime} onChange={(event) => setForm((state) => ({ ...state, startTime: event.target.value }))} /></div><div><Label>Fin</Label><Input type="datetime-local" value={form.endTime} onChange={(event) => setForm((state) => ({ ...state, endTime: event.target.value }))} /></div></div>
          <div className="grid gap-4 md:grid-cols-2"><div><Label>Capacidad</Label><Input type="number" value={form.capacity} onChange={(event) => setForm((state) => ({ ...state, capacity: event.target.value }))} /></div><div><Label>Estado</Label><Select value={form.status} onChange={(event) => setForm((state) => ({ ...state, status: event.target.value }))}><option value="1">Programada</option><option value="2">Cancelada</option><option value="3">Completada</option></Select></div></div>
          {error ? <Alert>{error}</Alert> : null}
          <div className="flex gap-3"><Button type="submit" disabled={saveMutation.isPending}>{editing ? "Guardar cambios" : "Crear clase"}</Button>{editing ? <Button variant="ghost" onClick={() => { setEditing(null); setForm(initialForm); }}>Cancelar</Button> : null}</div>
        </form>
      </Card>
      <div className="space-y-4">
        <SectionHeading eyebrow="Agenda live" title="Clases configuradas" description="Visualiza aforo, horario y estado con datos reales del backend." />
{!classes.length ? <EmptyState title="Sin clases" description="Todavía no existen clases configuradas en el sistema." /> : null}
        {classes.length ? <DataTable headers={["Clase", "Sucursal", "Horario", "Aforo", "Estado", "Acciones"]}>{classes.map((item) => <DataRow key={item.id}><DataCell><div className="font-semibold text-white">{item.name}</div><div className="text-xs text-slate-500">Trainer: {item.trainerUserId || "Sin asignar"}</div></DataCell><DataCell>{branchMap[item.branchId] ?? item.branchId}</DataCell><DataCell>{formatDateTime(item.startTime)} ? {formatDateTime(item.endTime)}</DataCell><DataCell>{item.reservedSpots}/{item.capacity}</DataCell><DataCell><Badge tone={item.status === 1 ? "success" : item.status === 2 ? "warning" : "neutral"}>{classStatusMap[item.status]}</Badge></DataCell><DataCell className="flex gap-2"><Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(item)}>Editar</Button><Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(item.id)}>Eliminar</Button></DataCell></DataRow>)}</DataTable> : null}
      </div>
    </div>
  );
}

