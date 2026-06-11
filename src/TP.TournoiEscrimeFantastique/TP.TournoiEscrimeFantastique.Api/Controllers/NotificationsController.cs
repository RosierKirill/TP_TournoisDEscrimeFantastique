using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP.TournoiEscrimeFantastique.Api.Data;
using TP.TournoiEscrimeFantastique.Api.DTOs;
using TP.TournoiEscrimeFantastique.Api.Notifications;
using TP.TournoiEscrimeFantastique.Api.Services;

namespace TP.TournoiEscrimeFantastique.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class NotificationsController(
    TournamentDbContext db,
    DomainPlayerService domainPlayers,
    INotificationService notifications) : ControllerBase
{
    /// <summary>Retourne l'historique des notifications envoyées (les plus récentes d'abord).</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<NotificationDto>>(StatusCodes.Status200OK)]
    public IActionResult GetHistory() =>
        Ok(notifications.GetHistory().Select(NotificationDto.From));

    /// <summary>Envoie une notification ciblée à un joueur.</summary>
    [HttpPost("players/{id:int}")]
    [ProducesResponseType<NotificationDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> NotifyPlayer(int id, [FromBody] NotifyDto dto)
    {
        var player = await db.Players.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
        if (player is null) return NotFound();

        var sent = notifications.Notify(player.Name, dto.Message);
        return Ok(NotificationDto.From(sent));
    }

    /// <summary>
    /// Notifie chaque participant de sa position finale au classement.
    /// </summary>
    [HttpPost("broadcast-ranking")]
    [ProducesResponseType<IEnumerable<NotificationDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> BroadcastRanking()
    {
        var players = await db.Players
            .Include(p => p.Matches.OrderBy(m => m.MatchOrder))
            .AsNoTracking()
            .ToListAsync();

        var ranking = domainPlayers.GetRankingDtos(players);
        var sent = ranking
            .Select(entry =>
            {
                var message = entry.IsDisqualified
                    ? "Vous avez été disqualifié du tournoi. Score final : 0."
                    : entry.Rank == 1 && entry.Score > 0
                        ? $"Félicitations ! Vous êtes le champion du tournoi avec {entry.Score} points 🏆"
                        : $"Tournoi terminé : vous terminez {entry.Rank}ᵉ avec {entry.Score} points.";

                return NotificationDto.From(notifications.Notify(entry.Name, message));
            })
            .ToList();

        return Ok(sent);
    }
}
