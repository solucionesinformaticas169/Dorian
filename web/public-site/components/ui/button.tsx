import Link from "next/link";
import { cn } from "@/lib/utils";

type ButtonProps = {
  href?: string;
  children: React.ReactNode;
  className?: string;
  variant?: "primary" | "secondary" | "ghost";
};

const variants = {
  primary: "bg-[var(--accent)] text-slate-950 hover:bg-[var(--accent-strong)] shadow-lg shadow-[var(--accent)]/20",
  secondary: "bg-white/8 text-white ring-1 ring-white/10 hover:bg-white/12",
  ghost: "bg-transparent text-slate-300 hover:bg-white/6 hover:text-white",
};

export function Button({ href, children, className, variant = "primary" }: ButtonProps) {
  const classes = cn("inline-flex items-center justify-center rounded-full px-6 py-3 text-sm font-semibold transition", variants[variant], className);
  if (href && (href.startsWith("http://") || href.startsWith("https://"))) {
    return <a href={href} className={classes} target="_blank" rel="noreferrer">{children}</a>;
  }
  if (href) return <Link href={href} className={classes}>{children}</Link>;
  return <button className={classes}>{children}</button>;
}
