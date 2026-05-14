import { apiFetch, authFetch } from "@/lib/api/client";
import type { AccessPass, ActivitySummary, BodySummary, Booking, Branch, CheckIn, ClassSession, Customer, CustomerFitnessProfile, CustomerSummary, DashboardSummary, MealPlan, NutritionProfile, Plan, Promotion, Session, StaffMember, TrainingPlan } from "@/lib/types";

export const authApi = {
  login: (payload: { email: string; password: string }) => authFetch<Session>("/login", { method: "POST", body: JSON.stringify(payload) }),
  logout: () => authFetch<void>("/logout", { method: "POST" }),
  session: () => authFetch<Session>("/session"),
};

export const branchesApi = {
  list: () => apiFetch<Branch[]>("/branches"),
  create: (payload: unknown) => apiFetch<Branch>("/branches", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: unknown) => apiFetch<Branch>(`/branches/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  remove: (id: string) => apiFetch<void>(`/branches/${id}`, { method: "DELETE" }),
};

export const customersApi = {
  list: () => apiFetch<Customer[]>("/customers"),
  summary: () => apiFetch<CustomerSummary>("/customers/summary"),
  listByBranch: (branchId: string) => apiFetch<Customer[]>(`/branches/${branchId}/customers`),
  fitnessProfile: (customerId: string) => apiFetch<CustomerFitnessProfile>(`/customers/${customerId}/fitness-profile`),
  bodySummary: (customerId: string) => apiFetch<BodySummary>(`/customers/${customerId}/body-summary`),
  nutritionProfile: (customerId: string) => apiFetch<NutritionProfile | undefined>(`/customers/${customerId}/nutrition-profile`),
  mealPlan: (customerId: string) => apiFetch<MealPlan[]>(`/customers/${customerId}/meal-plan`),
  trainingPlan: (customerId: string) => apiFetch<TrainingPlan | undefined>(`/customers/${customerId}/training-plan`),
  activitySummary: (customerId: string, range = 7) => apiFetch<ActivitySummary>(`/customers/${customerId}/activity-summary?range=${range}`),
  generateTrainingPlan: (customerId: string) => apiFetch<TrainingPlan>(`/customers/${customerId}/training-plan/generate`, { method: "POST" }),
  create: (payload: unknown) => apiFetch<Customer>("/customers", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: unknown) => apiFetch<Customer>(`/customers/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  remove: (id: string) => apiFetch<void>(`/customers/${id}`, { method: "DELETE" }),
};

export const staffApi = {
  list: () => apiFetch<StaffMember[]>("/staff"),
  get: (id: string) => apiFetch<StaffMember>(`/staff/${id}`),
  create: (payload: unknown) => apiFetch<StaffMember>("/staff", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: unknown) => apiFetch<StaffMember>(`/staff/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  remove: (id: string) => apiFetch<void>(`/staff/${id}`, { method: "DELETE" }),
};

export const plansApi = {
  list: () => apiFetch<Plan[]>("/plans"),
  create: (payload: unknown) => apiFetch<Plan>("/plans", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: unknown) => apiFetch<Plan>(`/plans/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  remove: (id: string) => apiFetch<void>(`/plans/${id}`, { method: "DELETE" }),
};

export const membershipsApi = plansApi;

export const classesApi = {
  list: () => apiFetch<ClassSession[]>("/classes"),
  listByBranch: (branchId: string) => apiFetch<ClassSession[]>(`/branches/${branchId}/classes`),
  create: (payload: unknown) => apiFetch<ClassSession>("/classes", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: unknown) => apiFetch<ClassSession>(`/classes/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  remove: (id: string) => apiFetch<void>(`/classes/${id}`, { method: "DELETE" }),
};

export const bookingsApi = {
  list: () => apiFetch<Booking[]>("/bookings"),
  create: (classId: string, payload: unknown) => apiFetch<Booking>(`/classes/${classId}/bookings`, { method: "POST", body: JSON.stringify(payload) }),
  cancel: (id: string) => apiFetch<Booking>(`/bookings/${id}/cancel`, { method: "PUT" }),
  attend: (id: string) => apiFetch<Booking>(`/bookings/${id}/attend`, { method: "PUT" }),
};

export const promotionsApi = {
  list: () => apiFetch<Promotion[]>("/promotions"),
  listByBranch: (branchId: string) => apiFetch<Promotion[]>(`/branches/${branchId}/promotions`),
  create: (payload: unknown) => apiFetch<Promotion>("/promotions", { method: "POST", body: JSON.stringify(payload) }),
  update: (id: string, payload: unknown) => apiFetch<Promotion>(`/promotions/${id}`, { method: "PUT", body: JSON.stringify(payload) }),
  remove: (id: string) => apiFetch<void>(`/promotions/${id}`, { method: "DELETE" }),
  activate: (id: string) => apiFetch<Promotion>(`/promotions/${id}/activate`, { method: "PUT" }),
  disable: (id: string) => apiFetch<Promotion>(`/promotions/${id}/disable`, { method: "PUT" }),
};

export const accessApi = {
  getAccessPass: (customerId: string) => apiFetch<AccessPass>(`/customers/${customerId}/access-pass`),
  regenerateAccessPass: (customerId: string) => apiFetch<AccessPass>(`/customers/${customerId}/access-pass/regenerate`, { method: "POST" }),
  getCheckInsByBranch: (branchId: string) => apiFetch<CheckIn[]>(`/branches/${branchId}/check-ins`),
  getCheckInsByCustomer: (customerId: string) => apiFetch<CheckIn[]>(`/customers/${customerId}/check-ins`),
  scan: (branchId: string, payload: { qrCodeValue: string }) => apiFetch<CheckIn>(`/branches/${branchId}/check-ins/scan`, { method: "POST", body: JSON.stringify(payload) }),
  manual: (branchId: string, payload: { customerId: string }) => apiFetch<CheckIn>(`/branches/${branchId}/check-ins/manual`, { method: "POST", body: JSON.stringify(payload) }),
};

export const dashboardApi = {
  summary: () => apiFetch<DashboardSummary>("/dashboard/summary"),
};

