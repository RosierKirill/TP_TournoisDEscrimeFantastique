"use client";

import * as React from "react";
import { Loader2, Sparkles, Users } from "lucide-react";
import { CreatePlayerForm } from "@/components/create-player-form";
import { DuelLauncher } from "@/components/duel-launcher";
import { PlayerCard } from "@/components/player-card";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { useToast } from "@/components/ui/toast";
import { api, ApiError } from "@/lib/api";
import type { Outcome, Player } from "@/lib/types";

const demoSeed: { name: string; matches: Outcome[]; penalty?: number; dq?: boolean }[] = [
  { name: "Sir Galahad", matches: ["Win", "Win", "Win", "Draw"] },
  { name: "Dame Morgane", matches: ["Win", "Draw", "Win", "Win", "Win"] },
  { name: "Le Chevalier Noir", matches: ["Win", "Win", "Loss", "Win", "Win", "Win", "Win"], penalty: 4 },
  { name: "Tristan le Félon", matches: ["Win", "Win", "Win"], dq: true },
];

export default function PlayersPage() {
  const { success, error } = useToast();
  const [players, setPlayers] = React.useState<Player[]>([]);
  const [loading, setLoading] = React.useState(true);
  const [failed, setFailed] = React.useState(false);
  const [seeding, setSeeding] = React.useState(false);

  const load = React.useCallback(async () => {
    try {
      const data = await api.getPlayers();
      setPlayers(data);
      setFailed(false);
    } catch (err) {
      setFailed(true);
      error("API injoignable", err instanceof ApiError ? err.message : "Lancez l'API .NET (port 5211).");
    } finally {
      setLoading(false);
    }
  }, [error]);

  React.useEffect(() => {
    load();
  }, [load]);

  async function seedDemo() {
    setSeeding(true);
    try {
      for (const p of demoSeed) {
        const created = await api.createPlayer({ name: p.name, penaltyPoints: p.penalty ?? 0 });
        for (const m of p.matches) await api.addMatch(created.id, m);
        if (p.dq)
          await api.updatePlayer(created.id, {
            name: created.name,
            isDisqualified: true,
            penaltyPoints: p.penalty ?? 0,
          });
      }
      success("Tournoi de démonstration créé", "4 chevaliers ont rejoint l'arène.");
      await load();
    } catch (err) {
      error("Échec du peuplement", err instanceof ApiError ? err.message : "Erreur réseau");
    } finally {
      setSeeding(false);
    }
  }

  return (
    <div className="space-y-8">
      <section className="space-y-2">
        <h1 className="font-display text-3xl font-bold tracking-wide sm:text-4xl">
          L&apos;<span className="text-gold-gradient">Arène</span> des Combattants
        </h1>
        <p className="max-w-2xl text-muted-foreground">
          Enrôlez vos chevaliers, enregistrez leurs duels et laissez le système calculer
          automatiquement leur score selon les règles du tournoi. Bonus de série, pénalités et
          disqualifications sont gérés en temps réel.
        </p>
      </section>

      <CreatePlayerForm onCreated={load} />

      {!loading && !failed && players.length >= 2 && (
        <DuelLauncher players={players} onResolved={load} />
      )}

      <div className="flex items-center justify-between">
        <h2 className="flex items-center gap-2 font-display text-xl font-semibold">
          <Users className="size-5 text-primary" />
          Combattants
          {!loading && <span className="text-base text-muted-foreground">({players.length})</span>}
        </h2>
        {!loading && players.length === 0 && !failed && (
          <Button variant="outline" size="sm" onClick={seedDemo} disabled={seeding}>
            <Sparkles className="size-4" />
            {seeding ? "Création…" : "Peupler une démo"}
          </Button>
        )}
      </div>

      {loading ? (
        <div className="flex items-center justify-center gap-2 py-16 text-muted-foreground">
          <Loader2 className="size-5 animate-spin" /> Chargement de l&apos;arène…
        </div>
      ) : failed ? (
        <Card>
          <CardContent className="py-10 text-center">
            <p className="text-sm text-muted-foreground">
              Impossible de contacter l&apos;API. Démarrez le backend .NET puis réessayez.
            </p>
            <Button variant="outline" className="mt-4" onClick={load}>
              Réessayer
            </Button>
          </CardContent>
        </Card>
      ) : players.length === 0 ? (
        <Card>
          <CardContent className="py-12 text-center">
            <p className="text-muted-foreground">
              L&apos;arène est vide. Enrôlez votre premier chevalier ci-dessus.
            </p>
          </CardContent>
        </Card>
      ) : (
        <div className="grid gap-4 md:grid-cols-2">
          {players.map((p) => (
            <PlayerCard key={p.id} player={p} onChange={load} />
          ))}
        </div>
      )}
    </div>
  );
}
