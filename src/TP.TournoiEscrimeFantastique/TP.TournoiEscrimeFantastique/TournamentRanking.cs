namespace TP.TournoiEscrimeFantastique;

/// <summary>
/// Établit le classement des joueurs d'un tournoi à partir de leurs scores.
/// </summary>
public class TournamentRanking
{
    private readonly IScoreCalculator _scoreCalculator;

    public TournamentRanking(IScoreCalculator scoreCalculator)
    {
        _scoreCalculator = scoreCalculator;
    }

    /// <summary>
    /// Classe les joueurs par score décroissant (ordre d'entrée préservé en cas d'égalité).
    /// </summary>
    public List<Player> GetRanking(List<Player> players)
    {
        ArgumentNullException.ThrowIfNull(players);

        // OrderByDescending est stable : l'ordre d'entrée est conservé pour les ex æquo.
        return players
            .OrderByDescending(ScoreOf)
            .ToList();
    }

    /// <summary>
    /// Retourne le joueur avec le meilleur score, ou <c>null</c> s'il n'y a pas de champion
    /// (liste vide ou meilleur score nul, p. ex. tous les joueurs disqualifiés).
    /// </summary>
    public Player? GetChampion(List<Player> players)
    {
        ArgumentNullException.ThrowIfNull(players);

        if (players.Count == 0)
        {
            return null;
        }

        var best = GetRanking(players)[0];
        return ScoreOf(best) > 0 ? best : null;
    }

    private int ScoreOf(Player player) =>
        _scoreCalculator.CalculateScore(player.Matches, player.IsDisqualified, player.PenaltyPoints);
}
