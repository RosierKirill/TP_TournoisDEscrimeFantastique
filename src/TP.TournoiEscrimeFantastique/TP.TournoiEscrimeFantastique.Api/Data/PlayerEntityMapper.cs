using TP.TournoiEscrimeFantastique;
using TP.TournoiEscrimeFantastique.Api.Data.Entities;

namespace TP.TournoiEscrimeFantastique.Api.Data;

internal static class PlayerEntityMapper
{
    public static Player ToDomain(PlayerEntity entity) =>
        new(
            entity.Name,
            entity.Matches
                .OrderBy(m => m.MatchOrder)
                .Select(m => OutcomeMapper.ToMatchResult(m.Outcome))
                .ToList(),
            entity.IsDisqualified,
            entity.PenaltyPoints);
}
