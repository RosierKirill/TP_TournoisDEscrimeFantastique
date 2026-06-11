using System.ComponentModel.DataAnnotations;

namespace TP.TournoiEscrimeFantastique.Api.DTOs;

/// <summary>Demande de duel entre deux combattants.</summary>
public record StartDuelDto(
    [Required] int PlayerAId,
    [Required] int PlayerBId
);

/// <summary>Résultat d'un duel du point de vue d'un combattant.</summary>
public record DuelCombatantDto(int PlayerId, string Name, string Outcome, int Score);

/// <summary>Résultat complet d'un duel.</summary>
/// <param name="Winner">"A", "B" ou "Draw".</param>
public record DuelResultDto(
    DuelCombatantDto PlayerA,
    DuelCombatantDto PlayerB,
    string Winner
);
