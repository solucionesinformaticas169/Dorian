"use client";

import { useEffect, useMemo, useState } from "react";
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
import { accessApi, branchesApi, customersApi } from "@/lib/api/admin";
import { accessPassStatusMap, checkInSourceMap, checkInStatusMap } from "@/lib/types";
import { formatDateTime, getErrorMessage } from "@/lib/utils";

export default function AccessPage() {
  const queryClient = useQueryClient();
  const [selectedBranchId, setSelectedBranchId] = useState("");
  const [selectedCustomerId, setSelectedCustomerId] = useState("");
  const [qrCodeValue, setQrCodeValue] = useState("");
  const [manualCustomerId, setManualCustomerId] = useState("");
  const [error, setError] = useState<string | null>(null);

  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const customersQuery = useQuery({ queryKey: ["customers"], queryFn: customersApi.list });

  useEffect(() => {
    if (!selectedBranchId && branchesQuery.data?.[0]?.id) setSelectedBranchId(branchesQuery.data[0].id);
  }, [branchesQuery.data, selectedBranchId]);

  const checkInsQuery = useQuery({ queryKey: ["checkins", selectedBranchId], queryFn: () => accessApi.getCheckInsByBranch(selectedBranchId), enabled: Boolean(selectedBranchId) });
  const accessPassQuery = useQuery({ queryKey: ["access-pass", selectedCustomerId], queryFn: () => accessApi.getAccessPass(selectedCustomerId), enabled: Boolean(selectedCustomerId) });

  const scanMutation = useMutation({ mutationFn: () => accessApi.scan(selectedBranchId, { qrCodeValue }), onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ["checkins", selectedBranchId] }); setError(null); setQrCodeValue(""); }, onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const manualMutation = useMutation({ mutationFn: () => accessApi.manual(selectedBranchId, { customerId: manualCustomerId }), onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ["checkins", selectedBranchId] }); setError(null); }, onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const regenerateMutation = useMutation({ mutationFn: () => accessApi.regenerateAccessPass(selectedCustomerId), onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ["access-pass", selectedCustomerId] }); setError(null); }, onError: (mutationError) => setError(getErrorMessage(mutationError)) });

  const customers = useMemo(() => (customersQuery.data ?? []).filter((customer) => !selectedBranchId || customer.branchId === selectedBranchId), [customersQuery.data, selectedBranchId]);

  if (branchesQuery.isLoading || customersQuery.isLoading) return <LoadingState label="Cargando modulo de accesos..." />;
  if (branchesQuery.error || customersQuery.error) return <Alert>{getErrorMessage(branchesQuery.error ?? customersQuery.error)}</Alert>;

  return (
    <div className="grid gap-6 xl:grid-cols-[0.9fr_1.1fr]">
      <div className="space-y-6">
        <Card>
          <SectionHeading eyebrow="Access pass" title="QR del cliente" description="Consulta o regenera el QR de acceso de un cliente real del backend." />
          <div className="mt-6 space-y-4">
            <div><Label>Sucursal</Label><Select value={selectedBranchId} onChange={(event) => setSelectedBranchId(event.target.value)}><option value="">Selecciona sucursal</option>{branchesQuery.data?.map((branch) => <option key={branch.id} value={branch.id}>{branch.name}</option>)}</Select></div>
            <div><Label>Cliente</Label><Select value={selectedCustomerId} onChange={(event) => setSelectedCustomerId(event.target.value)}><option value="">Selecciona cliente</option>{customers.map((customer) => <option key={customer.id} value={customer.id}>{customer.firstName} {customer.lastName}</option>)}</Select></div>
            {accessPassQuery.data ? <div className="rounded-[28px] border border-white/10 bg-white/[0.03] p-5"><p className="text-xs uppercase tracking-[0.24em] text-slate-500">QR actual</p><p className="mt-3 font-heading text-2xl text-white">{accessPassQuery.data.qrCodeValue}</p><p className="mt-2 text-sm text-slate-400">Vence: {formatDateTime(accessPassQuery.data.expiresAt)}</p><div className="mt-3"><Badge tone={accessPassQuery.data.status === 1 ? "success" : accessPassQuery.data.status === 2 ? "warning" : "danger"}>{accessPassStatusMap[accessPassQuery.data.status]}</Badge></div></div> : null}
            <Button onClick={() => regenerateMutation.mutate()} disabled={!selectedCustomerId || regenerateMutation.isPending}>Regenerar QR</Button>
          </div>
        </Card>

        <Card>
          <SectionHeading eyebrow="Front desk" title="Validar ingreso" description="Usa lectura QR o registro manual por cliente para recepcion." />
          <div className="mt-6 grid gap-6 md:grid-cols-2">
            <form className="space-y-4" onSubmit={(event) => { event.preventDefault(); scanMutation.mutate(); }}>
              <Label>QR escaneado</Label>
              <Input value={qrCodeValue} onChange={(event) => setQrCodeValue(event.target.value)} placeholder="ACC-..." />
              <Button type="submit" disabled={!selectedBranchId || !qrCodeValue}>Validar QR</Button>
            </form>
            <form className="space-y-4" onSubmit={(event) => { event.preventDefault(); manualMutation.mutate(); }}>
              <Label>Check-in manual</Label>
              <Select value={manualCustomerId} onChange={(event) => setManualCustomerId(event.target.value)}><option value="">Selecciona cliente</option>{customers.map((customer) => <option key={customer.id} value={customer.id}>{customer.firstName} {customer.lastName}</option>)}</Select>
              <Button type="submit" variant="secondary" disabled={!selectedBranchId || !manualCustomerId}>Registrar manual</Button>
            </form>
          </div>
          {error ? <div className="mt-4"><Alert>{error}</Alert></div> : null}
        </Card>
      </div>

      <div className="space-y-4">
        <SectionHeading eyebrow="Branch feed" title="Check-ins por sucursal" description="Vista de ingresos por sede, con fuente, estado y motivo de rechazo." />
        {!selectedBranchId ? <EmptyState title="Selecciona una sucursal" description="Elige una sede para consultar su actividad de acceso." /> : null}
        {checkInsQuery.isLoading ? <LoadingState label="Cargando check-ins..." /> : null}
        {checkInsQuery.error ? <Alert>{getErrorMessage(checkInsQuery.error)}</Alert> : null}
{selectedBranchId && !checkInsQuery.isLoading && !checkInsQuery.data?.length ? <EmptyState title="Sin registros de ingreso" description="Todavía no hay eventos de acceso para esta sucursal." /> : null}
        {checkInsQuery.data?.length ? <DataTable headers={["Cliente", "Momento", "Fuente", "Estado", "Detalle"]}>{checkInsQuery.data.map((item) => <DataRow key={item.id}><DataCell>{customersQuery.data?.find((customer) => customer.id === item.customerId)?.firstName ?? item.customerId}</DataCell><DataCell>{formatDateTime(item.checkedInAt)}</DataCell><DataCell>{checkInSourceMap[item.source]}</DataCell><DataCell><Badge tone={item.status === 1 ? "success" : "danger"}>{checkInStatusMap[item.status]}</Badge></DataCell><DataCell>{item.rejectionReason || "Ingreso correcto"}</DataCell></DataRow>)}</DataTable> : null}
      </div>
    </div>
  );
}

