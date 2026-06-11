"use client";

import * as React from "react";
import { Bell, Crown, Loader2, RefreshCw, Trophy } from "lucide-react";
import { Podium } from "@/components/podium";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useToast } from "@/components/ui/toast";
import { api, ApiError } from "@/lib/api";
import type { Notification, RankingEntry } from "@/lib/types";
import { cn } from "@/lib/utils";

export default function RankingPage() {
  const { success, error } = useToast();
  const [ranking, setRanking] = React.useState<RankingEntry[]>([]);
  const [champion, setChampion] = React.useState<RankingEntry | null>(null);
  const [notifications, setNotifications] = React.useState<Notification[]>([]);
  const [loading, setLoading] = React.useState(true);
  const [busy, setBusy] = React.useState(false);

  const load = React.useCallback(async () => {
    try {
      const [r, n] = await Promise.all([api.getRanking(), api.getNotifications()]);
      setRanking(r);
      setNotifications(n);
      try {
        setChampion(await api.getChampion());
      } catch {
        setChampion(null);
      }
    } catch (err) {
      error("API injoignable", err instanceof ApiError ? err.message : "Lancez l'API .NET.");
    } finally {
      setLoading(false);
    }
  }, [error]);

  React.useEffect(() => {
    load();
  }, [load]);

  async function broadcast() {
    setBusy(true);
    try {
      const sent = await api.broadcastRanking();
      success("Participants notifiés", `${sent.length} message(s) envoyé(s).`);
      await load();
    } catch (err) {
      error("Échec de l'envoi", err instanceof ApiError ? err.message : "Erreur réseau");
    } finally {
      setBusy(false);
    }
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center gap-2 py-20 text-muted-foreground">
        <Loader2 className="size-5 animate-spin" /> Calcul du classement…
      </div>
    );
  }

  return (
    <div className="space-y-8">
      <section className="flex flex-wrap items-end justify-between gap-4">
        <div className="space-y-2">
          <h1 className="font-display text-3xl font-bold tracking-wide sm:text-4xl">
            <span className="text-gold-gradient">Classement</span> du Tournoi
          </h1>
          <p className="text-muted-foreground">Les héros classés par score décroissant.</p>
        </div>
        <div className="flex gap-2">
          <Button variant="outline" onClick={load} disabled={busy}>
            <RefreshCw className="size-4" /> Actualiser
          </Button>
          <Button variant="gold" onClick={broadcast} disabled={busy || ranking.length === 0}>
            <Bell className="size-4" /> Notifier tous
          </Button>
        </div>
      </section>

      {/* Champion */}
      {champion && (
        <Card className="ring-gold">
          <CardContent className="flex items-center gap-4 py-6">
            <span className="flex size-14 items-center justify-center rounded-full bg-gold/15 ring-gold">
              <Crown className="size-7 text-gold" />
            </span>
            <div className="flex-1">
              <p className="text-xs uppercase tracking-[0.2em] text-muted-foreground">Champion du tournoi</p>
              <p className="font-display text-2xl font-bold text-gold-gradient">{champion.name}</p>
            </div>
            <div className="text-right">
              <p className="font-display text-3xl font-bold text-gold-gradient">{champion.score}</p>
              <p className="text-[10px] uppercase tracking-widest text-muted-foreground">points</p>
            </div>
          </CardContent>
        </Card>
      )}

      {ranking.length === 0 ? (
        <Card>
          <CardContent className="py-12 text-center text-muted-foreground">
            Aucun joueur dans le tournoi. Enrôlez des combattants pour établir un classement.
          </CardContent>
        </Card>
      ) : (
        <>
          {/* Podium */}
          {ranking.length >= 2 && (
            <div className="py-4">
              <Podium top={ranking} />
            </div>
          )}

          {/* Tableau complet */}
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <Trophy className="size-5 text-gold" /> Classement complet
              </CardTitle>
            </CardHeader>
            <CardContent className="p-0">
              <ul className="divide-y divide-border/60">
                {ranking.map((entry) => (
                  <li
                    key={entry.playerId}
                    className={cn(
                      "flex items-center gap-4 px-6 py-3 transition-colors hover:bg-secondary/40",
                      entry.rank === 1 && entry.score > 0 && "bg-gold/5"
                    )}
                  >
                    <span
                      className={cn(
                        "flex size-8 shrink-0 items-center justify-center rounded-full font-display text-sm font-bold",
                        entry.rank === 1 && entry.score > 0
                          ? "bg-gold/20 text-gold ring-gold"
                          : "bg-secondary text-muted-foreground"
                      )}
                    >
                      {entry.rank}
                    </span>
                    <span className="flex-1 truncate font-medium">{entry.name}</span>
                    {entry.isDisqualified && <Badge variant="destructive">Disqualifié</Badge>}
                    <span
                      className={cn(
                        "font-display text-xl font-bold",
                        entry.isDisqualified ? "text-muted-foreground" : "text-foreground"
                      )}
                    >
                      {entry.score}
                    </span>
                  </li>
                ))}
              </ul>
            </CardContent>
          </Card>
        </>
      )}

      {/* Notifications */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Bell className="size-5 text-primary" /> Notifications envoyées
          </CardTitle>
        </CardHeader>
        <CardContent>
          {notifications.length === 0 ? (
            <p className="text-sm italic text-muted-foreground">
              Aucune notification pour l&apos;instant. Utilisez « Notifier tous » pour prévenir les
              participants de leur classement.
            </p>
          ) : (
            <ul className="space-y-2">
              {notifications.map((n, i) => (
                <li key={i} className="flex items-start gap-3 rounded-md bg-secondary/40 p-3 text-sm">
                  <Badge variant="default" className="shrink-0">{n.playerName}</Badge>
                  <span className="flex-1 text-muted-foreground">{n.message}</span>
                  <span className="shrink-0 text-xs text-muted-foreground/70">
                    {new Date(n.sentAt).toLocaleTimeString("fr-FR")}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
