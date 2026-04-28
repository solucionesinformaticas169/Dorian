import { forwardRef } from "react";
import { cn } from "@/lib/utils";

type ButtonProps = React.ButtonHTMLAttributes<HTMLButtonElement> & {
  variant?: "primary" | "secondary" | "ghost" | "danger";
};

const variants: Record<NonNullable<ButtonProps["variant"]>, string> = {
  primary: "bg-[var(--accent)] text-slate-950 shadow-lg shadow-[var(--accent)]/20 hover:bg-[var(--accent-strong)]",
  secondary: "bg-white/8 text-white ring-1 ring-white/10 hover:bg-white/12",
  ghost: "bg-transparent text-slate-300 hover:bg-white/6 hover:text-white",
  danger: "bg-rose-500/90 text-white hover:bg-rose-500",
};

export const Button = forwardRef<HTMLButtonElement, ButtonProps>(function Button(
  { className, variant = "primary", type = "button", ...props },
  ref,
) {
  return (
    <button
      ref={ref}
      type={type}
      className={cn(
        "inline-flex items-center justify-center rounded-2xl px-4 py-2 text-sm font-semibold transition duration-200 disabled:cursor-not-allowed disabled:opacity-50",
        variants[variant],
        className,
      )}
      {...props}
    />
  );
});

