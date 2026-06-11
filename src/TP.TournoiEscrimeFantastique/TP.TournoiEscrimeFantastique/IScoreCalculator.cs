namespace TP.TournoiEscrimeFantastique;

/// <summary>
/// Abstraction du calcul de score, pour permettre l'injection et l'isolation en test (Moq).
/// </summary>
public interface IScoreCalculator
{
    /// <summary>
    /// Calcule le score final d'un joueur selon les règles du tournoi.
    /// </summary>
    int CalculateScore(List<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0);
}
