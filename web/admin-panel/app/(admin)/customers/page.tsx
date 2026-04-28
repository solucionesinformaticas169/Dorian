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
import { branchesApi, customersApi, membershipsApi } from "@/lib/api/admin";
import type { Customer } from "@/lib/types";
import { customerStatusMap, genderMap } from "@/lib/types";
import { dateTimeLocalToIso, formatDate, formatDateTime, getErrorMessage, toDateTimeLocalInput } from "@/lib/utils";

type CustomerForm = {
  email: string;
  password: string;
  branchId: string;
  activeMembershipId: string;
  activeMembershipStartsAtUtc: string;
  activeMembershipEndsAtUtc: string;
  firstName: string;
  lastName: string;
  identificationNumber: string;
  phone: string;
  birthDate: string;
  gender: string;
  emergencyContactName: string;
  emergencyContactPhone: string;
  status: string;
};

const initialForm: CustomerForm = {
  email: "",
  password: "Pass1234!",
  branchId: "",
  activeMembershipId: "",
  activeMembershipStartsAtUtc: "",
  activeMembershipEndsAtUtc: "",
  firstName: "",
  lastName: "",
  identificationNumber: "",
  phone: "",
  birthDate: "",
  gender: "1",
  emergencyContactName: "",
  emergencyContactPhone: "",
  status: "1",
};

