import { Crown, Medal } from "lucide-react";
import type { RankingEntry } from "@/lib/types";
import { cn } from "@/lib/utils";

const podiumStyles = [
  { height: "h-32", ring: "ring-gold", glow: "card-glow", accent: "text-gold", order: "order-2" },
  { height: "h-24", ring: "", glow: "", accent: "text-slate-300", order: "order-1" },
  { height: "h-20", ring: "", glow: "", accent: "text-amber-600", order: "order-3" },
];

export function Podium({ top }: { top: RankingEntry[] }) {
  if (top.length === 0) return null;

  return (
    <div className="flex items-end justify-center gap-3 sm:gap-6">
      {top.slice(0, 3).map((entry, i) => {
        const style = podiumStyles[i];
        const isFirst = i === 0;
        return (
          <div key={entry.playerId} className={cn("flex w-24 flex-col items-center sm:w-32", style.order)}>
            <div className="mb-2 flex flex-col items-center text-center">
              {isFirst ? (
                <Crown className="mb-1 size-7 text-gold drop-shadow" />
              ) : (
                <Medal className={cn("mb-1 size-5", style.accent)} />
              )}
              <p className="line-clamp-1 max-w-full text-sm font-medium">{entry.name}</p>
              <p className={cn("font-display text-xl font-bold", isFirst ? "text-gold-gradient" : style.accent)}>
                {entry.score}
              </p>
            </div>
            <div
              className={cn(
                "flex w-full items-start justify-center rounded-t-lg border border-border bg-secondary/70 pt-2 font-display text-2xl font-bold text-muted-foreground",
                style.height,
                style.ring,
                style.glow
              )}
            >
              {entry.rank}
            </div>
          </div>
        );
      })}
    </div>
  );
}
