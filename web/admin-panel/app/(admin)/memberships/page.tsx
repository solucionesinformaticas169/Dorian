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
import { branchesApi, plansApi } from "@/lib/api/admin";
import type { Plan } from "@/lib/types";
import { formatCurrency, formatDateTime, getErrorMessage } from "@/lib/utils";
import { authApi } from "@/lib/api/admin";

type PlanForm = { branchId: string; name: string; durationInDays: string; price: string; currency: string; isActive: boolean };
const initialForm: PlanForm = { branchId: "", name: "", durationInDays: "30", price: "35", currency: "USD", isActive: true };

export default function PlansPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<Plan | null>(null);
  const [form, setForm] = useState<PlanForm>(initialForm);
  const [error, setError] = useState<string | null>(null);

  const sessionQuery = useQuery({ queryKey: ["admin-session"], queryFn: authApi.session });
  const isTrainer = sessionQuery.data?.user.roles.includes("Trainer") ?? false;
  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list, enabled: !isTrainer });
  const plansQuery = useQuery({ queryKey: ["plans"], queryFn: plansApi.list });

  const saveMutation = useMutation({
    mutationFn: async () => {
      const payload = {
        branchId: form.branchId || null,
        name: form.name,
        durationInDays: Number(form.durationInDays),
        price: Number(form.price),
        currency: form.currency,
        isActive: form.isActive,
      };
      return editing ? plansApi.update(editing.id, payload) : plansApi.create(payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["plans"] });
      setEditing(null);
      setForm(initialForm);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const deleteMutation = useMutation({ mutationFn: plansApi.remove, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["plans"] }), onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const branchMap = useMemo(() => Object.fromEntries((branchesQuery.data ?? []).map((branch) => [branch.id, branch.name])), [branchesQuery.data]);
  const plans = plansQuery.data ?? [];

  function handleEdit(plan: Plan) {
    setEditing(plan);
    setForm({
      branchId: plan.branchId ?? "",
      name: plan.name,
      durationInDays: String(plan.durationInDays),
      price: String(plan.price),
      currency: plan.currency,
      isActive: plan.isActive,
    });
  }

if (sessionQuery.isLoading || branchesQuery.isLoading || plansQuery.isLoading) return <LoadingState label="Cargando planes..." />;
  if (branchesQuery.error || plansQuery.error) return <Alert>{getErrorMessage(branchesQuery.error ?? plansQuery.error)}</Alert>;

  return (
    <div className="grid gap-6 xl:grid-cols-[0.9fr_1.1fr]">
      {!isTrainer ? (
        <Card>
          <SectionHeading eyebrow="Revenue engine" title={editing ? "Editar plan" : "Nuevo plan"} description="Define el catálogo de planes de pago que luego el cliente podrá contratar desde la app." />
          <form className="mt-6 space-y-4" onSubmit={(event) => { event.preventDefault(); saveMutation.mutate(); }}>
            <div><Label>Sucursal</Label><Select value={form.branchId} onChange={(event) => setForm((state) => ({ ...state, branchId: event.target.value }))}><option value="">Global / todas las sucursales</option>{branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}</Select></div>
            <div><Label>Nombre</Label><Input value={form.name} onChange={(event) => setForm((state) => ({ ...state, name: event.target.value }))} /></div>
            <div className="grid gap-4 md:grid-cols-3">
            <div><Label>Duración (días)</Label><Input type="number" value={form.durationInDays} onChange={(event) => setForm((state) => ({ ...state, durationInDays: event.target.value }))} /></div>
              <div><Label>Precio</Label><Input type="number" step="0.01" value={form.price} onChange={(event) => setForm((state) => ({ ...state, price: event.target.value }))} /></div>
              <div><Label>Moneda</Label><Input value={form.currency} onChange={(event) => setForm((state) => ({ ...state, currency: event.target.value.toUpperCase() }))} /></div>
            </div>
            <div><Label>Estado</Label><Select value={String(form.isActive)} onChange={(event) => setForm((state) => ({ ...state, isActive: event.target.value === "true" }))}><option value="true">Activa</option><option value="false">Inactiva</option></Select></div>
            {error ? <Alert>{error}</Alert> : null}
            <div className="flex gap-3"><Button type="submit" disabled={saveMutation.isPending}>{editing ? "Guardar cambios" : "Crear plan"}</Button>{editing ? <Button variant="ghost" onClick={() => { setEditing(null); setForm(initialForm); }}>Cancelar</Button> : null}</div>
          </form>
        </Card>
      ) : (
        <Card>
          <SectionHeading eyebrow="Trainer" title="Planes visibles para tu sucursal" description="Consulta los planes globales y los planes de tu sucursal para orientar mejor al cliente durante el entrenamiento." />
          <div className="mt-6 rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-4 text-sm text-slate-300">
            Esta vista es informativa. Desde aquí no editas planes; solo consultas el catálogo disponible para tus clientes.
          </div>
        </Card>
      )}
      <div className="space-y-4">
        <SectionHeading eyebrow="Catalog" title="Planes vigentes" description="Catálogo administrativo de planes para cobro, activación desde la app y organización comercial." />
        {!plans.length ? <EmptyState title="Sin planes" description="Todavía no hay planes creados para las sucursales." /> : null}
        {plans.length ? <DataTable headers={!isTrainer ? ["Plan", "Sucursal", "Precio", "Estado", "Actualizado", "Acciones"] : ["Plan", "Sucursal", "Precio", "Estado", "Actualizado"]}>{plans.map((plan) => <DataRow key={plan.id}><DataCell><div className="font-semibold text-white">{plan.name}</div><div className="text-xs text-slate-500">{plan.durationInDays} días</div></DataCell><DataCell>{plan.branchId ? branchMap[plan.branchId] ?? "Tu sucursal" : "Global"}</DataCell><DataCell>{formatCurrency(plan.price, plan.currency)}</DataCell><DataCell><Badge tone={plan.isActive ? "success" : "warning"}>{plan.isActive ? "Activa" : "Inactiva"}</Badge></DataCell><DataCell>{formatDateTime(plan.updatedAtUtc ?? plan.createdAtUtc)}</DataCell>{!isTrainer ? <DataCell className="flex gap-2"><Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(plan)}>Editar</Button><Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(plan.id)}>Eliminar</Button></DataCell> : null}</DataRow>)}</DataTable> : null}
      </div>
    </div>
  );
}


