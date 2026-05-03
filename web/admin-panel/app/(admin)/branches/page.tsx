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
import { branchesApi } from "@/lib/api/admin";
import type { Branch } from "@/lib/types";
import { formatDateTime, getErrorMessage } from "@/lib/utils";

type BranchForm = {
  code: string;
  name: string;
  city: string;
  address: string;
  phoneNumber: string;
  openingHours: string;
  mapUrl: string;
  latitude: string;
  longitude: string;
  isActive: boolean;
};

const initialForm: BranchForm = {
  code: "",
  name: "",
  city: "",
  address: "",
  phoneNumber: "",
  openingHours: "",
  mapUrl: "",
  latitude: "",
  longitude: "",
  isActive: true,
};

export default function BranchesPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<Branch | null>(null);
  const [form, setForm] = useState<BranchForm>(initialForm);
  const [error, setError] = useState<string | null>(null);

  const query = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });

  const createMutation = useMutation({
    mutationFn: branchesApi.create,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["branches"] });
      setForm(initialForm);
      setEditing(null);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: unknown }) => branchesApi.update(id, payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["branches"] });
      setForm(initialForm);
      setEditing(null);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const deleteMutation = useMutation({
    mutationFn: branchesApi.remove,
    onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["branches"] }),
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const isSubmitting = createMutation.isPending || updateMutation.isPending;
  const sortedBranches = useMemo(() => [...(query.data ?? [])].sort((a, b) => a.name.localeCompare(b.name)), [query.data]);

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    const payload = {
      code: form.code,
      name: form.name,
      city: form.city,
      address: form.address || null,
      phoneNumber: form.phoneNumber || null,
      openingHours: form.openingHours || null,
      mapUrl: form.mapUrl || null,
      latitude: form.latitude ? Number(form.latitude) : null,
      longitude: form.longitude ? Number(form.longitude) : null,
      ...(editing ? { isActive: form.isActive } : {}),
    };

    if (editing) {
      updateMutation.mutate({ id: editing.id, payload });
      return;
    }

    createMutation.mutate(payload);
  }

  function handleEdit(branch: Branch) {
    setEditing(branch);
    setForm({
      code: branch.code,
      name: branch.name,
      city: branch.city,
      address: branch.address ?? "",
      phoneNumber: branch.phoneNumber ?? "",
      openingHours: branch.openingHours ?? "",
      mapUrl: branch.mapUrl ?? "",
      latitude: branch.latitude?.toString() ?? "",
      longitude: branch.longitude?.toString() ?? "",
      isActive: branch.isActive,
    });
  }

  if (query.isLoading) return <LoadingState label="Cargando sucursales..." />;
  if (query.error) return <Alert>{getErrorMessage(query.error)}</Alert>;

  return (
    <div className="grid gap-6 xl:grid-cols-[0.95fr_1.05fr]">
      <Card>
        <SectionHeading eyebrow="Red Dorian" title={editing ? "Editar sucursal" : "Nueva sucursal"} description="Gestiona ubicaciones reales, horarios y enlaces de Google Maps desde un solo formulario." />
        <form className="mt-6 space-y-4" onSubmit={handleSubmit}>
          <div className="grid gap-4 md:grid-cols-2">
            <div><Label>Codigo</Label><Input value={form.code} onChange={(event) => setForm((state) => ({ ...state, code: event.target.value }))} /></div>
            <div><Label>Nombre</Label><Input value={form.name} onChange={(event) => setForm((state) => ({ ...state, name: event.target.value }))} /></div>
            <div><Label>Ciudad</Label><Input value={form.city} onChange={(event) => setForm((state) => ({ ...state, city: event.target.value }))} /></div>
            <div><Label>Telefono</Label><Input value={form.phoneNumber} onChange={(event) => setForm((state) => ({ ...state, phoneNumber: event.target.value }))} /></div>
            <div><Label>Horario</Label><Input value={form.openingHours} onChange={(event) => setForm((state) => ({ ...state, openingHours: event.target.value }))} placeholder="Lun a Vie 06:00 - 22:00" /></div>
            <div><Label>Mapa URL</Label><Input value={form.mapUrl} onChange={(event) => setForm((state) => ({ ...state, mapUrl: event.target.value }))} placeholder="https://www.google.com/maps/search/?api=1&query=..." /></div>
            <div><Label>Latitud</Label><Input value={form.latitude} onChange={(event) => setForm((state) => ({ ...state, latitude: event.target.value }))} placeholder="-2.900000" /></div>
            <div><Label>Longitud</Label><Input value={form.longitude} onChange={(event) => setForm((state) => ({ ...state, longitude: event.target.value }))} placeholder="-79.000000" /></div>
          </div>
          <div><Label>Direccion</Label><Input value={form.address} onChange={(event) => setForm((state) => ({ ...state, address: event.target.value }))} /></div>
          {editing ? (
            <div>
              <Label>Estado</Label>
              <select className="h-11 w-full rounded-2xl border border-white/10 bg-white/5 px-4 text-sm text-white" value={String(form.isActive)} onChange={(event) => setForm((state) => ({ ...state, isActive: event.target.value === "true" }))}>
                <option value="true">Activa</option>
                <option value="false">Inactiva</option>
              </select>
            </div>
          ) : null}
          {error ? <Alert>{error}</Alert> : null}
          <div className="flex gap-3">
            <Button type="submit" disabled={isSubmitting}>{editing ? "Guardar cambios" : "Crear sucursal"}</Button>
            {editing ? <Button variant="ghost" onClick={() => { setEditing(null); setForm(initialForm); }}>Cancelar</Button> : null}
          </div>
        </form>
      </Card>

      <div className="space-y-4">
        <SectionHeading eyebrow="Live data" title="Sucursales registradas" description="Vista conectada al endpoint real `/branches` del backend." />
        {!sortedBranches.length ? <EmptyState title="Sin sucursales" description="Crea la primera sucursal desde el panel para comenzar el MVP." /> : null}
        {sortedBranches.length ? (
          <DataTable headers={["Sucursal", "Ciudad", "Mapa", "Estado", "Actualizado", "Acciones"]}>
            {sortedBranches.map((branch) => (
              <DataRow key={branch.id}>
                <DataCell>
                  <div className="font-semibold text-white">{branch.name}</div>
                  <div className="text-xs text-slate-500">{branch.code} · {branch.phoneNumber || "Sin telefono"}</div>
                </DataCell>
                <DataCell>{branch.city}</DataCell>
                <DataCell>
                  {branch.mapUrl ? (
                    <a href={branch.mapUrl} target="_blank" rel="noreferrer" className="text-[var(--accent)] hover:underline">
                      Ver mapa
                    </a>
                  ) : (
                    <span className="text-slate-500">Pendiente</span>
                  )}
                </DataCell>
                <DataCell><Badge tone={branch.isActive ? "success" : "warning"}>{branch.isActive ? "Activa" : "Inactiva"}</Badge></DataCell>
                <DataCell>{formatDateTime(branch.updatedAtUtc ?? branch.createdAtUtc)}</DataCell>
                <DataCell className="flex gap-2">
                  <Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(branch)}>Editar</Button>
                  <Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(branch.id)} disabled={deleteMutation.isPending}>Eliminar</Button>
                </DataCell>
              </DataRow>
            ))}
          </DataTable>
        ) : null}
      </div>
    </div>
  );
}
