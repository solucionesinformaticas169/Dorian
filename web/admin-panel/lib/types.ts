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
  openingHours?: string | null;
  mapUrl?: string | null;
  latitude?: number | null;
  longitude?: number | null;
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
  onboardingCompleted: boolean;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type CustomerFitnessProfile = {
  id?: string | null;
  customerId?: string | null;
  goal?: number | null;
  focusMuscleGroup?: number | null;
  experienceLevel?: number | null;
  gymType?: number | null;
  includeCardio: boolean;
  trainingDays: number[];
  preferredTrainingTime?: string | null;
  gender?: number | null;
  birthDate?: string | null;
  weightKg?: number | null;
  heightCm?: number | null;
  targetWeightKg?: number | null;
  notificationsEnabled: boolean;
  notificationIntensity?: number | null;
  onboardingCompleted: boolean;
  createdAtUtc?: string | null;
  updatedAtUtc?: string | null;
};

export type BodyWeightHistoryPoint = {
  measuredAt: string;
  weightKg: number;
  bmi: number;
};

export type BodyProgressPhoto = {
  id: string;
  customerId: string;
  photoUrl: string;
  takenAt: string;
  type: number;
  notes?: string | null;
  createdAtUtc: string;
};

export type BodySummary = {
  currentWeightKg?: number | null;
  targetWeightKg?: number | null;
  heightCm?: number | null;
  bmi?: number | null;
  bmiLabel: string;
  latestMeasurementDate?: string | null;
  weightHistory: BodyWeightHistoryPoint[];
  measurementsHistory: Array<Record<string, number | string | null>>;
  progressPhotos: BodyProgressPhoto[];
  daysSinceLastMeasurement?: number | null;
};

export type TrainingPlanExercise = {
  id: string;
  exerciseId?: string | null;
  name: string;
  muscleGroup: number;
  sets: number;
  reps: string;
  restSeconds: number;
  weightKg?: number | null;
  notes?: string | null;
  order: number;
};

export type TrainingPlanDay = {
  id: string;
  dayOfWeek: number;
  title: string;
  estimatedMinutes: number;
  intensity: number;
  completedAt?: string | null;
  exercises: TrainingPlanExercise[];
};

export type TrainingPlanWeek = {
  id: string;
  weekNumber: number;
  title: string;
  description: string;
  days: TrainingPlanDay[];
};

export type TrainingPlanPhase = {
  id: string;
  name: number;
  description: string;
  order: number;
  durationWeeks: number;
  isCurrent: boolean;
  weeks: TrainingPlanWeek[];
};

export type TrainingPlan = {
  id: string;
  customerId: string;
  goal: number;
  experienceLevel: number;
  focusMuscleGroup: number;
  status: number;
  startDate: string;
  endDate?: string | null;
  currentPhaseName: string;
  completedDaysCount: number;
  totalDaysCount: number;
  progressPercent: number;
  phases: TrainingPlanPhase[];
};

export type ActivitySummaryMuscleGroup = {
  muscleGroup: string;
  sessions: number;
  exercisesCompleted: number;
  percentage: number;
  fatigueStatus: string;
};

export type ActivityByDayPoint = {
  day: string;
  activityCount: number;
  durationSeconds: number;
  caloriesEstimated: number;
  exercisesCompleted: number;
};

export type ActivityHistoryItem = {
  id: string;
  title: string;
  completedAt: string;
  durationSeconds: number;
  caloriesEstimated: number;
  exercisesCompleted: number;
  muscleGroups: string[];
  notes?: string | null;
};

export type ActivitySummary = {
  rangeInDays: number;
  daysTrained: number;
  totalDurationSeconds: number;
  caloriesEstimated: number;
  exercisesCompleted: number;
  seriesCompleted: number;
  repsCompleted: number;
  totalLoadKg?: number | null;
  muscleGroups: ActivitySummaryMuscleGroup[];
  activityByDay: ActivityByDayPoint[];
  recentActivities: ActivityHistoryItem[];
};

export type NutritionProfile = {
  id: string;
  customerId: string;
  goal: number;
  dailyCaloriesTarget: number;
  proteinGrams: number;
  carbsGrams: number;
  fatGrams: number;
  mealsPerDay: number;
  waterLitersTarget: number;
  dietaryRestrictions?: string | null;
  disclaimer: string;
  createdAtUtc: string;
  updatedAtUtc?: string | null;
};

export type MealItem = {
  id: string;
  mealType: number;
  name: string;
  description: string;
  calories: number;
  proteinGrams: number;
  carbsGrams: number;
  fatGrams: number;
};

export type MealPlan = {
  id: string;
  customerId: string;
  title: string;
  description: string;
  dayOfWeek?: number | null;
  items: MealItem[];
  createdAtUtc: string;
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

export type DashboardBranchActivity = {
  branchId: string;
  branchName: string;
  activityCount: number;
  activeCustomersCount: number;
  todayClassesCount: number;
  todayCheckInsCount: number;
};

export type DashboardClassOccupancy = {
  classSessionId: string;
  className: string;
  branchName: string;
  startTime: string;
  reservedSpots: number;
  capacity: number;
  occupancyRate: number;
};

export type DashboardSummary = {
  activeCustomersCount: number;
  todayClassesCount: number;
  todayCheckInsCount: number;
  estimatedRevenue: number;
  mostActiveBranchName: string;
  branchActivity: DashboardBranchActivity[];
  classOccupancy: DashboardClassOccupancy[];
  estimatedRevenueFormula: string;
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

export const fitnessGoalMap: Record<number, string> = {
  1: "Perder peso",
  2: "Definicion muscular",
  3: "Hipertrofia",
  4: "Mantener condicion",
};

export const focusMuscleGroupMap: Record<number, string> = {
  1: "Balanceado",
  2: "Pecho",
  3: "Espalda",
  4: "Brazos",
  5: "Piernas",
  6: "Abdomen",
  7: "Gluteos",
};

export const fitnessExperienceLevelMap: Record<number, string> = {
  1: "Principiante",
  2: "Intermedio",
  3: "Avanzado",
};

export const trainingDayMap: Record<number, string> = {
  1: "Lunes",
  2: "Martes",
  3: "Miercoles",
  4: "Jueves",
  5: "Viernes",
  6: "Sabado",
  7: "Domingo",
};

export const classStatusMap: Record<number, string> = {
  1: "Programada",
  2: "Cancelada",
  3: "Completada",
};

export const trainingPlanStatusMap: Record<number, string> = {
  1: "Activo",
  2: "Completado",
  3: "Pausado",
  4: "Cancelado",
};

export const trainingPhaseNameMap: Record<number, string> = {
  1: "Resistencia",
  2: "Fuerza",
  3: "Hipertrofia",
  4: "Definicion",
};

export const trainingDayIntensityMap: Record<number, string> = {
  1: "Baja",
  2: "Media",
  3: "Alta",
};

export const mealTypeMap: Record<number, string> = {
  1: "Desayuno",
  2: "Almuerzo",
  3: "Cena",
  4: "Snack",
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

