import { Badge } from "@/components/ui/badge";
import type { Outcome } from "@/lib/types";

const labels: Record<Outcome, string> = {
  Win: "Victoire",
  Draw: "Nul",
  Loss: "Défaite",
};

const variants = {
  Win: "win",
  Draw: "draw",
  Loss: "loss",
} as const;

export function OutcomeBadge({ outcome }: { outcome: Outcome }) {
  return <Badge variant={variants[outcome]}>{labels[outcome]}</Badge>;
}

export const outcomeLabel = labels;
