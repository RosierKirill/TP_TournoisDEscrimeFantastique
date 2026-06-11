"use client";

import Link from "next/link";
import { usePathname } from "next/navigation";
import { Swords, Trophy, Users } from "lucide-react";
import { cn } from "@/lib/utils";

const links = [
  { href: "/", label: "Joueurs", icon: Users },
  { href: "/ranking", label: "Classement", icon: Trophy },
];

export function SiteHeader() {
  const pathname = usePathname();

  return (
    <header className="sticky top-0 z-40 border-b border-border/60 bg-background/70 backdrop-blur-lg">
      <div className="container flex h-16 items-center justify-between">
        <Link href="/" className="group flex items-center gap-2.5">
          <span className="flex size-9 items-center justify-center rounded-lg bg-gold/15 ring-gold transition group-hover:scale-105">
            <Swords className="size-5 text-gold" />
          </span>
          <div className="leading-tight">
            <p className="font-display text-base font-bold tracking-wide text-gold-gradient">
              Tournoi d&apos;Escrime
            </p>
            <p className="text-[10px] uppercase tracking-[0.2em] text-muted-foreground">
              Arène Fantastique
            </p>
          </div>
        </Link>

        <nav className="flex items-center gap-1">
          {links.map(({ href, label, icon: Icon }) => {
            const active = pathname === href;
            return (
              <Link
                key={href}
                href={href}
                className={cn(
                  "flex items-center gap-2 rounded-md px-3 py-2 text-sm font-medium transition-colors",
                  active
                    ? "bg-secondary text-foreground"
                    : "text-muted-foreground hover:bg-secondary/60 hover:text-foreground"
                )}
              >
                <Icon className="size-4" />
                <span className="hidden sm:inline">{label}</span>
              </Link>
            );
          })}
        </nav>
      </div>
    </header>
  );
}
