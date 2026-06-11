using TP.TournoiEscrimeFantastique;
using static TP.TournoiEscrimeFantastique.MatchResult.Result;

namespace TP.TournoiEscrimeFantastique.Tests.TestData;

public static class ScoreCalculatorTheoryData
{
    private static MatchResult[] Matches(params MatchResult.Result[] outcomes) =>
        outcomes.Select(o => new MatchResult(o)).ToArray();

    // { résultats, pénalités, disqualifié, scoreAttendu, description }
    public static IEnumerable<object[]> ComplexScoringScenarios =>
        new List<object[]>
        {
            new object[] { Matches(Win, Win, Win, Loss, Win, Win, Win, Win), 0, false, 31, "Double bonus série" },
            new object[] { Matches(Win, Win, Draw),                          3, false,  4, "Pénalités normales" },
            new object[] { Matches(Win, Draw),                              10, false,  0, "Pénalités > score" },
            new object[] { Matches(Win, Win, Win),                           0, true,   0, "Disqualification" },
            new object[] { Matches(Win, Win, Win, Win),                      2, false, 15, "Bonus + pénalités" },
        };
}
