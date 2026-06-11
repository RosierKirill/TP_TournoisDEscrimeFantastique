using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP.TournoiEscrimeFantastique.Api.Data;
using TP.TournoiEscrimeFantastique.Api.DTOs;
using DomainMatchResult = TP.TournoiEscrimeFantastique.MatchResult;
using IFightScoreCalculator = TP.TournoiEscrimeFantastique.IScoreCalculator;

namespace TP.TournoiEscrimeFantastique.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RankingController(TournamentDbContext db, IFightScoreCalculator scoreCalculator) : ControllerBase
{
    /// <summary>Retourne le classement de tous les joueurs par score décroissant.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<RankingEntryDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRanking()
    {
        var players = await db.Players
            .Include(p => p.Matches.OrderBy(m => m.MatchOrder))
            .AsNoTracking()
            .ToListAsync();

        var ranked = players
            .Select(p => new
            {
                Player = p,
                Score = scoreCalculator.CalculateScore(
                    p.Matches.OrderBy(m => m.MatchOrder)
                             .Select(m => OutcomeMapper.ToMatchResult(m.Outcome))
                             .ToList<DomainMatchResult>(),
                    p.IsDisqualified,
                    p.PenaltyPoints)
            })
            .OrderByDescending(x => x.Score)
            .Select((x, i) => new RankingEntryDto(i + 1, x.Player.Id, x.Player.Name, x.Score, x.Player.IsDisqualified));

        return Ok(ranked);
    }

    /// <summary>Retourne le champion du tournoi (joueur avec le meilleur score &gt; 0).</summary>
    [HttpGet("champion")]
    [ProducesResponseType<RankingEntryDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChampion()
    {
        var players = await db.Players
            .Include(p => p.Matches.OrderBy(m => m.MatchOrder))
            .AsNoTracking()
            .ToListAsync();

        var ranked = players
            .Select(p => new
            {
                Player = p,
                Score = scoreCalculator.CalculateScore(
                    p.Matches.OrderBy(m => m.MatchOrder)
                             .Select(m => OutcomeMapper.ToMatchResult(m.Outcome))
                             .ToList<DomainMatchResult>(),
                    p.IsDisqualified,
                    p.PenaltyPoints)
            })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (ranked is null || ranked.Score == 0)
            return NotFound("Aucun champion : tous les joueurs ont un score nul ou la liste est vide.");

        return Ok(new RankingEntryDto(1, ranked.Player.Id, ranked.Player.Name, ranked.Score, ranked.Player.IsDisqualified));
    }
}