export default function CustomersPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<Customer | null>(null);
  const [form, setForm] = useState<CustomerForm>(initialForm);
  const [error, setError] = useState<string | null>(null);

  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const membershipsQuery = useQuery({ queryKey: ["memberships"], queryFn: membershipsApi.list });
  const customersQuery = useQuery({ queryKey: ["customers"], queryFn: customersApi.list });

  const saveMutation = useMutation({
    mutationFn: async () => {
      const payload = {
        ...(editing ? {} : { email: form.email, password: form.password }),
        branchId: form.branchId,
        activeMembershipId: form.activeMembershipId || null,
        activeMembershipStartsAtUtc: form.activeMembershipId ? dateTimeLocalToIso(form.activeMembershipStartsAtUtc) ?? null : null,
        activeMembershipEndsAtUtc: form.activeMembershipId ? dateTimeLocalToIso(form.activeMembershipEndsAtUtc) ?? null : null,
        firstName: form.firstName,
        lastName: form.lastName,
        identificationNumber: form.identificationNumber,
        phone: form.phone || null,
        birthDate: form.birthDate || null,
        gender: Number(form.gender),
        emergencyContactName: form.emergencyContactName || null,
        emergencyContactPhone: form.emergencyContactPhone || null,
        status: Number(form.status),
      };
      return editing ? customersApi.update(editing.id, payload) : customersApi.create(payload);
    },
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["customers"] });
      setEditing(null);
      setForm(initialForm);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const deleteMutation = useMutation({ mutationFn: customersApi.remove, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["customers"] }), onError: (mutationError) => setError(getErrorMessage(mutationError)) });

  const branchMap = useMemo(() => Object.fromEntries((branchesQuery.data ?? []).map((branch) => [branch.id, branch.name])), [branchesQuery.data]);
  const availableMemberships = useMemo(() => (membershipsQuery.data ?? []).filter((membership) => !form.branchId || membership.branchId === form.branchId), [form.branchId, membershipsQuery.data]);
  const customers = customersQuery.data ?? [];

  function handleEdit(customer: Customer) {
    setEditing(customer);
    setForm({
      email: customer.email,
      password: "",
      branchId: customer.branchId,
      activeMembershipId: customer.activeMembershipId ?? "",
      activeMembershipStartsAtUtc: toDateTimeLocalInput(customer.activeMembershipStartsAtUtc),
      activeMembershipEndsAtUtc: toDateTimeLocalInput(customer.activeMembershipEndsAtUtc),
      firstName: customer.firstName,
      lastName: customer.lastName,
      identificationNumber: customer.identificationNumber,
      phone: customer.phone ?? "",
      birthDate: customer.birthDate ?? "",
      gender: String(customer.gender),
      emergencyContactName: customer.emergencyContactName ?? "",
      emergencyContactPhone: customer.emergencyContactPhone ?? "",
      status: String(customer.status),
    });
  }

  if (branchesQuery.isLoading || membershipsQuery.isLoading || customersQuery.isLoading) return <LoadingState label="Cargando clientes..." />;
  if (branchesQuery.error || membershipsQuery.error || customersQuery.error) return <Alert>{getErrorMessage(branchesQuery.error ?? membershipsQuery.error ?? customersQuery.error)}</Alert>;

  return (
    <div className="grid gap-6 xl:grid-cols-[0.92fr_1.08fr]">
      <Card>
        <SectionHeading eyebrow="CRM" title={editing ? "Editar cliente" : "Nuevo cliente"} description="Administra altas de clientes y la vigencia de su membresia activa." />
        <form className="mt-6 space-y-4" onSubmit={(event) => { event.preventDefault(); saveMutation.mutate(); }}>
          {!editing ? <div className="grid gap-4 md:grid-cols-2"><div><Label>Correo</Label><Input type="email" value={form.email} onChange={(event) => setForm((state) => ({ ...state, email: event.target.value }))} /></div><div><Label>Contrasena</Label><Input type="password" value={form.password} onChange={(event) => setForm((state) => ({ ...state, password: event.target.value }))} /></div></div> : null}
          <div className="grid gap-4 md:grid-cols-2"><div><Label>Nombres</Label><Input value={form.firstName} onChange={(event) => setForm((state) => ({ ...state, firstName: event.target.value }))} /></div><div><Label>Apellidos</Label><Input value={form.lastName} onChange={(event) => setForm((state) => ({ ...state, lastName: event.target.value }))} /></div></div>
          <div className="grid gap-4 md:grid-cols-2"><div><Label>Identificacion</Label><Input value={form.identificationNumber} onChange={(event) => setForm((state) => ({ ...state, identificationNumber: event.target.value }))} /></div><div><Label>Telefono</Label><Input value={form.phone} onChange={(event) => setForm((state) => ({ ...state, phone: event.target.value }))} /></div></div>
          <div className="grid gap-4 md:grid-cols-3"><div><Label>Sucursal</Label><Select value={form.branchId} onChange={(event) => setForm((state) => ({ ...state, branchId: event.target.value, activeMembershipId: "" }))}><option value="">Selecciona una sucursal</option>{branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}</Select></div><div><Label>Genero</Label><Select value={form.gender} onChange={(event) => setForm((state) => ({ ...state, gender: event.target.value }))}><option value="1">Masculino</option><option value="2">Femenino</option><option value="3">Otro</option></Select></div><div><Label>Estado</Label><Select value={form.status} onChange={(event) => setForm((state) => ({ ...state, status: event.target.value }))}><option value="1">Activo</option><option value="2">Inactivo</option><option value="3">Suspendido</option></Select></div></div>
          <div className="grid gap-4 md:grid-cols-3"><div><Label>Fecha nacimiento</Label><Input type="date" value={form.birthDate} onChange={(event) => setForm((state) => ({ ...state, birthDate: event.target.value }))} /></div><div><Label>Contacto emergencia</Label><Input value={form.emergencyContactName} onChange={(event) => setForm((state) => ({ ...state, emergencyContactName: event.target.value }))} /></div><div><Label>Telefono emergencia</Label><Input value={form.emergencyContactPhone} onChange={(event) => setForm((state) => ({ ...state, emergencyContactPhone: event.target.value }))} /></div></div>
          <div className="grid gap-4 md:grid-cols-3"><div><Label>Membresia activa</Label><Select value={form.activeMembershipId} onChange={(event) => setForm((state) => ({ ...state, activeMembershipId: event.target.value }))}><option value="">Sin membresia</option>{availableMemberships.map((membership) => <option key={membership.id} value={membership.id}>{membership.name}</option>)}</Select></div><div><Label>Inicio vigencia</Label><Input type="datetime-local" value={form.activeMembershipStartsAtUtc} onChange={(event) => setForm((state) => ({ ...state, activeMembershipStartsAtUtc: event.target.value }))} /></div><div><Label>Fin vigencia</Label><Input type="datetime-local" value={form.activeMembershipEndsAtUtc} onChange={(event) => setForm((state) => ({ ...state, activeMembershipEndsAtUtc: event.target.value }))} /></div></div>
          {error ? <Alert>{error}</Alert> : null}
          <div className="flex gap-3"><Button type="submit" disabled={saveMutation.isPending}>{editing ? "Guardar cambios" : "Crear cliente"}</Button>{editing ? <Button variant="ghost" onClick={() => { setEditing(null); setForm(initialForm); }}>Cancelar</Button> : null}</div>
        </form>
      </Card>
      <div className="space-y-4">
        <SectionHeading eyebrow="Client base" title="Clientes registrados" description="Tabla alimentada por los endpoints reales de customers y memberships." />
        {!customers.length ? <EmptyState title="Sin clientes" description="Aun no se han registrado clientes en la plataforma." /> : null}
        {customers.length ? <DataTable headers={["Cliente", "Sucursal", "Membresia", "Estado", "Nacimiento", "Acciones"]}>{customers.map((customer) => <DataRow key={customer.id}><DataCell><div className="font-semibold text-white">{customer.firstName} {customer.lastName}</div><div className="text-xs text-slate-500">{customer.email} · {genderMap[customer.gender]}</div></DataCell><DataCell>{branchMap[customer.branchId] ?? customer.branchId}</DataCell><DataCell>{customer.activeMembershipId ? `${formatDateTime(customer.activeMembershipStartsAtUtc)} ? ${formatDateTime(customer.activeMembershipEndsAtUtc)}` : "Sin membresia"}</DataCell><DataCell><Badge tone={customer.status === 1 ? "success" : customer.status === 2 ? "warning" : "danger"}>{customerStatusMap[customer.status]}</Badge></DataCell><DataCell>{customer.birthDate ? formatDate(customer.birthDate) : "-"}</DataCell><DataCell className="flex gap-2"><Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(customer)}>Editar</Button><Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(customer.id)}>Eliminar</Button></DataCell></DataRow>)}</DataTable> : null}
      </div>
    </div>
  );
}

