import { MessageCircle } from "lucide-react";

export function WhatsappFab() {
  const href = process.env.NEXT_PUBLIC_WHATSAPP_URL ?? "https://wa.me/593999999999";

  return (
    <a
      href={href}
      target="_blank"
      rel="noreferrer"
      className="fixed bottom-5 right-5 z-50 inline-flex h-14 w-14 items-center justify-center rounded-full border border-white/10 bg-[var(--accent)] text-black shadow-2xl shadow-[var(--accent)]/30 transition hover:scale-105 hover:bg-[var(--accent-strong)]"
      aria-label="WhatsApp"
    >
      <MessageCircle className="h-6 w-6" />
    </a>
  );
}
