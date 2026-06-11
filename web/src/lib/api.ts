import type {
  CreatePlayerInput,
  DuelResult,
  Notification,
  Outcome,
  Player,
  RankingEntry,
  UpdatePlayerInput,
} from "./types";

const BASE_URL =
  process.env.NEXT_PUBLIC_API_URL?.replace(/\/$/, "") ?? "http://localhost:5211";

class ApiError extends Error {
  constructor(public status: number, message: string) {
    super(message);
    this.name = "ApiError";
  }
}

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json", ...(init?.headers ?? {}) },
    cache: "no-store",
    ...init,
  });

  if (!res.ok) {
    let message = `Erreur ${res.status}`;
    try {
      const text = await res.text();
      if (text) message = text;
    } catch {
      /* ignore */
    }
    throw new ApiError(res.status, message);
  }

  if (res.status === 204) return undefined as T;
  const contentType = res.headers.get("content-type") ?? "";
  if (!contentType.includes("application/json")) return undefined as T;
  return res.json() as Promise<T>;
}

export const api = {
  // Joueurs
  getPlayers: () => request<Player[]>("/api/players"),
  getPlayer: (id: number) => request<Player>(`/api/players/${id}`),
  createPlayer: (input: CreatePlayerInput) =>
    request<Player>("/api/players", {
      method: "POST",
      body: JSON.stringify(input),
    }),
  updatePlayer: (id: number, input: UpdatePlayerInput) =>
    request<Player>(`/api/players/${id}`, {
      method: "PUT",
      body: JSON.stringify(input),
    }),
  deletePlayer: (id: number) =>
    request<void>(`/api/players/${id}`, { method: "DELETE" }),

  // Combats
  addMatch: (playerId: number, outcome: Outcome) =>
    request<Player>(`/api/players/${playerId}/matches`, {
      method: "POST",
      body: JSON.stringify({ outcome }),
    }),
  deleteMatch: (playerId: number, matchId: number) =>
    request<Player>(`/api/players/${playerId}/matches/${matchId}`, {
      method: "DELETE",
    }),

  // Duel
  startDuel: (playerAId: number, playerBId: number) =>
    request<DuelResult>("/api/duels", {
      method: "POST",
      body: JSON.stringify({ playerAId, playerBId }),
    }),

  // Classement
  getRanking: () => request<RankingEntry[]>("/api/ranking"),
  getChampion: () => request<RankingEntry>("/api/ranking/champion"),

  // Notifications
  getNotifications: () => request<Notification[]>("/api/notifications"),
  notifyPlayer: (playerId: number, message: string) =>
    request<Notification>(`/api/notifications/players/${playerId}`, {
      method: "POST",
      body: JSON.stringify({ message }),
    }),
  broadcastRanking: () =>
    request<Notification[]>("/api/notifications/broadcast-ranking", {
      method: "POST",
    }),
};

export { ApiError };
