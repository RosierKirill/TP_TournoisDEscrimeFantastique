import type { Metadata } from "next";
import { Cinzel, Inter } from "next/font/google";
import "./globals.css";
import { ToastProvider } from "@/components/ui/toast";
import { SiteHeader } from "@/components/site-header";

const cinzel = Cinzel({
  subsets: ["latin"],
  weight: ["500", "600", "700", "800"],
  variable: "--font-cinzel",
});

const inter = Inter({
  subsets: ["latin"],
  variable: "--font-inter",
});

export const metadata: Metadata = {
  title: "Tournoi d'Escrime Fantastique",
  description:
    "Système de notation pour un tournoi d'escrime fantastique — gérez les chevaliers, leurs duels et couronnez le champion.",
};

export default function RootLayout({
  children,
}: Readonly<{ children: React.ReactNode }>) {
  return (
    <html lang="fr" className={`${cinzel.variable} ${inter.variable}`}>
      <body className="font-sans">
        <ToastProvider>
          <SiteHeader />
          <main className="container py-8">{children}</main>
          <footer className="border-t border-border/60 py-6 text-center text-xs text-muted-foreground">
            Arène magique · Système de notation d&apos;escrime fantastique
          </footer>
        </ToastProvider>
      </body>
    </html>
  );
}
