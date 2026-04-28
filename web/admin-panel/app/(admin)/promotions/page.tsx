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
import { branchesApi, promotionsApi } from "@/lib/api/admin";
import type { Promotion } from "@/lib/types";
import { promotionDiscountTypeMap, promotionStatusMap } from "@/lib/types";
import { dateTimeLocalToIso, formatDateTime, getErrorMessage, toDateTimeLocalInput } from "@/lib/utils";

type PromotionForm = { branchId: string; title: string; description: string; imageUrl: string; discountType: string; discountValue: string; startsAt: string; endsAt: string; status: string };
const initialForm: PromotionForm = { branchId: "", title: "", description: "", imageUrl: "", discountType: "3", discountValue: "", startsAt: "", endsAt: "", status: "1" };

export default function PromotionsPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<Promotion | null>(null);
  const [form, setForm] = useState<PromotionForm>(initialForm);
  const [error, setError] = useState<string | null>(null);
  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const promotionsQuery = useQuery({ queryKey: ["promotions"], queryFn: promotionsApi.list });
  const branchMap = useMemo(() => Object.fromEntries((branchesQuery.data ?? []).map((branch) => [branch.id, branch.name])), [branchesQuery.data]);

  const saveMutation = useMutation({ mutationFn: async () => { const payload = { branchId: form.branchId || null, title: form.title, description: form.description, imageUrl: form.imageUrl || null, discountType: Number(form.discountType), discountValue: form.discountValue ? Number(form.discountValue) : null, startsAt: dateTimeLocalToIso(form.startsAt), endsAt: dateTimeLocalToIso(form.endsAt), status: Number(form.status) }; return editing ? promotionsApi.update(editing.id, payload) : promotionsApi.create(payload); }, onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ["promotions"] }); setEditing(null); setForm(initialForm); setError(null); }, onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const deleteMutation = useMutation({ mutationFn: promotionsApi.remove, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["promotions"] }), onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const activateMutation = useMutation({ mutationFn: promotionsApi.activate, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["promotions"] }) });
  const disableMutation = useMutation({ mutationFn: promotionsApi.disable, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["promotions"] }) });

  function handleEdit(promotion: Promotion) {
    setEditing(promotion);
    setForm({ branchId: promotion.branchId ?? "", title: promotion.title, description: promotion.description, imageUrl: promotion.imageUrl ?? "", discountType: String(promotion.discountType), discountValue: promotion.discountValue?.toString() ?? "", startsAt: toDateTimeLocalInput(promotion.startsAt), endsAt: toDateTimeLocalInput(promotion.endsAt), status: String(promotion.status) });
  }

  if (branchesQuery.isLoading || promotionsQuery.isLoading) return <LoadingState label="Cargando promociones..." />;
  if (branchesQuery.error || promotionsQuery.error) return <Alert>{getErrorMessage(branchesQuery.error ?? promotionsQuery.error)}</Alert>;

  const promotions = promotionsQuery.data ?? [];

  return (
    <div className="grid gap-6 xl:grid-cols-[0.92fr_1.08fr]">
      <Card>
        <SectionHeading eyebrow="Growth" title={editing ? "Editar promocion" : "Nueva promocion"} description="Publica campañas globales o por sucursal para web, app y admin." />
        <form className="mt-6 space-y-4" onSubmit={(event) => { event.preventDefault(); saveMutation.mutate(); }}>
          <div><Label>Sucursal</Label><Select value={form.branchId} onChange={(event) => setForm((state) => ({ ...state, branchId: event.target.value }))}><option value="">Global</option>{branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}</Select></div>
          <div><Label>Titulo</Label><Input value={form.title} onChange={(event) => setForm((state) => ({ ...state, title: event.target.value }))} /></div>
          <div><Label>Descripcion</Label><Textarea value={form.description} onChange={(event) => setForm((state) => ({ ...state, description: event.target.value }))} /></div>
          <div className="grid gap-4 md:grid-cols-3"><div><Label>Imagen URL</Label><Input value={form.imageUrl} onChange={(event) => setForm((state) => ({ ...state, imageUrl: event.target.value }))} /></div><div><Label>Tipo descuento</Label><Select value={form.discountType} onChange={(event) => setForm((state) => ({ ...state, discountType: event.target.value }))}><option value="1">Porcentaje</option><option value="2">Monto fijo</option><option value="3">Informativa</option></Select></div><div><Label>Valor</Label><Input type="number" step="0.01" value={form.discountValue} onChange={(event) => setForm((state) => ({ ...state, discountValue: event.target.value }))} /></div></div>
          <div className="grid gap-4 md:grid-cols-3"><div><Label>Inicio</Label><Input type="datetime-local" value={form.startsAt} onChange={(event) => setForm((state) => ({ ...state, startsAt: event.target.value }))} /></div><div><Label>Fin</Label><Input type="datetime-local" value={form.endsAt} onChange={(event) => setForm((state) => ({ ...state, endsAt: event.target.value }))} /></div><div><Label>Estado</Label><Select value={form.status} onChange={(event) => setForm((state) => ({ ...state, status: event.target.value }))}><option value="1">Borrador</option><option value="2">Activa</option><option value="3">Expirada</option><option value="4">Deshabilitada</option></Select></div></div>
          {error ? <Alert>{error}</Alert> : null}
          <div className="flex gap-3"><Button type="submit" disabled={saveMutation.isPending}>{editing ? "Guardar cambios" : "Crear promocion"}</Button>{editing ? <Button variant="ghost" onClick={() => { setEditing(null); setForm(initialForm); }}>Cancelar</Button> : null}</div>
        </form>
      </Card>
      <div className="space-y-4">
        <SectionHeading eyebrow="Campaigns" title="Promociones publicadas" description="Activa o deshabilita promociones en caliente desde el panel." />
        {!promotions.length ? <EmptyState title="Sin promociones" description="Todavia no hay campañas creadas para mostrar." /> : null}
        {promotions.length ? <DataTable headers={["Promocion", "Cobertura", "Vigencia", "Estado", "Acciones"]}>{promotions.map((promotion) => <DataRow key={promotion.id}><DataCell><div className="font-semibold text-white">{promotion.title}</div><div className="text-xs text-slate-500">{promotionDiscountTypeMap[promotion.discountType]} {promotion.discountValue ? `· ${promotion.discountValue}` : ""}</div></DataCell><DataCell>{promotion.branchId ? branchMap[promotion.branchId] ?? promotion.branchId : "Global"}</DataCell><DataCell>{formatDateTime(promotion.startsAt)} ? {formatDateTime(promotion.endsAt)}</DataCell><DataCell><Badge tone={promotion.status === 2 ? "success" : promotion.status === 4 ? "warning" : "neutral"}>{promotionStatusMap[promotion.status]}</Badge></DataCell><DataCell className="flex flex-wrap gap-2"><Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(promotion)}>Editar</Button><Button variant="ghost" className="px-3 py-2" onClick={() => activateMutation.mutate(promotion.id)}>Activar</Button><Button variant="ghost" className="px-3 py-2" onClick={() => disableMutation.mutate(promotion.id)}>Deshabilitar</Button><Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(promotion.id)}>Eliminar</Button></DataCell></DataRow>)}</DataTable> : null}
      </div>
    </div>
  );
}

