namespace TP.TournoiEscrimeFantastique.Api.Duels;

/// <summary>Issue d'un duel pour le combattant A (B reçoit l'issue miroir).</summary>
public enum DuelOutcome
{
    PlayerAWins,
    PlayerBWins,
    Draw,
}

/// <summary>Tire l'issue d'un duel. Isolé pour rester testable / remplaçable.</summary>
public interface IDuelOutcomeGenerator
{
    DuelOutcome Draw();
}
