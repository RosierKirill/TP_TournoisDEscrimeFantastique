namespace TP.TournoiEscrimeFantastique;

public class Player
{
    public string Name { get; }
    public IList<MatchResult> Matches { get; }
    public bool IsDisqualified { get; }
    public int PenaltyPoints { get; }

    public Player(string name, IList<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0)
    {
        Name = name;
        Matches = matches;
        IsDisqualified = isDisqualified;
        PenaltyPoints = penaltyPoints;
    }
}
