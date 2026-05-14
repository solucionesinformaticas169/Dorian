"use client";

import { useMemo, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { authApi, customersApi, plansApi } from "@/lib/api/admin";
import { Button } from "@/components/ui/button";
import { Alert } from "@/components/ui/alert";
import { Card } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { EmptyState, LoadingState } from "@/components/admin/page-state";
import { SectionHeading } from "@/components/admin/section-heading";
import type { Customer, Plan } from "@/lib/types";
import { dateInputToIso, formatDate, getErrorMessage, toDateInput } from "@/lib/utils";

export default function CustomersPage() {
  const queryClient = useQueryClient();
  const [search, setSearch] = useState("");
  const [selectedCustomerId, setSelectedCustomerId] = useState<string | null>(null);
  const [selectedPlanId, setSelectedPlanId] = useState("");
  const [startsAt, setStartsAt] = useState("");
  const [endsAt, setEndsAt] = useState("");
  const [error, setError] = useState<string | null>(null);

  const sessionQuery = useQuery({ queryKey: ["admin-session"], queryFn: authApi.session });
  const summaryQuery = useQuery({
    queryKey: ["customers-summary"],
    queryFn: customersApi.summary,
    enabled: sessionQuery.data?.user.roles.includes("Reception") === false && sessionQuery.data?.user.roles.includes("Trainer") === false,
  });
  const customersQuery = useQuery({
    queryKey: ["customers"],
    queryFn: customersApi.list,
    enabled: sessionQuery.data?.user.roles.some((role) => role === "Reception" || role === "Trainer") === true,
  });
  const plansQuery = useQuery({ queryKey: ["plans"], queryFn: plansApi.list, enabled: sessionQuery.data?.user.roles.includes("Reception") === true });
  const metrics = summaryQuery.data;
  const session = sessionQuery.data;
  const isReception = session?.user.roles.includes("Reception") ?? false;
  const isTrainer = session?.user.roles.includes("Trainer") ?? false;

  const receptionCustomers = customersQuery.data ?? [];
  const receptionPlans = useMemo(() => {
    if (!isReception) return [];
    return (plansQuery.data ?? []).filter((plan) => plan.isActive);
  }, [isReception, plansQuery.data]);

  const filteredCustomers = useMemo(() => {
    const query = search.trim().toLowerCase();
    if (!query) return [];
    return receptionCustomers.filter((customer) => {
      const fullName = `${customer.firstName} ${customer.lastName}`.toLowerCase();
      return fullName.includes(query) || customer.identificationNumber.toLowerCase().includes(query);
    });
  }, [receptionCustomers, search]);

  const selectedCustomer = useMemo(
    () => receptionCustomers.find((customer) => customer.id === selectedCustomerId) ?? null,
    [receptionCustomers, selectedCustomerId],
  );

  const selectedPlan = useMemo(
    () => receptionPlans.find((plan) => plan.id === selectedPlanId) ?? null,
    [receptionPlans, selectedPlanId],
  );
  const trainerPlanQuery = useQuery({
    queryKey: ["customer-training-plan", selectedCustomerId],
    queryFn: () => customersApi.trainingPlan(selectedCustomerId!),
    enabled: isTrainer && !!selectedCustomerId,
  });

  const activationMutation = useMutation({
    mutationFn: ({ customer, plan, startDate, endDate }: { customer: Customer; plan: Plan; startDate: string; endDate: string }) =>
      customersApi.update(customer.id, {
        branchId: customer.branchId || null,
        activeMembershipId: plan.id,
        activeMembershipStartsAtUtc: dateInputToIso(startDate),
        activeMembershipEndsAtUtc: dateInputToIso(endDate, true),
        firstName: customer.firstName,
        lastName: customer.lastName,
        identificationNumber: customer.identificationNumber,
        phone: customer.phone ?? null,
        birthDate: customer.birthDate ?? null,
        gender: customer.gender,
        emergencyContactName: customer.emergencyContactName ?? null,
        emergencyContactPhone: customer.emergencyContactPhone ?? null,
        status: 1,
      }),
    onSuccess: async (_, variables) => {
      await queryClient.invalidateQueries({ queryKey: ["customers"] });
      setSelectedCustomerId(variables.customer.id);
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });
  const assignTrainingMutation = useMutation({
    mutationFn: (customerId: string) => customersApi.generateTrainingPlan(customerId),
    onSuccess: async () => {
      await queryClient.invalidateQueries({ queryKey: ["customer-training-plan", selectedCustomerId] });
      setError(null);
    },
    onError: (mutationError) => setError(getErrorMessage(mutationError)),
  });

  if (sessionQuery.isLoading || summaryQuery.isLoading || ((isReception || isTrainer) && customersQuery.isLoading) || (isReception && plansQuery.isLoading) || (isTrainer && trainerPlanQuery.isLoading && !!selectedCustomerId)) {
    return <LoadingState label="Cargando panel de clientes Dorian..." />;
  }

  if (sessionQuery.error) {
    return <Alert>{getErrorMessage(sessionQuery.error)}</Alert>;
  }

  if (summaryQuery.error) {
    return <Alert>{getErrorMessage(summaryQuery.error)}</Alert>;
  }

  if (isReception && customersQuery.error) {
    return <Alert>{getErrorMessage(customersQuery.error)}</Alert>;
  }

  if (isReception && plansQuery.error) {
    return <Alert>{getErrorMessage(plansQuery.error)}</Alert>;
  }
  if (isTrainer && trainerPlanQuery.error) {
    return <Alert>{getErrorMessage(trainerPlanQuery.error)}</Alert>;
  }

  function handleSelectCustomer(customer: Customer) {
    setSelectedCustomerId(customer.id);
    setSelectedPlanId(customer.activeMembershipId ?? "");
    setStartsAt(toDateInput(customer.activeMembershipStartsAtUtc));
    setEndsAt(toDateInput(customer.activeMembershipEndsAtUtc));
    setError(null);
  }

  function handlePlanChange(planId: string) {
    setSelectedPlanId(planId);
    if (!startsAt) return;
    const plan = receptionPlans.find((item) => item.id === planId);
    if (!plan) return;
    const start = new Date(`${startsAt}T00:00:00`);
    start.setDate(start.getDate() + Math.max(plan.durationInDays - 1, 0));
    setEndsAt(start.toISOString().slice(0, 10));
  }

  function handleActivatePlan() {
    if (!selectedCustomer || !selectedPlan || !startsAt || !endsAt) {
      setError("Selecciona cliente, plan y fechas para activar el plan.");
      return;
    }

    if (new Date(`${endsAt}T00:00:00`) < new Date(`${startsAt}T00:00:00`)) {
      setError("La fecha final no puede ser menor que la fecha inicial.");
      return;
    }

    setError(null);
    activationMutation.mutate({ customer: selectedCustomer, plan: selectedPlan, startDate: startsAt, endDate: endsAt });
  }

  function handleAssignTraining() {
    if (!selectedCustomer) {
      setError("Selecciona un cliente para asignar el entrenamiento.");
      return;
    }

    setError(null);
    assignTrainingMutation.mutate(selectedCustomer.id);
  }

  if (isReception) {
    return (
      <div className="grid gap-6 xl:grid-cols-[0.95fr_1.05fr]">
        <Card>
          <SectionHeading
            eyebrow="Recepción"
            title="Buscar y activar cliente"
            description="Busca por nombre o cédula, selecciona el cliente y activa su plan con fecha inicial y final."
          />
          <div className="mt-6 space-y-4">
            <div>
              <Label>Buscar cliente</Label>
              <Input
                placeholder="Nombre o cédula"
                value={search}
                onChange={(event) => setSearch(event.target.value)}
              />
            </div>

            <div className="space-y-3">
              {filteredCustomers.slice(0, 8).map((customer) => (
                <button
                  key={customer.id}
                  type="button"
                  onClick={() => handleSelectCustomer(customer)}
                  className={`w-full rounded-2xl border px-4 py-4 text-left transition ${
                    selectedCustomerId === customer.id ? "border-[var(--accent)] bg-[var(--accent)]/10" : "border-white/10 bg-white/[0.03] hover:bg-white/[0.05]"
                  }`}
                >
                  <div className="font-semibold text-white">{customer.firstName} {customer.lastName}</div>
                  <div className="mt-1 text-sm text-slate-400">{customer.identificationNumber} · {customer.email}</div>
                  <div className="mt-2 text-xs text-slate-500">
                    {customer.activeMembershipName ? `${customer.activeMembershipName} · vence ${formatDate(customer.activeMembershipEndsAtUtc)}` : "Sin plan activo"}
                  </div>
                </button>
              ))}
            </div>

            {!search.trim() ? (
              <EmptyState title="Empieza la búsqueda" description="Escribe el nombre o la cédula del cliente para ver resultados." />
            ) : null}
            {search.trim() && !filteredCustomers.length ? <EmptyState title="Sin resultados" description="No encontramos clientes con ese nombre o cédula." /> : null}
          </div>
        </Card>

        <Card>
          <SectionHeading
            eyebrow="Activación"
            title={selectedCustomer ? `${selectedCustomer.firstName} ${selectedCustomer.lastName}` : "Selecciona un cliente"}
            description={selectedCustomer ? `Cédula: ${selectedCustomer.identificationNumber}` : "Primero elige un cliente desde la lista de búsqueda."}
          />

          {selectedCustomer ? (
            <div className="mt-6 space-y-4">
              <div className="grid gap-4 md:grid-cols-2">
                <div>
                  <Label>Plan</Label>
                  <select
                    className="h-11 w-full rounded-2xl border border-white/10 bg-white/5 px-4 text-sm text-white"
                    value={selectedPlanId}
                    onChange={(event) => handlePlanChange(event.target.value)}
                  >
                    <option value="">Selecciona plan</option>
                    {receptionPlans.map((plan) => (
                      <option key={plan.id} value={plan.id}>
                        {plan.name} · {plan.durationInDays} días
                      </option>
                    ))}
                  </select>
                </div>
                <div>
                  <Label>Estado actual</Label>
                  <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-3 text-sm text-slate-300">
                    {selectedCustomer.activeMembershipName ? `${selectedCustomer.activeMembershipName} · hasta ${formatDate(selectedCustomer.activeMembershipEndsAtUtc)}` : "Sin plan activo"}
                  </div>
                </div>
                <div>
                  <Label>Fecha inicial</Label>
                  <Input type="date" value={startsAt} onChange={(event) => setStartsAt(event.target.value)} />
                </div>
                <div>
                  <Label>Fecha final</Label>
                  <Input type="date" value={endsAt} onChange={(event) => setEndsAt(event.target.value)} />
                </div>
              </div>

              {selectedPlan ? (
                <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-4 text-sm text-slate-300">
                  Activarás <span className="font-semibold text-white">{selectedPlan.name}</span> por {selectedPlan.durationInDays} días.
                </div>
              ) : null}

              {error ? <Alert>{error}</Alert> : null}

              <Button onClick={handleActivatePlan} disabled={activationMutation.isPending}>
                {activationMutation.isPending ? "Activando..." : "Activar plan"}
              </Button>
            </div>
          ) : (
            <div className="mt-6">
              <EmptyState title="Sin cliente seleccionado" description="Busca por nombre o cédula y selecciona al cliente que vas a activar." />
            </div>
          )}
        </Card>
      </div>
    );
  }

  if (isTrainer) {
    return (
      <div className="grid gap-6 xl:grid-cols-[0.95fr_1.05fr]">
        <Card>
          <SectionHeading
            eyebrow="Trainer"
            title="Buscar cliente"
            description="Busca por nombre o cédula para localizar al cliente de tu sucursal y asignarle un entrenamiento."
          />
          <div className="mt-6 space-y-4">
            <div>
              <Label>Buscar cliente</Label>
              <Input placeholder="Nombre o cédula" value={search} onChange={(event) => setSearch(event.target.value)} />
            </div>

            <div className="space-y-3">
              {filteredCustomers.slice(0, 8).map((customer) => (
                <button
                  key={customer.id}
                  type="button"
                  onClick={() => handleSelectCustomer(customer)}
                  className={`w-full rounded-2xl border px-4 py-4 text-left transition ${
                    selectedCustomerId === customer.id ? "border-[var(--accent)] bg-[var(--accent)]/10" : "border-white/10 bg-white/[0.03] hover:bg-white/[0.05]"
                  }`}
                >
                  <div className="font-semibold text-white">{customer.firstName} {customer.lastName}</div>
                  <div className="mt-1 text-sm text-slate-400">{customer.identificationNumber}</div>
                </button>
              ))}
            </div>

            {!filteredCustomers.length ? <EmptyState title="Sin resultados" description="No encontramos clientes con ese nombre o cédula." /> : null}
          </div>
        </Card>

        <Card>
          <SectionHeading
            eyebrow="Entrenamiento"
            title={selectedCustomer ? `${selectedCustomer.firstName} ${selectedCustomer.lastName}` : "Selecciona un cliente"}
            description={selectedCustomer ? `Cédula: ${selectedCustomer.identificationNumber}` : "Primero elige un cliente desde la lista de búsqueda."}
          />

          {selectedCustomer ? (
            <div className="mt-6 space-y-4">
              <div className="grid gap-4 md:grid-cols-2">
                <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-4">
                  <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Estado fitness</p>
                  <p className="mt-2 text-sm text-white">{selectedCustomer.onboardingCompleted ? "Listo para asignar entrenamiento" : "Debe completar onboarding fitness"}</p>
                </div>
                <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-4">
                  <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Plan actual</p>
                  <p className="mt-2 text-sm text-white">{trainerPlanQuery.data ? trainerPlanQuery.data.currentPhaseName : "Sin entrenamiento asignado"}</p>
                </div>
              </div>

              {trainerPlanQuery.data ? (
                <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-4 text-sm text-slate-300">
                  El cliente ya tiene un entrenamiento con avance de <span className="font-semibold text-white">{trainerPlanQuery.data.progressPercent}%</span> y fase actual <span className="font-semibold text-white">{trainerPlanQuery.data.currentPhaseName}</span>.
                </div>
              ) : (
                <div className="rounded-2xl border border-white/10 bg-white/[0.03] px-4 py-4 text-sm text-slate-300">
                  Todavía no tiene un entrenamiento asignado. Puedes generarlo desde aquí.
                </div>
              )}

              {error ? <Alert>{error}</Alert> : null}

              <Button onClick={handleAssignTraining} disabled={assignTrainingMutation.isPending}>
                {assignTrainingMutation.isPending ? "Asignando..." : trainerPlanQuery.data ? "Regenerar entrenamiento" : "Asignar entrenamiento"}
              </Button>
            </div>
          ) : (
            <div className="mt-6">
              <EmptyState title="Sin cliente seleccionado" description="Busca por nombre o cédula y selecciona al cliente al que vas a asignar entrenamiento." />
            </div>
          )}
        </Card>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <Card className="overflow-hidden border-white/10 bg-[radial-gradient(circle_at_top_left,_rgba(45,212,191,0.16),_transparent_35%),linear-gradient(135deg,rgba(15,23,42,0.98),rgba(10,15,31,0.94))]">
        <div className="space-y-6">
          <SectionHeading
            eyebrow="Clientes Dorian"
            title="Base global de clientes"
            description="Resumen global del gimnasio Dorian. SuperAdmin y BranchAdmin ven el total del ecosistema, sin depender de una sucursal fija."
          />
          <div className="grid gap-4 md:grid-cols-3">
            <div className="rounded-3xl border border-white/10 bg-black/20 p-5">
              <p className="text-xs uppercase tracking-[0.28em] text-emerald-300/80">Total Dorian</p>
              <p className="mt-3 text-4xl font-semibold text-white">{metrics?.totalCustomers ?? 0}</p>
              <p className="mt-3 text-sm text-slate-300">Clientes registrados en toda la plataforma, sin depender de una sede fija.</p>
            </div>
            <div className="rounded-3xl border border-white/10 bg-black/20 p-5">
              <p className="text-xs uppercase tracking-[0.28em] text-cyan-300/80">Activos</p>
              <p className="mt-3 text-4xl font-semibold text-white">{metrics?.activeCustomers ?? 0}</p>
              <p className="mt-3 text-sm text-slate-300">Miembros con estado activo dentro del ecosistema Dorian.</p>
            </div>
            <div className="rounded-3xl border border-white/10 bg-black/20 p-5">
              <p className="text-xs uppercase tracking-[0.28em] text-amber-300/80">Inactivos</p>
              <p className="mt-3 text-4xl font-semibold text-white">{metrics?.inactiveCustomers ?? 0}</p>
              <p className="mt-3 text-sm text-slate-300">Clientes que hoy no están activos dentro del ecosistema Dorian.</p>
            </div>
          </div>
        </div>
      </Card>

      {!metrics?.totalCustomers ? (
        <Card>
          <EmptyState title="Sin clientes registrados" description="Cuando existan clientes en Dorian, esta pantalla mostrará aquí su total consolidado." />
        </Card>
      ) : null}
    </div>
  );
}
