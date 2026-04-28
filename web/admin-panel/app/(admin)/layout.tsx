import { AdminShell } from "@/components/layout/admin-shell";
import { requireServerSession } from "@/lib/auth/server";

export default async function AdminLayout({ children }: { children: React.ReactNode }) {
  const session = await requireServerSession();
  return <AdminShell session={session}>{children}</AdminShell>;
}

