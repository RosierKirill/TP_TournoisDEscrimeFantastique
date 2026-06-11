namespace TP.TournoiEscrimeFantastique;

public class Player
{
    public string Name { get; set; } = string.Empty;
    public List<MatchResult> Matches { get; set; } = new();
    public bool IsDisqualified { get; set; }
    public int PenaltyPoints { get; set; }

    // Constructeur par défaut (initialisation par propriétés possible).
    public Player() { }

    // Constructeur de commodité pour faciliter les tests.
    public Player(string name, List<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0)
    {
        Name = name;
        Matches = matches;
        IsDisqualified = isDisqualified;
        PenaltyPoints = penaltyPoints;
    }
}
