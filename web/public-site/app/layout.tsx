import type { Metadata } from "next";
import { Plus_Jakarta_Sans, Space_Grotesk } from "next/font/google";
import { SiteFooter } from "@/components/marketing/site-footer";
import { SiteHeader } from "@/components/marketing/site-header";
import { WhatsappFab } from "@/components/marketing/whatsapp-fab";
import "@/app/globals.css";

const jakarta = Plus_Jakarta_Sans({ subsets: ["latin"], variable: "--font-jakarta" });
const space = Space_Grotesk({ subsets: ["latin"], variable: "--font-space" });

export const metadata: Metadata = {
  metadataBase: new URL("https://dorianfitness.example"),
  title: {
    default: "Gimnasio Dorian | Gimnasio premium multi-sucursal",
    template: "%s | Gimnasio Dorian",
  },
  description: "Gimnasio premium con clases dirigidas, promociones activas, membresias flexibles y una experiencia conectada desde la app.",
  keywords: ["gimnasio", "fitness", "clases", "membresias", "entrenamiento"],
  openGraph: {
    title: "Gimnasio Dorian",
    description: "Entrenamiento premium, tecnologia y comunidad en una sola experiencia.",
    type: "website",
  },
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="es">
      <body className={`${jakarta.variable} ${space.variable} font-sans`}>
        <SiteHeader />
        {children}
        <SiteFooter />
        <WhatsappFab />
      </body>
    </html>
  );
}
