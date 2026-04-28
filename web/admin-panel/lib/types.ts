export type AuthUser = {
  id: string;
  email: string;
  fullName: string;
  branchId?: string | null;
  roles: string[];
};

export type AuthResponse = {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
  user: AuthUser;
};

export type Session = {
  user: AuthUser;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
};

export type Branch = {
  id: string;
  code: string;
  name: string;
  city: string;
  address?: string | null;
  phoneNumber?: string | null;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type Customer = {
  id: string;
  userId: string;
  email: string;
  branchId: string;
  activeMembershipId?: string | null;
  activeMembershipStartsAtUtc?: string | null;
  activeMembershipEndsAtUtc?: string | null;
  firstName: string;
  lastName: string;
  identificationNumber: string;
  phone?: string | null;
  birthDate?: string | null;
  gender: number;
  emergencyContactName?: string | null;
  emergencyContactPhone?: string | null;
  status: number;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type Membership = {
  id: string;
  branchId: string;
  name: string;
  durationInDays: number;
  price: number;
  currency: string;
  isActive: boolean;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type ClassSession = {
  id: string;
  branchId: string;
  trainerUserId?: string | null;
  name: string;
  description?: string | null;
  startTime: string;
  endTime: string;
  capacity: number;
  status: number;
  reservedSpots: number;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type Booking = {
  id: string;
  customerId: string;
  classSessionId: string;
  branchId: string;
  status: number;
  bookedAt: string;
  cancelledAt?: string | null;
};

export type Promotion = {
  id: string;
  branchId?: string | null;
  title: string;
  description: string;
  imageUrl?: string | null;
  discountType: number;
  discountValue?: number | null;
  startsAt: string;
  endsAt: string;
  status: number;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type AccessPass = {
  id: string;
  customerId: string;
  qrCodeValue: string;
  expiresAt: string;
  status: number;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type CheckIn = {
  id: string;
  customerId: string;
  branchId: string;
  checkedInAt: string;
  checkedInByUserId?: string | null;
  source: number;
  status: number;
  rejectionReason?: string | null;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type ApiErrorPayload = {
  message?: string;
  detail?: string;
  title?: string;
  errors?: Record<string, string[]>;
};

export const customerStatusMap: Record<number, string> = {
  1: "Activo",
  2: "Inactivo",
  3: "Suspendido",
};

export const genderMap: Record<number, string> = {
  1: "Masculino",
  2: "Femenino",
  3: "Otro",
};

export const classStatusMap: Record<number, string> = {
  1: "Programada",
  2: "Cancelada",
  3: "Completada",
};

export const bookingStatusMap: Record<number, string> = {
  1: "Reservada",
  2: "Cancelada",
  3: "Asistio",
  4: "No asistio",
};

export const promotionDiscountTypeMap: Record<number, string> = {
  1: "Porcentaje",
  2: "Monto fijo",
  3: "Informativa",
};

export const promotionStatusMap: Record<number, string> = {
  1: "Borrador",
  2: "Activa",
  3: "Expirada",
  4: "Deshabilitada",
};

export const accessPassStatusMap: Record<number, string> = {
  1: "Activa",
  2: "Expirada",
  3: "Revocada",
};

export const checkInStatusMap: Record<number, string> = {
  1: "Aceptado",
  2: "Rechazado",
};

export const checkInSourceMap: Record<number, string> = {
  1: "QR",
  2: "Manual",
};

