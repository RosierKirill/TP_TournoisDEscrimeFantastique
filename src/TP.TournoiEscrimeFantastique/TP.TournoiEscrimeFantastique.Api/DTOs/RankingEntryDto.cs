namespace TP.TournoiEscrimeFantastique.Api.DTOs;

public record RankingEntryDto(int Rank, int PlayerId, string Name, int Score, bool IsDisqualified);
