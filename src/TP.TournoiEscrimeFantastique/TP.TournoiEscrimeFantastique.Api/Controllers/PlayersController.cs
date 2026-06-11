using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP.TournoiEscrimeFantastique.Api.Data;
using TP.TournoiEscrimeFantastique.Api.Data.Entities;
using TP.TournoiEscrimeFantastique.Api.DTOs;
using TP.TournoiEscrimeFantastique.Api.Services;

namespace TP.TournoiEscrimeFantastique.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PlayersController(TournamentDbContext db, DomainPlayerService domainPlayers) : ControllerBase
{
    /// <summary>Récupère tous les joueurs avec leur score calculé.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<PlayerDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var players = await db.Players
            .Include(p => p.Matches.OrderBy(m => m.MatchOrder))
            .AsNoTracking()
            .ToListAsync();

        return Ok(players.Select(domainPlayers.ToDto));
    }

    /// <summary>Récupère un joueur par son identifiant.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<PlayerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var player = await FindPlayerAsync(id);
        return player is null ? NotFound() : Ok(domainPlayers.ToDto(player));
    }

    /// <summary>Crée un nouveau joueur.</summary>
    [HttpPost]
    [ProducesResponseType<PlayerDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreatePlayerDto dto)
    {
        if (dto.PenaltyPoints < 0)
            return BadRequest("Les pénalités ne peuvent pas être négatives.");

        var entity = new PlayerEntity
        {
            Name = dto.Name,
            IsDisqualified = dto.IsDisqualified,
            PenaltyPoints = dto.PenaltyPoints
        };

        db.Players.Add(entity);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, domainPlayers.ToDto(entity));
    }

    /// <summary>Met à jour les informations d'un joueur (nom, disqualification, pénalités).</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<PlayerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePlayerDto dto)
    {
        if (dto.PenaltyPoints < 0)
            return BadRequest("Les pénalités ne peuvent pas être négatives.");

        var player = await FindPlayerAsync(id);
        if (player is null) return NotFound();

        player.Name = dto.Name;
        player.IsDisqualified = dto.IsDisqualified;
        player.PenaltyPoints = dto.PenaltyPoints;

        await db.SaveChangesAsync();
        return Ok(domainPlayers.ToDto(player));
    }

    /// <summary>Supprime un joueur et tous ses combats.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var player = await FindPlayerAsync(id);
        if (player is null) return NotFound();

        db.Players.Remove(player);
        await db.SaveChangesAsync();
        return NoContent();
    }

    /// <summary>Ajoute un résultat de combat à un joueur.</summary>
    /// <remarks>
    /// Valeurs acceptées pour `outcome` : `Win`, `Draw`, `Loss`
    /// </remarks>
    [HttpPost("{id:int}/matches")]
    [ProducesResponseType<PlayerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddMatch(int id, [FromBody] AddMatchDto dto)
    {
        if (!OutcomeMapper.IsValid(dto.Outcome))
            return BadRequest($"Valeur invalide pour outcome. Valeurs acceptées : {OutcomeMapper.ValidValues}");

        var player = await FindPlayerAsync(id);
        if (player is null) return NotFound();

        var nextOrder = player.Matches.Count == 0 ? 1 : player.Matches.Max(m => m.MatchOrder) + 1;
        player.Matches.Add(new MatchEntity
        {
            PlayerId = id,
            Outcome = OutcomeMapper.Normalize(dto.Outcome),
            MatchOrder = nextOrder
        });

        await db.SaveChangesAsync();
        return Ok(domainPlayers.ToDto(player));
    }

    /// <summary>Supprime un combat d'un joueur.</summary>
    [HttpDelete("{id:int}/matches/{matchId:int}")]
    [ProducesResponseType<PlayerDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMatch(int id, int matchId)
    {
        var player = await FindPlayerAsync(id);
        if (player is null) return NotFound();

        var match = player.Matches.FirstOrDefault(m => m.Id == matchId);
        if (match is null) return NotFound();

        player.Matches.Remove(match);
        var ordered = player.Matches.OrderBy(m => m.MatchOrder).ToList();
        for (var i = 0; i < ordered.Count; i++)
            ordered[i].MatchOrder = i + 1;

        await db.SaveChangesAsync();
        return Ok(domainPlayers.ToDto(player));
    }

    private async Task<PlayerEntity?> FindPlayerAsync(int id) =>
        await db.Players
            .Include(p => p.Matches.OrderBy(m => m.MatchOrder))
            .FirstOrDefaultAsync(p => p.Id == id);
}
