"use client";

import { useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { Dumbbell, ShieldCheck } from "lucide-react";
import { authApi } from "@/lib/api/admin";
import { getErrorMessage } from "@/lib/utils";
import { Alert } from "@/components/ui/alert";
import { Button } from "@/components/ui/button";
import { Card, CardDescription, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

export function LoginForm() {
  const router = useRouter();
  const [email, setEmail] = useState("superadmin@dorian.test");
  const [password, setPassword] = useState("Pass1234!");
  const [error, setError] = useState<string | null>(null);
  const [isPending, startTransition] = useTransition();

  function onSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);

    startTransition(async () => {
      try {
        await authApi.login({ email, password });
        router.replace("/dashboard");
        router.refresh();
      } catch (submissionError) {
        setError(getErrorMessage(submissionError));
      }
    });
  }

  return (
    <div className="grid min-h-screen lg:grid-cols-[1.1fr_0.9fr]">
      <section className="relative hidden overflow-hidden lg:block">
        <div className="absolute inset-0 bg-[radial-gradient(circle_at_20%_20%,_rgba(35,214,154,0.35),_transparent_30%),radial-gradient(circle_at_80%_30%,_rgba(14,165,233,0.18),_transparent_24%),linear-gradient(180deg,_#06131f_0%,_#02040a_100%)]" />
        <div className="relative flex h-full flex-col justify-between p-12">
          <div className="flex items-center gap-3 text-[var(--accent)]">
            <div className="rounded-2xl border border-[var(--accent)]/20 bg-[var(--accent)]/10 p-3">
              <Dumbbell className="h-7 w-7" />
            </div>
            <span className="font-heading text-2xl font-semibold text-white">Dorian Admin</span>
          </div>
          <div className="max-w-xl space-y-6">
            <p className="text-xs font-semibold uppercase tracking-[0.36em] text-[var(--accent)]">Premium fitness operations</p>
            <h1 className="font-heading text-6xl font-semibold leading-none text-white">Una cabina de control para cada sucursal, clase y acceso QR.</h1>
            <p className="max-w-lg text-lg text-slate-300">
              Gestiona membresias, clientes, promociones y check-ins con una interfaz de estilo SaaS enfocada en la operacion diaria del gimnasio.
            </p>
          </div>
          <div className="flex items-center gap-3 rounded-[28px] border border-white/10 bg-white/5 p-5 text-slate-300 backdrop-blur">
            <ShieldCheck className="h-5 w-5 text-[var(--accent)]" />
            Tokens protegidos con cookies httpOnly y proxy seguro desde Next.js para la demo.
          </div>
        </div>
      </section>

      <section className="flex items-center justify-center px-4 py-10 sm:px-6 lg:px-10">
        <Card className="w-full max-w-xl rounded-[32px] border-white/10 bg-slate-950/85 p-8">
          <p className="text-xs font-semibold uppercase tracking-[0.32em] text-[var(--accent)]">Ingreso administrativo</p>
          <CardTitle className="mt-4 text-4xl">Accede al panel</CardTitle>
          <CardDescription className="mt-3 text-base">
            Usa un rol existente del backend para operar en tiempo real sobre las APIs del MVP.
          </CardDescription>

          <form className="mt-8 space-y-5" onSubmit={onSubmit}>
            <div>
              <Label htmlFor="email">Correo</Label>
              <Input id="email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} placeholder="superadmin@dorian.test" />
            </div>
            <div>
              <Label htmlFor="password">Contrasena</Label>
              <Input id="password" type="password" value={password} onChange={(event) => setPassword(event.target.value)} placeholder="Pass1234!" />
            </div>
            {error ? <Alert>{error}</Alert> : null}
            <Button type="submit" className="h-12 w-full text-base" disabled={isPending}>
              {isPending ? "Ingresando..." : "Entrar al admin"}
            </Button>
          </form>

          <div className="mt-8 rounded-[24px] border border-white/10 bg-white/[0.03] p-4 text-sm text-slate-400">
            Credenciales demo sugeridas: <span className="text-white">superadmin@dorian.test / Pass1234!</span>
          </div>
        </Card>
      </section>
    </div>
  );
}

