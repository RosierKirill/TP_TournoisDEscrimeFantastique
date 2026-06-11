using TP.TournoiEscrimeFantastique;
using static TP.TournoiEscrimeFantastique.MatchResult.Result;

namespace TP.TournoiEscrimeFantastique.Api.Data;

internal static class OutcomeMapper
{
    private static readonly string[] ValidOutcomes = ["Win", "Draw", "Loss"];

    public static bool IsValid(string? value) =>
        string.Equals(value, "Win",  StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "Draw", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(value, "Loss", StringComparison.OrdinalIgnoreCase);

    public static MatchResult ToMatchResult(string outcome) =>
        outcome switch
        {
            "Draw" => new MatchResult(Draw),
            "Loss" => new MatchResult(Loss),
            _      => new MatchResult(Win),
        };

    public static string Normalize(string outcome)
    {
        if (string.Equals(outcome, "Win",  StringComparison.OrdinalIgnoreCase)) return "Win";
        if (string.Equals(outcome, "Draw", StringComparison.OrdinalIgnoreCase)) return "Draw";
        return "Loss";
    }

    public static string ValidValues => string.Join(", ", ValidOutcomes);
}
