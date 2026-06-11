using TP.TournoiEscrimeFantastique;
using TP.TournoiEscrimeFantastique.Api.Data;
using TP.TournoiEscrimeFantastique.Api.Data.Entities;
using TP.TournoiEscrimeFantastique.Api.DTOs;

namespace TP.TournoiEscrimeFantastique.Api.Services;

/// <summary>
/// Pont entre les entités persistées et le modèle domaine (Player, TournamentRanking).
/// </summary>
public class DomainPlayerService(IScoreCalculator scoreCalculator, TournamentRanking tournamentRanking)
{
    public int GetScore(PlayerEntity entity)
    {
        var domain = PlayerEntityMapper.ToDomain(entity);
        return scoreCalculator.CalculateScore(domain.Matches, domain.IsDisqualified, domain.PenaltyPoints);
    }

    public PlayerDto ToDto(PlayerEntity entity) =>
        new(
            entity.Id,
            entity.Name,
            entity.IsDisqualified,
            entity.PenaltyPoints,
            entity.Matches
                .OrderBy(m => m.MatchOrder)
                .Select(m => new MatchDto(m.Id, m.Outcome, m.MatchOrder))
                .ToList(),
            GetScore(entity));

    public List<RankingEntryDto> GetRankingDtos(IReadOnlyList<PlayerEntity> entities)
    {
        var pairs = entities.Select(e => (Entity: e, Domain: PlayerEntityMapper.ToDomain(e))).ToList();
        var entityByDomain = pairs.ToDictionary(p => p.Domain, p => p.Entity);
        var ranked = tournamentRanking.GetRanking(pairs.Select(p => p.Domain).ToList());

        return ranked.Select((domain, index) =>
        {
            var entity = entityByDomain[domain];
            return new RankingEntryDto(
                index + 1,
                entity.Id,
                entity.Name,
                scoreCalculator.CalculateScore(domain.Matches, domain.IsDisqualified, domain.PenaltyPoints),
                entity.IsDisqualified);
        }).ToList();
    }

    public RankingEntryDto? GetChampionDto(IReadOnlyList<PlayerEntity> entities)
    {
        var pairs = entities.Select(e => (Entity: e, Domain: PlayerEntityMapper.ToDomain(e))).ToList();
        var champion = tournamentRanking.GetChampion(pairs.Select(p => p.Domain).ToList());
        if (champion is null) return null;

        var entity = pairs.First(p => ReferenceEquals(p.Domain, champion)).Entity;
        var score = scoreCalculator.CalculateScore(champion.Matches, champion.IsDisqualified, champion.PenaltyPoints);
        return new RankingEntryDto(1, entity.Id, entity.Name, score, entity.IsDisqualified);
    }
}
