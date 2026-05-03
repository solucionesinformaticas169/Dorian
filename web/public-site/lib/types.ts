export type Branch = {
  id: string;
  code: string;
  name: string;
  city: string;
  address?: string | null;
  phoneNumber?: string | null;
  openingHours?: string | null;
  mapUrl?: string | null;
  latitude?: number | null;
  longitude?: number | null;
  isActive: boolean;
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
};

export type Membership = {
  id: string;
  branchId: string;
  name: string;
  durationInDays: number;
  price: number;
  currency: string;
  isActive: boolean;
};

export type AuthResponse = {
  accessToken: string;
  refreshToken: string;
  accessTokenExpiresAtUtc: string;
  refreshTokenExpiresAtUtc: string;
  user: {
    id: string;
    email: string;
    fullName: string;
    branchId?: string | null;
    roles: string[];
  };
};
