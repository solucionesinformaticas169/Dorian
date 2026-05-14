"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { DataCell, DataRow, DataTable } from "@/components/admin/data-table";
import { EmptyState, LoadingState } from "@/components/admin/page-state";
import { SectionHeading } from "@/components/admin/section-heading";
import { authApi, branchesApi, staffApi } from "@/lib/api/admin";
import type { Branch, Session, StaffMember } from "@/lib/types";
import { formatDateTime, getErrorMessage } from "@/lib/utils";

type StaffForm = {
  email: string;
  password: string;
  fullName: string;
  phoneNumber: string;
  branchId: string;
  role: string;
  isActive: boolean;
};

const initialForm: StaffForm = {
  email: "",
  password: "",
  fullName: "",
  phoneNumber: "",
  branchId: "",
  role: "Reception",
  isActive: true,
};

const roleLabels: Record<string, string> = {
  SuperAdmin: "SuperAdmin",
  BranchAdmin: "Administrador de sucursal",
  Reception: "Recepción",
  Trainer: "Trainer",
};

function getRoleLabel(role: string) {
  return roleLabels[role] ?? role;
}

function roleTone(role: string) {
  if (role === "SuperAdmin") return "danger" as const;
  if (role === "BranchAdmin") return "warning" as const;
  if (role === "Reception") return "neutral" as const;
  return "success" as const;
}

function getAllowedRoles(session: Session | undefined) {
  if (!session) return [];
  if (session.user.roles.includes("SuperAdmin")) return ["BranchAdmin", "Reception", "Trainer"];
  if (session.user.roles.includes("BranchAdmin")) return ["Reception", "Trainer"];
  return [];
}

function canManageMember(session: Session | undefined, member: StaffMember) {
  if (!session) return false;
  if (session.user.roles.includes("SuperAdmin")) {
    return member.primaryRole !== "SuperAdmin";
  }

  if (session.user.roles.includes("BranchAdmin")) {
    return member.branchId === session.user.branchId && (member.primaryRole === "Reception" || member.primaryRole === "Trainer");
  }

  return false;
}

function buildPayload(form: StaffForm, includePassword: boolean) {
  return {
    email: form.email,
    password: includePassword ? form.password : undefined,
    fullName: form.fullName,
    phoneNumber: form.phoneNumber || null,
    branchId: form.branchId || null,
    role: form.role,
    isActive: form.isActive,
  };
}

