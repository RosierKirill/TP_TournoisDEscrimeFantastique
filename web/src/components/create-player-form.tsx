"use client";

import * as React from "react";
import { UserPlus } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { useToast } from "@/components/ui/toast";
import { api, ApiError } from "@/lib/api";

export function CreatePlayerForm({ onCreated }: { onCreated: () => void }) {
  const { success, error } = useToast();
  const [name, setName] = React.useState("");
  const [penalty, setPenalty] = React.useState(0);
  const [submitting, setSubmitting] = React.useState(false);

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    if (!name.trim()) {
      error("Nom requis", "Donnez un nom à votre chevalier.");
      return;
    }
    setSubmitting(true);
    try {
      await api.createPlayer({ name: name.trim(), penaltyPoints: penalty });
      success("Chevalier enrôlé", `${name.trim()} entre dans l'arène.`);
      setName("");
      setPenalty(0);
      onCreated();
    } catch (err) {
      error("Échec de l'enrôlement", err instanceof ApiError ? err.message : "Erreur réseau");
    } finally {
      setSubmitting(false);
    }
  }

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <UserPlus className="size-5 text-gold" />
          Enrôler un combattant
        </CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={handleSubmit} className="flex flex-col gap-3 sm:flex-row sm:items-end">
          <div className="flex-1 space-y-1.5">
            <label className="text-xs font-medium text-muted-foreground">Nom du chevalier</label>
            <Input
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Sir Galahad…"
              maxLength={100}
            />
          </div>
          <div className="w-full space-y-1.5 sm:w-32">
            <label className="text-xs font-medium text-muted-foreground">Pénalités</label>
            <Input
              type="number"
              min={0}
              value={penalty}
              onChange={(e) => setPenalty(Math.max(0, Number(e.target.value) || 0))}
            />
          </div>
          <Button type="submit" variant="gold" disabled={submitting}>
            <UserPlus className="size-4" />
            {submitting ? "Enrôlement…" : "Enrôler"}
          </Button>
        </form>
      </CardContent>
    </Card>
  );
}
