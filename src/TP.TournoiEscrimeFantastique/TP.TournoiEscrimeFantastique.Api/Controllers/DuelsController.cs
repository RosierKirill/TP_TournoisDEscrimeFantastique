using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TP.TournoiEscrimeFantastique.Api.Data;
using TP.TournoiEscrimeFantastique.Api.Data.Entities;
using TP.TournoiEscrimeFantastique.Api.DTOs;
using TP.TournoiEscrimeFantastique.Api.Duels;
using TP.TournoiEscrimeFantastique.Api.Notifications;
using DomainMatchResult = TP.TournoiEscrimeFantastique.MatchResult;
using IFightScoreCalculator = TP.TournoiEscrimeFantastique.IScoreCalculator;

namespace TP.TournoiEscrimeFantastique.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class DuelsController(
    TournamentDbContext db,
    IFightScoreCalculator scoreCalculator,
    IDuelOutcomeGenerator outcomeGenerator,
    INotificationService notifications) : ControllerBase
{
    /// <summary>
    /// Lance un duel entre deux combattants. L'issue est tirée aléatoirement
    /// (45 % / 45 % / 10 % nul), ajoutée aux deux profils, et les deux joueurs
    /// sont notifiés au lancement puis à l'issue du duel.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<DuelResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartDuel([FromBody] StartDuelDto dto)
    {
        if (dto.PlayerAId == dto.PlayerBId)
            return BadRequest("Un combattant ne peut pas se battre contre lui-même.");

        var playerA = await FindAsync(dto.PlayerAId);
        var playerB = await FindAsync(dto.PlayerBId);
        if (playerA is null || playerB is null) return NotFound("Un des combattants est introuvable.");

        // Notification de lancement
        notifications.Notify(playerA.Name, $"Un duel commence : {playerA.Name} affronte {playerB.Name} ! ⚔️");
        notifications.Notify(playerB.Name, $"Un duel commence : {playerB.Name} affronte {playerA.Name} ! ⚔️");

        // Tirage de l'issue
        var (outcomeA, outcomeB, winner) = outcomeGenerator.Draw() switch
        {
            DuelOutcome.PlayerAWins => ("Win", "Loss", "A"),
            DuelOutcome.PlayerBWins => ("Loss", "Win", "B"),
            _ => ("Draw", "Draw", "Draw"),
        };

        AppendMatch(playerA, outcomeA);
        AppendMatch(playerB, outcomeB);
        await db.SaveChangesAsync();

        var scoreA = ScoreOf(playerA);
        var scoreB = ScoreOf(playerB);

        // Notification du résultat
        notifications.Notify(playerA.Name, ResultMessage(playerA.Name, playerB.Name, outcomeA, scoreA));
        notifications.Notify(playerB.Name, ResultMessage(playerB.Name, playerA.Name, outcomeB, scoreB));

        return Ok(new DuelResultDto(
            new DuelCombatantDto(playerA.Id, playerA.Name, outcomeA, scoreA),
            new DuelCombatantDto(playerB.Id, playerB.Name, outcomeB, scoreB),
            winner));
    }

    private static string ResultMessage(string self, string opponent, string outcome, int score) =>
        outcome switch
        {
            "Win" => $"Victoire contre {opponent} ! Vous totalisez désormais {score} points. 🏅",
            "Loss" => $"Défaite face à {opponent}. Score actuel : {score} points.",
            _ => $"Match nul contre {opponent}. Score actuel : {score} points.",
        };

    private static void AppendMatch(PlayerEntity player, string outcome)
    {
        var nextOrder = player.Matches.Count == 0 ? 1 : player.Matches.Max(m => m.MatchOrder) + 1;
        player.Matches.Add(new MatchEntity
        {
            PlayerId = player.Id,
            Outcome = outcome,
            MatchOrder = nextOrder,
        });
    }

    private int ScoreOf(PlayerEntity player) =>
        scoreCalculator.CalculateScore(
            player.Matches.OrderBy(m => m.MatchOrder)
                          .Select(m => OutcomeMapper.ToMatchResult(m.Outcome))
                          .ToList<DomainMatchResult>(),
            player.IsDisqualified,
            player.PenaltyPoints);

    private async Task<PlayerEntity?> FindAsync(int id) =>
        await db.Players
            .Include(p => p.Matches.OrderBy(m => m.MatchOrder))
            .FirstOrDefaultAsync(p => p.Id == id);
}