export default function StaffPage() {
  const queryClient = useQueryClient();
  const [editing, setEditing] = useState<StaffMember | null>(null);
  const [form, setForm] = useState<StaffForm>(initialForm);
  const [error, setError] = useState<string | null>(null);

  const sessionQuery = useQuery({ queryKey: ["admin-session"], queryFn: authApi.session });
  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const staffQuery = useQuery({ queryKey: ["staff"], queryFn: staffApi.list });

  const createMutation = useMutation({
    mutationFn: staffApi.create,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["staff"] });
      setEditing(null);
      setForm(initialForm);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const updateMutation = useMutation({
    mutationFn: ({ id, payload }: { id: string; payload: unknown }) => staffApi.update(id, payload),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["staff"] });
      setEditing(null);
      setForm(initialForm);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const deleteMutation = useMutation({
    mutationFn: staffApi.remove,
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["staff"] });
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  const session = sessionQuery.data;
  const allowedRoles = getAllowedRoles(session);
  const branches = branchesQuery.data ?? [];
  const visibleBranches = useMemo(() => {
    if (!session) return [];
    if (session.user.roles.includes("SuperAdmin")) return branches;
    if (session.user.roles.includes("BranchAdmin")) return branches.filter((branch) => branch.id === session.user.branchId);
    return [];
  }, [branches, session]);

  const sortedStaff = useMemo(() => [...(staffQuery.data ?? [])].sort((a, b) => a.fullName.localeCompare(b.fullName)), [staffQuery.data]);
  const summary = useMemo(() => {
    const items = sortedStaff;
    return {
      total: items.length,
      branchAdmins: items.filter((item) => item.primaryRole === "BranchAdmin").length,
      reception: items.filter((item) => item.primaryRole === "Reception").length,
      trainers: items.filter((item) => item.primaryRole === "Trainer").length,
    };
  }, [sortedStaff]);

  const isLoading = sessionQuery.isLoading || branchesQuery.isLoading || staffQuery.isLoading;
  if (isLoading) return <LoadingState label="Cargando estructura de personal..." />;
  if (sessionQuery.error) return <Alert>{getErrorMessage(sessionQuery.error)}</Alert>;
  if (branchesQuery.error) return <Alert>{getErrorMessage(branchesQuery.error)}</Alert>;
  if (staffQuery.error) return <Alert>{getErrorMessage(staffQuery.error)}</Alert>;

  if (!session || allowedRoles.length === 0) {
    return (
      <Card>
        <SectionHeading eyebrow="Jerarquía Dorian" title="Personal operativo" description="Este módulo está disponible para SuperAdmin y BranchAdmin." />
        <Alert className="mt-6">Tu rol actual no tiene acceso a la gestión de personal.</Alert>
      </Card>
    );
  }

  function handleEdit(member: StaffMember) {
    setEditing(member);
    setForm({
      email: member.email,
      password: "",
      fullName: member.fullName,
      phoneNumber: member.phoneNumber ?? "",
      branchId: member.branchId ?? "",
      role: member.primaryRole,
      isActive: member.isActive,
    });
    setError(null);
  }

  function resetForm() {
    setEditing(null);
    setForm({
      ...initialForm,
      role: allowedRoles[0] ?? "Reception",
      branchId: session?.user.roles.includes("BranchAdmin") ? session.user.branchId ?? "" : "",
    });
    setError(null);
  }

  function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    if (editing) {
      updateMutation.mutate({
        id: editing.id,
        payload: {
          fullName: form.fullName,
          phoneNumber: form.phoneNumber || null,
          branchId: form.branchId || null,
          role: form.role,
          isActive: form.isActive,
          password: form.password || null,
        },
      });
      return;
    }

    createMutation.mutate({
      email: form.email,
      password: form.password,
      fullName: form.fullName,
      phoneNumber: form.phoneNumber || null,
      branchId: form.branchId || null,
      role: form.role,
    });
  }

  return (
    <div className="space-y-6">
      <div className="grid gap-4 md:grid-cols-4">
        <Card className="space-y-2">
          <p className="text-xs font-semibold uppercase tracking-[0.3em] text-[var(--accent)]">Personal</p>
          <p className="text-4xl font-semibold text-white">{summary.total}</p>
          <p className="text-sm text-slate-400">Usuarios operativos y administrativos sin contar clientes.</p>
        </Card>
        <Card className="space-y-2">
          <p className="text-xs font-semibold uppercase tracking-[0.3em] text-amber-300">Branch Admin</p>
          <p className="text-4xl font-semibold text-white">{summary.branchAdmins}</p>
          <p className="text-sm text-slate-400">Administradores ligados a una sola sucursal.</p>
        </Card>
        <Card className="space-y-2">
          <p className="text-xs font-semibold uppercase tracking-[0.3em] text-slate-300">Recepción</p>
          <p className="text-4xl font-semibold text-white">{summary.reception}</p>
          <p className="text-sm text-slate-400">Counter y operación diaria de cada sede.</p>
        </Card>
        <Card className="space-y-2">
          <p className="text-xs font-semibold uppercase tracking-[0.3em] text-emerald-300">Trainers</p>
          <p className="text-4xl font-semibold text-white">{summary.trainers}</p>
          <p className="text-sm text-slate-400">Coaches y seguimiento deportivo por sucursal.</p>
        </Card>
      </div>

      <div className="grid gap-6 xl:grid-cols-[0.9fr_1.1fr]">
        <Card>
          <SectionHeading
            eyebrow="Jerarquía Dorian"
            title={editing ? "Editar integrante" : "Nuevo integrante"}
            description={session.user.roles.includes("SuperAdmin") ? "SuperAdmin crea BranchAdmin y también puede intervenir recepción o trainers de cualquier sede." : "BranchAdmin crea y gestiona recepción y trainers de su propia sucursal."}
          />
          <form className="mt-6 space-y-4" onSubmit={handleSubmit}>
            <div className="grid gap-4 md:grid-cols-2">
              <div>
                <Label>Nombre completo</Label>
                <Input value={form.fullName} onChange={(event) => setForm((state) => ({ ...state, fullName: event.target.value }))} />
              </div>
              <div>
                <Label>Teléfono</Label>
                <Input value={form.phoneNumber} onChange={(event) => setForm((state) => ({ ...state, phoneNumber: event.target.value }))} />
              </div>
              <div>
                <Label>Rol</Label>
                <select className="h-11 w-full rounded-2xl border border-white/10 bg-white/5 px-4 text-sm text-white" value={form.role} onChange={(event) => setForm((state) => ({ ...state, role: event.target.value }))}>
                  {allowedRoles.map((role) => (
                    <option key={role} value={role}>{getRoleLabel(role)}</option>
                  ))}
                </select>
              </div>
              <div>
                <Label>Sucursal</Label>
                <select className="h-11 w-full rounded-2xl border border-white/10 bg-white/5 px-4 text-sm text-white" value={form.branchId} onChange={(event) => setForm((state) => ({ ...state, branchId: event.target.value }))}>
                  <option value="">Selecciona sucursal</option>
                  {visibleBranches.map((branch: Branch) => (
                    <option key={branch.id} value={branch.id}>{branch.name}</option>
                  ))}
                </select>
              </div>
              <div>
                <Label>Correo</Label>
                <Input value={form.email} disabled={Boolean(editing)} onChange={(event) => setForm((state) => ({ ...state, email: event.target.value }))} />
              </div>
              <div>
                <Label>{editing ? "Nueva contraseña (opcional)" : "Contraseña"}</Label>
                <Input type="password" value={form.password} onChange={(event) => setForm((state) => ({ ...state, password: event.target.value }))} />
              </div>
            </div>

            {editing ? (
              <div>
                <Label>Estado</Label>
                <select className="h-11 w-full rounded-2xl border border-white/10 bg-white/5 px-4 text-sm text-white" value={String(form.isActive)} onChange={(event) => setForm((state) => ({ ...state, isActive: event.target.value === "true" }))}>
                  <option value="true">Activo</option>
                  <option value="false">Inactivo</option>
                </select>
              </div>
            ) : null}

            {error ? <Alert>{error}</Alert> : null}

            <div className="flex gap-3">
              <Button type="submit" disabled={createMutation.isPending || updateMutation.isPending}>{editing ? "Guardar cambios" : "Crear integrante"}</Button>
              {(editing || form.fullName || form.email) ? <Button type="button" variant="ghost" onClick={resetForm}>Cancelar</Button> : null}
            </div>
          </form>
        </Card>

        <div className="space-y-4">
          <SectionHeading eyebrow="Operación por rol" title="Equipo administrativo y operativo" description="SuperAdmin ve todo el personal. BranchAdmin solo ve y gestiona su propia estructura local." />
          {!sortedStaff.length ? <EmptyState title="Sin personal registrado" description="Crea el primer BranchAdmin, recepcionista o trainer desde este panel." /> : null}
          {sortedStaff.length ? (
            <DataTable headers={["Persona", "Rol", "Sucursal", "Estado", "Actualizado", "Acciones"]}>
              {sortedStaff.map((member) => (
                <DataRow key={member.id}>
                  <DataCell>
                    <div className="font-semibold text-white">{member.fullName}</div>
                    <div className="text-xs text-slate-500">{member.email}</div>
                  </DataCell>
                  <DataCell>
                    <Badge tone={roleTone(member.primaryRole)}>{getRoleLabel(member.primaryRole)}</Badge>
                  </DataCell>
                  <DataCell>{member.branchName ?? "Global"}</DataCell>
                  <DataCell><Badge tone={member.isActive ? "success" : "warning"}>{member.isActive ? "Activo" : "Inactivo"}</Badge></DataCell>
                  <DataCell>{formatDateTime(member.updatedAtUtc ?? member.createdAtUtc)}</DataCell>
                  <DataCell className="flex gap-2">
                    {canManageMember(session, member) ? (
                      <>
                        <Button variant="secondary" className="px-3 py-2" onClick={() => handleEdit(member)}>Editar</Button>
                        <Button variant="danger" className="px-3 py-2" onClick={() => deleteMutation.mutate(member.id)} disabled={deleteMutation.isPending || !member.isActive}>Desactivar</Button>
                      </>
                    ) : (
                      <span className="text-xs text-slate-500">Vista referencial</span>
                    )}
                  </DataCell>
                </DataRow>
              ))}
            </DataTable>
          ) : null}
        </div>
      </div>
    </div>
  );
}
