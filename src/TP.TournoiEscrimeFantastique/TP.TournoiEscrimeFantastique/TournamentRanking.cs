namespace TP.TournoiEscrimeFantastique;

/// <summary>
/// Établit le classement des joueurs d'un tournoi à partir de leurs scores.
/// </summary>
public class TournamentRanking
{
    private readonly ScoreCalculator _scoreCalculator;

    public TournamentRanking(ScoreCalculator scoreCalculator)
    {
        _scoreCalculator = scoreCalculator;
    }

    /// <summary>
    /// Classe les joueurs par score décroissant (ordre d'entrée préservé en cas d'égalité).
    /// </summary>
    public List<Player> GetRanking(List<Player> players)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Retourne le joueur avec le meilleur score, ou <c>null</c> s'il n'y a pas de champion
    /// (liste vide ou meilleur score nul, p. ex. tous les joueurs disqualifiés).
    /// </summary>
    public Player? GetChampion(List<Player> players)
    {
        throw new NotImplementedException();
    }
}
