"use client";

import * as React from "react";
import { createPortal } from "react-dom";
import { Crown, Shield, Swords, X, Zap } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { useToast } from "@/components/ui/toast";
import { api, ApiError } from "@/lib/api";
import type { DuelResult, Outcome, Player } from "@/lib/types";
import { cn } from "@/lib/utils";

type Phase = "fighting" | "result";

const FIGHT_MS = 2300;

const outcomeText: Record<Outcome, string> = {
  Win: "Victoire",
  Draw: "Match nul",
  Loss: "Défaite",
};

function Fighter({
  name,
  side,
  fighting,
  result,
}: {
  name: string;
  side: "left" | "right";
  fighting: boolean;
  result?: { outcome: Outcome; score: number; winner: boolean };
}) {
  return (
    <div className="flex flex-1 flex-col items-center gap-3 text-center">
      <div
        className={cn(
          "relative flex size-24 items-center justify-center rounded-full border-2 transition-colors sm:size-28",
          result?.winner
            ? "border-gold ring-gold bg-gold/10"
            : result && result.outcome === "Loss"
            ? "border-rose-500/50 bg-rose-500/5 opacity-70"
            : "border-border bg-secondary/60"
        )}
      >
        {result?.winner && (
          <Crown className="absolute -top-5 size-6 text-gold animate-pop-in" />
        )}
        <Swords
          className={cn(
            "size-10 text-primary",
            fighting && (side === "left" ? "animate-lunge-left" : "animate-lunge-right")
          )}
        />
      </div>
      <p className="font-display text-base font-semibold sm:text-lg">{name}</p>
      {result && (
        <div className="animate-pop-in">
          <Badge variant={result.outcome === "Win" ? "win" : result.outcome === "Draw" ? "draw" : "loss"}>
            {outcomeText[result.outcome]}
          </Badge>
          <p className="mt-1.5 font-display text-2xl font-bold text-gold-gradient">{result.score}</p>
          <p className="text-[10px] uppercase tracking-widest text-muted-foreground">points</p>
        </div>
      )}
    </div>
  );
}

export function DuelArena({
  playerA,
  playerB,
  onClose,
  onResolved,
}: {
  playerA: Player;
  playerB: Player;
  onClose: () => void;
  onResolved: () => void;
}) {
  const { error } = useToast();
  const [phase, setPhase] = React.useState<Phase>("fighting");
  const [result, setResult] = React.useState<DuelResult | null>(null);
  const startedRef = React.useRef(false);

  React.useEffect(() => {
    if (startedRef.current) return;
    startedRef.current = true;

    const startedAt = Date.now();
    api
      .startDuel(playerA.id, playerB.id)
      .then((res) => {
        // On laisse l'animation se jouer au moins FIGHT_MS pour le suspense.
        const elapsed = Date.now() - startedAt;
        const wait = Math.max(0, FIGHT_MS - elapsed);
        setTimeout(() => {
          setResult(res);
          setPhase("result");
          onResolved();
        }, wait);
      })
      .catch((err) => {
        error("Duel impossible", err instanceof ApiError ? err.message : "Erreur réseau");
        onClose();
      });
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // La modale est rendue via un portail sur <body> : ainsi son
  // positionnement `fixed` se base sur la fenêtre du navigateur et non
  // sur un ancêtre transformé (ex. <main> animé).
  const [mounted, setMounted] = React.useState(false);
  React.useEffect(() => setMounted(true), []);

  // Empêche le défilement de l'arrière-plan pendant le duel.
  React.useEffect(() => {
    const prev = document.body.style.overflow;
    document.body.style.overflow = "hidden";
    return () => {
      document.body.style.overflow = prev;
    };
  }, []);

  const fighting = phase === "fighting";

  if (!mounted) return null;

  return createPortal(
    <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
      <div
        className="absolute inset-0 bg-background/80 backdrop-blur-sm"
        onClick={phase === "result" ? onClose : undefined}
      />

      <div
        className={cn(
          "relative w-full max-w-xl overflow-hidden rounded-2xl border border-border bg-card p-6 shadow-2xl ring-gold sm:p-8",
          fighting && "animate-shake-x"
        )}
      >
        {phase === "result" && (
          <button
            onClick={onClose}
            className="absolute right-4 top-4 text-muted-foreground hover:text-foreground"
          >
            <X className="size-5" />
          </button>
        )}

        <div className="mb-6 text-center">
          <p className="flex items-center justify-center gap-2 font-display text-xl font-bold tracking-wide text-gold-gradient">
            <Swords className="size-5 text-gold" />
            {fighting ? "Duel en cours…" : "Résultat du duel"}
          </p>
          {fighting && (
            <p className="mt-1 text-sm text-muted-foreground">
              Les lames s&apos;entrechoquent dans l&apos;arène magique…
            </p>
          )}
        </div>

        <div className="relative flex items-center justify-between gap-2">
          <Fighter
            name={playerA.name}
            side="left"
            fighting={fighting}
            result={
              result
                ? {
                    outcome: result.playerA.outcome,
                    score: result.playerA.score,
                    winner: result.winner === "A",
                  }
                : undefined
            }
          />

          {/* Centre : VS + étincelles de choc */}
          <div className="relative flex w-16 shrink-0 flex-col items-center">
            {fighting && (
              <Zap className="absolute left-1/2 top-1/2 size-12 -translate-x-1/2 -translate-y-1/2 text-gold animate-clash" />
            )}
            <span className="font-display text-2xl font-bold text-muted-foreground">VS</span>
          </div>

          <Fighter
            name={playerB.name}
            side="right"
            fighting={fighting}
            result={
              result
                ? {
                    outcome: result.playerB.outcome,
                    score: result.playerB.score,
                    winner: result.winner === "B",
                  }
                : undefined
            }
          />
        </div>

        {phase === "result" && result && (
          <div className="mt-7 text-center animate-fade-up">
            <p className="font-display text-lg font-semibold">
              {result.winner === "Draw" ? (
                <span className="flex items-center justify-center gap-2 text-amber-300">
                  <Shield className="size-5" /> Match nul ! Les deux héros se valent.
                </span>
              ) : (
                <span className="flex items-center justify-center gap-2 text-gold">
                  <Crown className="size-5" />
                  {(result.winner === "A" ? result.playerA : result.playerB).name} l&apos;emporte !
                </span>
              )}
            </p>
            <Button variant="gold" className="mt-5" onClick={onClose}>
              Retour à l&apos;arène
            </Button>
          </div>
        )}
      </div>
    </div>,
    document.body
  );
}
