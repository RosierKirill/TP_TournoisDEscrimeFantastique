namespace TP.TournoiEscrimeFantastique.Api.DTOs;

public record PlayerDto(
    int Id,
    string Name,
    bool IsDisqualified,
    int PenaltyPoints,
    IReadOnlyList<MatchDto> Matches,
    int Score
);

public record MatchDto(int Id, string Outcome, int MatchOrder);
