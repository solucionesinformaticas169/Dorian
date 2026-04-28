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
import { branchesApi, membershipsApi } from "@/lib/api/admin";
import type { Membership } from "@/lib/types";
import { formatCurrency, formatDateTime, getErrorMessage } from "@/lib/utils";

type MembershipForm = { branchId: string; name: string; durationInDays: string; price: string; currency: string; isActive: boolean };
const initialForm: MembershipForm = { branchId: "", name: "", durationInDays: "30", price: "35", currency: "USD", isActive: true };

export default function MembershipsPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<Membership | null>(null);
  const [form, setForm] = useState<MembershipForm>(initialForm);
  const [error, setError] = useState<string | null>(null);

  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const membershipsQuery = useQuery({ queryKey: ["memberships"], queryFn: membershipsApi.list });

  const saveMutation = useMutation({
    mutationFn: async () => {
      const payload = { branchId: form.branchId, name: form.name, durationInDays: Number(form.durationInDays), price: Number(form.price), currency: form.currency, isActive: form.isActive };
      return editing ? membershipsApi.update(editing.id, payload) : membershipsApi.create(payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["memberships"] });
      setEditing(null);
      setForm(initialForm);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const deleteMutation = useMutation({ mutationFn: membershipsApi.remove, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["memberships"] }), onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const branchMap = useMemo(() => Object.fromEntries((branchesQuery.data ?? []).map((branch) => [branch.id, branch.name])), [branchesQuery.data]);
  const memberships = membershipsQuery.data ?? [];

  function handleEdit(membership: Membership) {
    setEditing(membership);
    setForm({ branchId: membership.branchId, name: membership.name, durationInDays: String(membership.durationInDays), price: String(membership.price), currency: membership.currency, isActive: membership.isActive });
  }

  if (branchesQuery.isLoading || membershipsQuery.isLoading) return <LoadingState label="Cargando membresias..." />;
  if (branchesQuery.error || membershipsQuery.error) return <Alert>{getErrorMessage(branchesQuery.error ?? membershipsQuery.error)}</Alert>;

  return (
    <div className="grid gap-6 xl:grid-cols-[0.9fr_1.1fr]">
      <Card>
        <SectionHeading eyebrow="Revenue engine" title={editing ? "Editar membresia" : "Nueva membresia"} description="Crea planes por sucursal para el MVP operativo del gimnasio." />
        <form className="mt-6 space-y-4" onSubmit={(event) => { event.preventDefault(); saveMutation.mutate(); }}>
          <div><Label>Sucursal</Label><Select value={form.branchId} onChange={(event) => setForm((state) => ({ ...state, branchId: event.target.value }))}><option value="">Selecciona una sucursal</option>{branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}</Select></div>
          <div><Label>Nombre</Label><Input value={form.name} onChange={(event) => setForm((state) => ({ ...state, name: event.target.value }))} /></div>
          <div className="grid gap-4 md:grid-cols-3">
            <div><Label>Duracion (dias)</Label><Input type="number" value={form.durationInDays} onChange={(event) => setForm((state) => ({ ...state, durationInDays: event.target.value }))} /></div>
            <div><Label>Precio</Label><Input type="number" step="0.01" value={form.price} onChange={(event) => setForm((state) => ({ ...state, price: event.target.value }))} /></div>
            <div><Label>Moneda</Label><Input value={form.currency} onChange={(event) => setForm((state) => ({ ...state, currency: event.target.value.toUpperCase() }))} /></div>
          </div>
          <div><Label>Estado</Label><Select value={String(form.isActive)} onChange={(event) => setForm((state) => ({ ...state, isActive: event.target.value === "true" }))}><option value="true">Activa</option><option value="false">Inactiva</option></Select></div>
          {error ? <Alert>{error}</Alert> : null}
          <div className="flex gap-3"><Button type="submit" disabled={saveMutation.isPending}>{editing ? "Guardar cambios" : "Crear membresia"}</Button>{editing ? <Button variant="ghost" onClick={() => { setEditing(null); setForm(initialForm); }}>Cancelar</Button> : null}</div>
        </form>
      </Card>
      <div className="space-y-4">
        <SectionHeading eyebrow="Catalog" title="Planes vigentes" description="Tabla conectada al backend para venta, activacion y organizacion por sede." />
        {!memberships.length ? <EmptyState title="Sin membresias" description="Todavia no hay planes creados para las sucursales." /> : null}
        {memberships.length ? <DataTable headers={["Plan", "Sucursal", "Precio", "Estado", "Actualizado", "Acciones"]}>{memberships.map((membership) => <DataRow key={membership.id}><DataCell><div className="font-semibold text-white">{membership.name}</div><div className="text-xs text-slate-500">{membership.durationInDays} dias</div></DataCell><DataCell>{branchMap[membership.branchId] ?? membership.branchId}</DataCell><DataCell>{formatCurrency(membership.price, membership.currency)}</DataCell><DataCell><Badge tone={membership.isActive ? "success" : "warning"}>{membership.isActive ? "Activa" : "Inactiva"}</Badge></DataCell><DataCell>{formatDateTime(membership.updatedAtUtc ?? membership.createdAtUtc)}</DataCell><DataCell className="flex gap-2"><Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(membership)}>Editar</Button><Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(membership.id)}>Eliminar</Button></DataCell></DataRow>)}</DataTable> : null}
      </div>
    </div>
  );
}

