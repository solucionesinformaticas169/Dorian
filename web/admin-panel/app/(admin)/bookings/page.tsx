"use client";

import { useMemo, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { DataCell, DataRow, DataTable } from "@/components/admin/data-table";
import { EmptyState, LoadingState } from "@/components/admin/page-state";
import { SectionHeading } from "@/components/admin/section-heading";
import { Alert } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Select } from "@/components/ui/select";
import { bookingsApi, branchesApi, classesApi, customersApi } from "@/lib/api/admin";
import { formatDateTime, getErrorMessage } from "@/lib/utils";

function getCapacityTone(reservedSpots: number, capacity: number) {
  const safeCapacity = Math.max(capacity, 0);
  const safeReserved = Math.max(reservedSpots, 0);
  const occupancyRate = safeCapacity > 0 ? (safeReserved / safeCapacity) * 100 : 0;

  if (safeCapacity > 0 && safeReserved >= safeCapacity) {
    return {
      badgeTone: "danger" as const,
      label: "Clase al tope",
      progressClassName: "bg-rose-400",
    };
  }

  if (occupancyRate >= 80) {
    return {
      badgeTone: "warning" as const,
      label: "Ultimos cupos",
      progressClassName: "bg-amber-400",
    };
  }

  return {
    badgeTone: "success" as const,
    label: "Disponible",
    progressClassName: "bg-emerald-400",
  };
}

