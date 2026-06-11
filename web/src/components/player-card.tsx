"use client";

import * as React from "react";
import {
  Ban,
  Bell,
  Check,
  Pencil,
  Shield,
  Swords,
  Trash2,
  X,
} from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader } from "@/components/ui/card";
import { useToast } from "@/components/ui/toast";
import { OutcomeBadge } from "@/components/outcome-badge";
import { api, ApiError } from "@/lib/api";
import type { Outcome, Player } from "@/lib/types";
import { cn } from "@/lib/utils";

const outcomeButtons: { outcome: Outcome; label: string; className: string }[] = [
  { outcome: "Win", label: "Victoire", className: "border-emerald-500/40 text-emerald-300 hover:bg-emerald-500/15" },
  { outcome: "Draw", label: "Nul", className: "border-amber-500/40 text-amber-300 hover:bg-amber-500/15" },
  { outcome: "Loss", label: "Défaite", className: "border-rose-500/40 text-rose-300 hover:bg-rose-500/15" },
];

export function PlayerCard({
  player,
  onChange,
}: {
  player: Player;
  onChange: () => void;
}) {
  const { success, error } = useToast();
  const [busy, setBusy] = React.useState(false);
  const [editing, setEditing] = React.useState(false);
  const [draftName, setDraftName] = React.useState(player.name);
  const [draftPenalty, setDraftPenalty] = React.useState(player.penaltyPoints);

  async function run(action: () => Promise<unknown>, okMsg?: string) {
    setBusy(true);
    try {
      await action();
      if (okMsg) success(okMsg);
      onChange();
    } catch (err) {
      error("Action impossible", err instanceof ApiError ? err.message : "Erreur réseau");
    } finally {
      setBusy(false);
    }
  }

  const wins = player.matches.filter((m) => m.outcome === "Win").length;

  return (
    <Card className={cn("transition-shadow", player.isDisqualified && "opacity-70")}>
      <CardHeader className="flex-row items-start justify-between gap-3 space-y-0">
        {editing ? (
          <div className="flex flex-1 flex-col gap-2">
            <Input value={draftName} onChange={(e) => setDraftName(e.target.value)} maxLength={100} />
            <div className="flex items-center gap-2">
              <span className="text-xs text-muted-foreground">Pénalités</span>
              <Input
                type="number"
                min={0}
                className="h-8 w-24"
                value={draftPenalty}
                onChange={(e) => setDraftPenalty(Math.max(0, Number(e.target.value) || 0))}
              />
            </div>
          </div>
        ) : (
          <div className="min-w-0 flex-1">
            <div className="flex items-center gap-2">
              <Shield className="size-4 shrink-0 text-primary" />
              <h3 className="truncate font-display text-lg font-semibold">{player.name}</h3>
            </div>
            <div className="mt-1.5 flex flex-wrap items-center gap-1.5">
              <Badge variant="muted">{player.matches.length} duel{player.matches.length > 1 ? "s" : ""}</Badge>
              <Badge variant="win">{wins} victoire{wins > 1 ? "s" : ""}</Badge>
              {player.penaltyPoints > 0 && (
                <Badge variant="destructive">−{player.penaltyPoints} pénalité</Badge>
              )}
              {player.isDisqualified && <Badge variant="destructive">Disqualifié</Badge>}
            </div>
          </div>
        )}

        <div className="flex shrink-0 flex-col items-end">
          <span
            className={cn(
              "font-display text-3xl font-bold leading-none",
              player.isDisqualified ? "text-muted-foreground line-through" : "text-gold-gradient"
            )}
          >
            {player.score}
          </span>
          <span className="text-[10px] uppercase tracking-widest text-muted-foreground">points</span>
        </div>
      </CardHeader>

      <CardContent className="space-y-4">
        {/* Timeline des combats */}
        {player.matches.length > 0 ? (
          <div className="flex flex-wrap gap-1.5">
            {player.matches.map((m) => (
              <button
                key={m.id}
                disabled={busy}
                onClick={() => run(() => api.deleteMatch(player.id, m.id))}
                title="Cliquer pour retirer ce combat"
                className="group relative"
              >
                <OutcomeBadge outcome={m.outcome} />
                <X className="absolute -right-1 -top-1 size-3 rounded-full bg-destructive p-px text-white opacity-0 transition group-hover:opacity-100" />
              </button>
            ))}
          </div>
        ) : (
          <p className="text-sm italic text-muted-foreground">Aucun duel disputé pour l&apos;instant.</p>
        )}

        {/* Ajouter un combat */}
        <div className="flex flex-wrap items-center gap-2">
          <span className="flex items-center gap-1 text-xs font-medium text-muted-foreground">
            <Swords className="size-3.5" /> Ajouter :
          </span>
          {outcomeButtons.map((b) => (
            <Button
              key={b.outcome}
              size="sm"
              variant="outline"
              disabled={busy}
              className={b.className}
              onClick={() => run(() => api.addMatch(player.id, b.outcome))}
            >
              {b.label}
            </Button>
          ))}
        </div>

        {/* Actions */}
        <div className="flex flex-wrap items-center gap-2 border-t border-border/60 pt-3">
          {editing ? (
            <>
              <Button
                size="sm"
                variant="gold"
                disabled={busy}
                onClick={() =>
                  run(
                    () =>
                      api.updatePlayer(player.id, {
                        name: draftName.trim() || player.name,
                        isDisqualified: player.isDisqualified,
                        penaltyPoints: draftPenalty,
                      }),
                    "Joueur mis à jour"
                  ).then(() => setEditing(false))
                }
              >
                <Check className="size-4" /> Enregistrer
              </Button>
              <Button size="sm" variant="ghost" onClick={() => setEditing(false)}>
                <X className="size-4" /> Annuler
              </Button>
            </>
          ) : (
            <>
              <Button
                size="sm"
                variant="outline"
                onClick={() => {
                  setDraftName(player.name);
                  setDraftPenalty(player.penaltyPoints);
                  setEditing(true);
                }}
              >
                <Pencil className="size-4" /> Modifier
              </Button>
              <Button
                size="sm"
                variant="outline"
                disabled={busy}
                onClick={() =>
                  run(
                    () =>
                      api.updatePlayer(player.id, {
                        name: player.name,
                        isDisqualified: !player.isDisqualified,
                        penaltyPoints: player.penaltyPoints,
                      }),
                    player.isDisqualified ? "Joueur réintégré" : "Joueur disqualifié"
                  )
                }
              >
                {player.isDisqualified ? <Check className="size-4" /> : <Ban className="size-4" />}
                {player.isDisqualified ? "Réintégrer" : "Disqualifier"}
              </Button>
              <Button
                size="sm"
                variant="outline"
                disabled={busy}
                onClick={() => {
                  const msg = window.prompt(`Message à envoyer à ${player.name} :`, "Votre prochain duel approche !");
                  if (msg) run(() => api.notifyPlayer(player.id, msg), "Notification envoyée");
                }}
              >
                <Bell className="size-4" /> Notifier
              </Button>
              <Button
                size="sm"
                variant="destructive"
                className="ml-auto"
                disabled={busy}
                onClick={() => {
                  if (window.confirm(`Supprimer ${player.name} et tous ses duels ?`))
                    run(() => api.deletePlayer(player.id), "Joueur supprimé");
                }}
              >
                <Trash2 className="size-4" />
              </Button>
            </>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
