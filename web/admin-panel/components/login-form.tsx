"use client";

import Image from "next/image";
import { useState, useTransition } from "react";
import { useRouter } from "next/navigation";
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
    <div className="flex min-h-screen items-center justify-center bg-[radial-gradient(circle_at_top,_rgba(255,106,31,0.18),_transparent_24%),linear-gradient(180deg,_#1a100b_0%,_#090909_55%,_#030303_100%)] px-4 py-10 sm:px-6 lg:px-10">
      <section className="flex w-full items-center justify-center">
        <Card className="w-full max-w-xl rounded-[32px] border-white/10 bg-slate-950/85 p-8">
          <div className="mb-6 flex items-center justify-center">
            <div className="flex h-16 w-16 items-center justify-center rounded-2xl border border-[var(--accent)]/20 bg-white/5 p-3">
              <Image src="/brand/dorian-logo.png" alt="Gimnasio Dorian" width={44} height={44} className="h-auto w-full" priority />
            </div>
          </div>

          <p className="text-center text-xs font-semibold uppercase tracking-[0.32em] text-[var(--accent)]">Ingreso administrativo</p>
          <CardTitle className="mt-4 text-center text-4xl">Accede al panel</CardTitle>
          <CardDescription className="mt-3 text-center text-base">
            Ingresa con tus credenciales para continuar.
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
        </Card>
      </section>
    </div>
  );
}
