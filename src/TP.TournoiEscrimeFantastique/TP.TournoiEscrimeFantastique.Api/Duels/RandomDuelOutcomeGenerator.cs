namespace TP.TournoiEscrimeFantastique.Api.Duels;

/// <summary>
/// Tirage aléatoire : 45 % victoire A, 45 % victoire B, 10 % match nul.
/// </summary>
public class RandomDuelOutcomeGenerator : IDuelOutcomeGenerator
{
    public DuelOutcome Draw()
    {
        var roll = Random.Shared.Next(100); // 0..99
        return roll switch
        {
            < 45 => DuelOutcome.PlayerAWins,
            < 90 => DuelOutcome.PlayerBWins,
            _ => DuelOutcome.Draw,
        };
    }
}
