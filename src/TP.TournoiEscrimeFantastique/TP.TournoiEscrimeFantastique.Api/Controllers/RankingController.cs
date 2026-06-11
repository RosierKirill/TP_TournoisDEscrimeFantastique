using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP.TournoiEscrimeFantastique.Api.Data;
using TP.TournoiEscrimeFantastique.Api.DTOs;
using TP.TournoiEscrimeFantastique.Api.Services;

namespace TP.TournoiEscrimeFantastique.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class RankingController(TournamentDbContext db, DomainPlayerService domainPlayers) : ControllerBase
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

        return Ok(domainPlayers.GetRankingDtos(players));
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

        var champion = domainPlayers.GetChampionDto(players);
        if (champion is null)
            return NotFound("Aucun champion : tous les joueurs ont un score nul ou la liste est vide.");

        return Ok(champion);
    }
}
