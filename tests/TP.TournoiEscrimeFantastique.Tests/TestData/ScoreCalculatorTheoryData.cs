using TP.TournoiEscrimeFantastique;
using static TP.TournoiEscrimeFantastique.MatchResult;

namespace TP.TournoiEscrimeFantastique.Tests.TestData;

public static class ScoreCalculatorTheoryData
{
    // { résultats, pénalités, disqualifié, scoreAttendu, description }
    public static IEnumerable<object[]> ComplexScoringScenarios =>
        new List<object[]>
        {
            new object[] { new[] { Win, Win, Win, Loss, Win, Win, Win, Win }, 0, false, 31, "Double bonus série" },
            new object[] { new[] { Win, Win, Draw },                          3, false,  4, "Pénalités normales" },
            new object[] { new[] { Win, Draw },                              10, false,  0, "Pénalités > score" },
            new object[] { new[] { Win, Win, Win },                           0, true,   0, "Disqualification" },
            new object[] { new[] { Win, Win, Win, Win },                      2, false, 15, "Bonus + pénalités" },
        };
}
