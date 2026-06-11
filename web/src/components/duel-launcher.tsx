"use client";

import * as React from "react";
import { Shuffle, Swords } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { DuelArena } from "@/components/duel-arena";
import type { Player } from "@/lib/types";
import { cn } from "@/lib/utils";

function FighterSelect({
  label,
  players,
  value,
  exclude,
  onChange,
}: {
  label: string;
  players: Player[];
  value: number | null;
  exclude: number | null;
  onChange: (id: number) => void;
}) {
  return (
    <label className="flex-1 space-y-1.5">
      <span className="text-xs font-medium text-muted-foreground">{label}</span>
      <select
        value={value ?? ""}
        onChange={(e) => onChange(Number(e.target.value))}
        className="flex h-10 w-full rounded-md border border-input bg-background/60 px-3 text-sm shadow-sm focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
      >
        <option value="" disabled>
          Choisir un combattant…
        </option>
        {players.map((p) => (
          <option key={p.id} value={p.id} disabled={p.id === exclude}>
            {p.name}
            {p.isDisqualified ? " (disqualifié)" : ""}
          </option>
        ))}
      </select>
    </label>
  );
}

export function DuelLauncher({
  players,
  onResolved,
}: {
  players: Player[];
  onResolved: () => void;
}) {
  const [aId, setAId] = React.useState<number | null>(null);
  const [bId, setBId] = React.useState<number | null>(null);
  const [active, setActive] = React.useState<{ a: Player; b: Player } | null>(null);

  const playerA = players.find((p) => p.id === aId) ?? null;
  const playerB = players.find((p) => p.id === bId) ?? null;
  const canFight = playerA && playerB && playerA.id !== playerB.id;

  function randomMatchup() {
    if (players.length < 2) return;
    const pool = [...players];
    const i = Math.floor(Math.random() * pool.length);
    const a = pool.splice(i, 1)[0];
    const j = Math.floor(Math.random() * pool.length);
    const b = pool[j];
    setAId(a.id);
    setBId(b.id);
  }

  if (players.length < 2) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Swords className="size-5 text-gold" /> Lancer un duel
          </CardTitle>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">
            Il faut au moins deux combattants dans l&apos;arène pour organiser un duel.
          </p>
        </CardContent>
      </Card>
    );
  }

  return (
    <>
      <Card className="ring-gold">
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Swords className="size-5 text-gold" /> Lancer un duel
          </CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex flex-col items-stretch gap-3 sm:flex-row sm:items-end">
            <FighterSelect
              label="Combattant A"
              players={players}
              value={aId}
              exclude={bId}
              onChange={setAId}
            />
            <span className="hidden pb-2.5 font-display text-lg font-bold text-muted-foreground sm:block">
              VS
            </span>
            <FighterSelect
              label="Combattant B"
              players={players}
              value={bId}
              exclude={aId}
              onChange={setBId}
            />
          </div>

          <div className="flex flex-wrap gap-2">
            <Button
              variant="gold"
              disabled={!canFight}
              onClick={() => canFight && setActive({ a: playerA!, b: playerB! })}
            >
              <Swords className="size-4" /> Combattre !
            </Button>
            <Button variant="outline" onClick={randomMatchup}>
              <Shuffle className="size-4" /> Tirage aléatoire
            </Button>
          </div>
          <p className={cn("text-xs text-muted-foreground")}>
            L&apos;issue du duel est tirée au sort (45 % / 45 % / 10 % de nul) et ajoutée aux deux
            profils. Les deux combattants sont notifiés.
          </p>
        </CardContent>
      </Card>

      {active && (
        <DuelArena
          playerA={active.a}
          playerB={active.b}
          onResolved={onResolved}
          onClose={() => setActive(null)}
        />
      )}
    </>
  );
}
