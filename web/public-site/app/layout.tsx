import type { Metadata } from "next";
import { Archivo_Narrow, Hanken_Grotesk, JetBrains_Mono } from "next/font/google";
import { SiteFooter } from "@/components/marketing/site-footer";
import { SiteHeader } from "@/components/marketing/site-header";
import { WhatsappFab } from "@/components/marketing/whatsapp-fab";
import "@/app/globals.css";

const hanken = Hanken_Grotesk({ subsets: ["latin"], variable: "--font-hanken" });
const archivo = Archivo_Narrow({ subsets: ["latin"], variable: "--font-archivo" });
const jetbrains = JetBrains_Mono({ subsets: ["latin"], variable: "--font-jetbrains" });

export const metadata: Metadata = {
  metadataBase: new URL("https://dorianfitness.example"),
  title: {
    default: "Gimnasio Dorian | Gimnasio premium multi-sucursal",
    template: "%s | Gimnasio Dorian",
  },
  description: "Gimnasio premium con clases dirigidas, promociones activas, membresías flexibles y una experiencia conectada desde la app.",
  keywords: ["gimnasio", "fitness", "clases", "membresías", "entrenamiento"],
  openGraph: {
    title: "Gimnasio Dorian",
    description: "Entrenamiento premium, tecnología y comunidad en una sola experiencia.",
    type: "website",
  },
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="es">
      <body className={`${hanken.variable} ${archivo.variable} ${jetbrains.variable} font-sans`}>
        <SiteHeader />
        {children}
        <SiteFooter />
        <WhatsappFab />
      </body>
    </html>
  );
}

