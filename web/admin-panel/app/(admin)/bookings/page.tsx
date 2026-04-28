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
import { Label } from "@/components/ui/label";
import { Select } from "@/components/ui/select";
import { bookingsApi, classesApi, customersApi } from "@/lib/api/admin";
import { bookingStatusMap } from "@/lib/types";
import { formatDateTime, getErrorMessage } from "@/lib/utils";

export default function BookingsPage() {
  const queryClient = useQueryClient();
  const [classId, setClassId] = useState("");
  const [customerId, setCustomerId] = useState("");
  const [error, setError] = useState<string | null>(null);

  const bookingsQuery = useQuery({ queryKey: ["bookings"], queryFn: bookingsApi.list });
  const customersQuery = useQuery({ queryKey: ["customers"], queryFn: customersApi.list });
  const classesQuery = useQuery({ queryKey: ["classes"], queryFn: classesApi.list });

  const createMutation = useMutation({ mutationFn: () => bookingsApi.create(classId, { customerId }), onSuccess: async () => { await queryClient.invalidateQueries({ queryKey: ["bookings"] }); setError(null); }, onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const cancelMutation = useMutation({ mutationFn: bookingsApi.cancel, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["bookings"] }), onError: (mutationError) => setError(getErrorMessage(mutationError)) });
  const attendMutation = useMutation({ mutationFn: bookingsApi.attend, onSuccess: async () => queryClient.invalidateQueries({ queryKey: ["bookings"] }), onError: (mutationError) => setError(getErrorMessage(mutationError)) });

  const customerMap = useMemo(() => Object.fromEntries((customersQuery.data ?? []).map((customer) => [customer.id, `${customer.firstName} ${customer.lastName}`])), [customersQuery.data]);
  const classMap = useMemo(() => Object.fromEntries((classesQuery.data ?? []).map((item) => [item.id, item.name])), [classesQuery.data]);

  if (bookingsQuery.isLoading || customersQuery.isLoading || classesQuery.isLoading) return <LoadingState label="Cargando reservas..." />;
  if (bookingsQuery.error || customersQuery.error || classesQuery.error) return <Alert>{getErrorMessage(bookingsQuery.error ?? customersQuery.error ?? classesQuery.error)}</Alert>;

  const bookings = bookingsQuery.data ?? [];

  return (
    <div className="grid gap-6 xl:grid-cols-[0.82fr_1.18fr]">
      <Card>
        <SectionHeading eyebrow="Reservations" title="Crear reserva" description="Asocia un cliente con una clase real del backend y administra el estado operativo." />
        <form className="mt-6 space-y-4" onSubmit={(event) => { event.preventDefault(); createMutation.mutate(); }}>
          <div><Label>Clase</Label><Select value={classId} onChange={(event) => setClassId(event.target.value)}><option value="">Selecciona una clase</option>{classesQuery.data?.map((item) => <option key={item.id} value={item.id}>{item.name}</option>)}</Select></div>
          <div><Label>Cliente</Label><Select value={customerId} onChange={(event) => setCustomerId(event.target.value)}><option value="">Selecciona un cliente</option>{customersQuery.data?.map((customer) => <option key={customer.id} value={customer.id}>{customer.firstName} {customer.lastName}</option>)}</Select></div>
          {error ? <Alert>{error}</Alert> : null}
          <Button type="submit" disabled={createMutation.isPending}>Crear reserva</Button>
        </form>
      </Card>
      <div className="space-y-4">
        <SectionHeading eyebrow="Live bookings" title="Reservas registradas" description="Marca cancelaciones o asistencia desde el mismo panel administrativo." />
        {!bookings.length ? <EmptyState title="Sin reservas" description="No existen reservas activas o historicas para mostrar." /> : null}
        {bookings.length ? <DataTable headers={["Cliente", "Clase", "Fecha", "Estado", "Acciones"]}>{bookings.map((booking) => <DataRow key={booking.id}><DataCell>{customerMap[booking.customerId] ?? booking.customerId}</DataCell><DataCell>{classMap[booking.classSessionId] ?? booking.classSessionId}</DataCell><DataCell>{formatDateTime(booking.bookedAt)}</DataCell><DataCell><Badge tone={booking.status === 1 ? "success" : booking.status === 2 ? "warning" : "neutral"}>{bookingStatusMap[booking.status]}</Badge></DataCell><DataCell className="flex gap-2">{booking.status === 1 ? <><Button variant="secondary" className="px-3 py-2" onClick={() => attendMutation.mutate(booking.id)}>Asistencia</Button><Button variant="danger" className="px-3 py-2" onClick={() => cancelMutation.mutate(booking.id)}>Cancelar</Button></> : <span className="text-xs text-slate-500">Sin acciones</span>}</DataCell></DataRow>)}</DataTable> : null}
      </div>
    </div>
  );
}

