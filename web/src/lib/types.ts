export type Outcome = "Win" | "Draw" | "Loss";

export interface Match {
  id: number;
  outcome: Outcome;
  matchOrder: number;
}

export interface Player {
  id: number;
  name: string;
  isDisqualified: boolean;
  penaltyPoints: number;
  matches: Match[];
  score: number;
}

export interface RankingEntry {
  rank: number;
  playerId: number;
  name: string;
  score: number;
  isDisqualified: boolean;
}

export interface Notification {
  playerName: string;
  message: string;
  sentAt: string;
}

export interface CreatePlayerInput {
  name: string;
  isDisqualified?: boolean;
  penaltyPoints?: number;
}

export interface UpdatePlayerInput {
  name: string;
  isDisqualified: boolean;
  penaltyPoints: number;
}

export interface DuelCombatant {
  playerId: number;
  name: string;
  outcome: Outcome;
  score: number;
}

export interface DuelResult {
  playerA: DuelCombatant;
  playerB: DuelCombatant;
  winner: "A" | "B" | "Draw";
}