export default function BookingsPage() {
  const [classId, setClassId] = useState("");
  const [inspectedClassId, setInspectedClassId] = useState("");
  const [showList, setShowList] = useState(false);

  const bookingsQuery = useQuery({ queryKey: ["bookings"], queryFn: bookingsApi.list });
  const branchesQuery = useQuery({ queryKey: ["branches"], queryFn: branchesApi.list });
  const customersQuery = useQuery({ queryKey: ["customers"], queryFn: customersApi.list });
  const classesQuery = useQuery({ queryKey: ["classes"], queryFn: classesApi.list });

  const branchMap = useMemo(() => Object.fromEntries((branchesQuery.data ?? []).map((branch) => [branch.id, branch.name])), [branchesQuery.data]);
  const customerMap = useMemo(() => Object.fromEntries((customersQuery.data ?? []).map((customer) => [customer.id, `${customer.firstName} ${customer.lastName}`])), [customersQuery.data]);
  const classSummaries = useMemo(
    () =>
      (classesQuery.data ?? [])
        .map((item) => {
          const reservedSpots = Math.max(item.reservedSpots, 0);
          const capacity = Math.max(item.capacity, 0);
          const availableSpots = Math.max(capacity - reservedSpots, 0);
          const occupancyRate = capacity > 0 ? Math.min((reservedSpots / capacity) * 100, 100) : 0;
          return {
            ...item,
            availableSpots,
            occupancyRate,
            ...getCapacityTone(reservedSpots, capacity),
          };
        })
        .sort((left, right) => new Date(left.startTime).getTime() - new Date(right.startTime).getTime()),
    [classesQuery.data],
  );
  const selectedClass = useMemo(() => classSummaries.find((item) => item.id === inspectedClassId) ?? null, [inspectedClassId, classSummaries]);
  const selectedClassBookings = useMemo(
    () =>
      (bookingsQuery.data ?? [])
        .filter((booking) => booking.classSessionId === inspectedClassId)
        .sort((left, right) => new Date(right.bookedAt).getTime() - new Date(left.bookedAt).getTime()),
    [bookingsQuery.data, inspectedClassId],
  );
  const isInspectDisabled = !classId;

  if (bookingsQuery.isLoading || branchesQuery.isLoading || customersQuery.isLoading || classesQuery.isLoading) return <LoadingState label="Cargando reservas..." />;
  if (bookingsQuery.error || branchesQuery.error || customersQuery.error || classesQuery.error) return <Alert>{getErrorMessage(bookingsQuery.error ?? branchesQuery.error ?? customersQuery.error ?? classesQuery.error)}</Alert>;

  return (
    <div className="grid gap-6 xl:grid-cols-[0.82fr_1.18fr]">
      <div className="space-y-4">
        <Card>
          <SectionHeading eyebrow="Reservations" title="Consultar reservas" description="Selecciona una clase para ver su sucursal, aforo y el listado de clientes reservados solo cuando lo necesites." />
          <form
            className="mt-6 space-y-4"
            onSubmit={(event) => {
              event.preventDefault();
              setInspectedClassId(classId);
              setShowList(false);
            }}
          >
            <div>
              <Label>Clase</Label>
              <Select value={classId} onChange={(event) => setClassId(event.target.value)}>
                <option value="">Selecciona una clase</option>
                {classSummaries.map((item) => (
                  <option key={item.id} value={item.id}>
                    {item.name} - {item.reservedSpots}/{item.capacity}
                  </option>
                ))}
              </Select>
            </div>
            <Button type="submit" disabled={isInspectDisabled}>Reservas</Button>
            {selectedClass ? (
              <div className="rounded-[24px] border border-white/10 bg-white/[0.03] p-4">
                <div className="flex flex-wrap items-start justify-between gap-3">
                  <div>
                    <p className="font-semibold text-white">{selectedClass.name}</p>
                    <p className="mt-1 text-sm text-slate-400">{branchMap[selectedClass.branchId] ?? selectedClass.branchId}</p>
                    <p className="mt-1 text-xs uppercase tracking-[0.22em] text-slate-500">{formatDateTime(selectedClass.startTime)}</p>
                  </div>
                  <Badge tone={selectedClass.badgeTone}>{selectedClass.label}</Badge>
                </div>
                <div className="mt-4 grid grid-cols-3 gap-3 text-center">
                  <div className="rounded-2xl border border-white/10 bg-black/20 px-3 py-3">
                    <p className="text-xs uppercase tracking-[0.18em] text-slate-500">Reservas</p>
                    <p className="mt-2 font-heading text-2xl text-white">{selectedClass.reservedSpots}</p>
                  </div>
                  <div className="rounded-2xl border border-white/10 bg-black/20 px-3 py-3">
                    <p className="text-xs uppercase tracking-[0.18em] text-slate-500">Aperturados</p>
                    <p className="mt-2 font-heading text-2xl text-white">{selectedClass.capacity}</p>
                  </div>
                  <div className="rounded-2xl border border-white/10 bg-black/20 px-3 py-3">
                    <p className="text-xs uppercase tracking-[0.18em] text-slate-500">Disponibles</p>
                    <p className="mt-2 font-heading text-2xl text-white">{selectedClass.availableSpots}</p>
                  </div>
                </div>
                <div className="mt-4">
                  <div className="h-2 overflow-hidden rounded-full bg-white/10">
                    <div className={`h-full rounded-full ${selectedClass.progressClassName}`} style={{ width: `${selectedClass.occupancyRate}%` }} />
                  </div>
                  <p className="mt-2 text-xs text-slate-400">{selectedClass.occupancyRate.toFixed(0)}% del aforo reservado</p>
                </div>
                <div className="mt-4 flex flex-wrap gap-3">
                  <Button type="button" variant="secondary" className="px-4 py-2" onClick={() => setShowList((current) => !current)}>
                    {showList ? "Ocultar listado" : "Listado"}
                  </Button>
                  <div className="rounded-full bg-white/8 px-4 py-2 text-xs font-semibold text-slate-300">
                    {selectedClassBookings.length} reservas registradas
                  </div>
                </div>
              </div>
            ) : null}
          </form>
        </Card>
        <Card>
          <SectionHeading eyebrow="Capacity" title="Panorama de cupos" description="Resumen compacto para ver reservas contra aforo sin cargar la tabla operativa." />
          <div className="mt-6 space-y-3">
            {!classSummaries.length ? <EmptyState title="Sin clases" description="No existen clases configuradas para revisar aforo." /> : null}
            {classSummaries.map((item) => (
              <div key={item.id} className="rounded-[24px] border border-white/10 bg-white/[0.03] p-4">
                <div className="flex flex-wrap items-start justify-between gap-3">
                  <div>
                    <p className="font-semibold text-white">{item.name}</p>
                    <p className="mt-1 text-xs uppercase tracking-[0.22em] text-slate-500">{formatDateTime(item.startTime)}</p>
                  </div>
                  <Badge tone={item.badgeTone}>{item.label}</Badge>
                </div>
                <div className="mt-4 flex items-center justify-between gap-4 text-sm text-slate-300">
                  <span>{item.reservedSpots}/{item.capacity} reservados</span>
                  <span>{item.availableSpots} disponibles</span>
                </div>
                <div className="mt-3 h-2 overflow-hidden rounded-full bg-white/10">
                  <div className={`h-full rounded-full ${item.progressClassName}`} style={{ width: `${item.occupancyRate}%` }} />
                </div>
              </div>
            ))}
          </div>
        </Card>
      </div>
      <div className="space-y-4">
        <SectionHeading eyebrow="Booking overview" title="Clientes reservados" description="Primero ves cuántos clientes tiene la clase y luego, si quieres, abres el detalle del listado." />
        {selectedClass ? (
          <Card>
            <div className="flex flex-wrap items-start justify-between gap-4">
              <div>
                <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Resumen de la clase</p>
                <h3 className="mt-3 font-heading text-2xl text-white">{selectedClass.name}</h3>
                <p className="mt-2 text-sm text-slate-400">
                  {branchMap[selectedClass.branchId] ?? selectedClass.branchId} | {formatDateTime(selectedClass.startTime)}
                </p>
              </div>
              <Badge tone={selectedClass.badgeTone}>{selectedClass.label}</Badge>
            </div>

            <div className="mt-6 grid gap-4 md:grid-cols-3">
              <div className="rounded-[26px] border border-emerald-400/20 bg-emerald-400/10 p-5">
                <p className="text-xs uppercase tracking-[0.24em] text-emerald-200/80">Clientes reservados</p>
                <p className="mt-3 font-heading text-5xl text-white">{selectedClassBookings.length}</p>
                <p className="mt-3 text-sm text-slate-200">Total de clientes que ya apartaron cupo en esta clase.</p>
              </div>
              <div className="rounded-[26px] border border-white/10 bg-white/[0.03] p-5">
                <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Cupos aperturados</p>
                <p className="mt-3 font-heading text-4xl text-white">{selectedClass.capacity}</p>
                <p className="mt-3 text-sm text-slate-300">Capacidad total disponible para esta sesión.</p>
              </div>
              <div className="rounded-[26px] border border-white/10 bg-white/[0.03] p-5">
                <p className="text-xs uppercase tracking-[0.24em] text-slate-500">Disponibles</p>
                <p className="mt-3 font-heading text-4xl text-white">{selectedClass.availableSpots}</p>
                <p className="mt-3 text-sm text-slate-300">Cupos que todavía no han sido tomados.</p>
              </div>
            </div>
          </Card>
        ) : null}
        {!inspectedClassId ? <EmptyState title="Selecciona una clase" description="Pulsa Reservas para cargar el resumen de una clase y habilitar su listado." /> : null}
        {inspectedClassId && !showList ? <EmptyState title="Listado oculto" description="Usa el boton Listado para ver los clientes reservados de esta clase." /> : null}
        {inspectedClassId && showList && !selectedClassBookings.length ? <EmptyState title="Sin reservas" description="Esta clase todavia no tiene clientes registrados." /> : null}
        {inspectedClassId && showList && selectedClassBookings.length ? (
          <Card>
            <div className="flex flex-wrap items-center justify-between gap-3">
              <div>
                <p className="text-xs font-semibold uppercase tracking-[0.24em] text-[var(--accent)]">Booking list</p>
                <h3 className="mt-2 font-heading text-2xl text-white">Detalle de clientes</h3>
                <p className="mt-2 text-sm text-slate-400">Listado de clientes que reservaron esta clase.</p>
              </div>
              <div className="rounded-full bg-white/8 px-4 py-2 text-xs font-semibold text-slate-300">
                {selectedClassBookings.length} clientes
              </div>
            </div>
            <div className="mt-5">
              <DataTable headers={["Cliente", "Sucursal", "Fecha de reserva"]}>
                {selectedClassBookings.map((booking) => (
                  <DataRow key={booking.id}>
                    <DataCell>{customerMap[booking.customerId] ?? booking.customerId}</DataCell>
                    <DataCell>{selectedClass ? branchMap[selectedClass.branchId] ?? selectedClass.branchId : booking.branchId}</DataCell>
                    <DataCell>{formatDateTime(booking.bookedAt)}</DataCell>
                  </DataRow>
                ))}
              </DataTable>
            </div>
          </Card>
        ) : null}
      </div>
    </div>
  );
}

