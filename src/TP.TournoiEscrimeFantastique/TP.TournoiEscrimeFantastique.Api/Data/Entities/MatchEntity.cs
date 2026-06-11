namespace TP.TournoiEscrimeFantastique.Api.Data.Entities;

public class MatchEntity
{
    public int Id { get; set; }

    public int PlayerId { get; set; }

    public PlayerEntity Player { get; set; } = null!;

    /// <summary>Stored as "Win", "Draw" or "Loss".</summary>
    public string Outcome { get; set; } = "Win";

    public int MatchOrder { get; set; }
}
