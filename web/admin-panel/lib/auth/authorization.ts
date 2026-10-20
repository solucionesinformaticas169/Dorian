import { redirect } from "next/navigation";
import type { Session } from "@/lib/types";

export function hasRole(session: Session, roles: string[]) {
  return roles.some((role) => session.user.roles.includes(role));
}

export function resolveDefaultAdminRoute(session: Session) {
  if (hasRole(session, ["SuperAdmin", "BranchAdmin"])) return "/dashboard";
  if (hasRole(session, ["Reception", "Trainer"])) return "/customers";
  return "/login";
}

export function requireRoleAccess(session: Session, allowedRoles: string[]) {
  if (hasRole(session, allowedRoles)) return;
  redirect(resolveDefaultAdminRoute(session));
}
