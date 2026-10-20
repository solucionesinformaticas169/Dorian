import { requireServerSession } from "@/lib/auth/server";
import { requireRoleAccess } from "@/lib/auth/authorization";

export default async function StaffLayout({ children }: { children: React.ReactNode }) {
  const session = await requireServerSession();
  requireRoleAccess(session, ["SuperAdmin", "BranchAdmin"]);
  return children;
}
