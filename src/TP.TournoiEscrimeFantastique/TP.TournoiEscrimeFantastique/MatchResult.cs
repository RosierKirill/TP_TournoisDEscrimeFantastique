namespace TP.TournoiEscrimeFantastique;

public class MatchResult
{
    public enum Result
    {
        Win,  // Victoire
        Draw, // Match nul
        Loss  // Défaite
    }

    public Result Outcome { get; set; }

    // Constructeur pour faciliter les tests
    public MatchResult(Result outcome)
    {
        Outcome = outcome;
    }

    // Constructeur par défaut
    public MatchResult() { }
}
