using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult.Result;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorPenaltyTests
{
    private readonly ScoreCalculator _calculator = new();

    [Fact]
    public void CalculateScore_WithNormalPenalties_Returns7Points()
    {
        var results = new List<MatchResult> { new(Win), new(Draw), new(Win), new(Win) }; // 10 pts brut

        var score = _calculator.CalculateScore(results, penaltyPoints: 3);

        score.Should().Be(7); // 10 - 3 = 7
    }

    [Fact]
    public void CalculateScore_WithPenaltiesGreaterThanScore_Returns0()
    {
        var results = new List<MatchResult> { new(Draw), new(Win), new(Draw) }; // 5 pts brut

        var score = _calculator.CalculateScore(results, penaltyPoints: 8);

        score.Should().Be(0); // Max(0, 5-8) = 0
    }

    [Fact]
    public void CalculateScore_WithEqualPenalties_Returns0()
    {
        var results = new List<MatchResult> { new(Win), new(Draw), new(Win) }; // 7 pts brut

        var score = _calculator.CalculateScore(results, penaltyPoints: 7);

        score.Should().Be(0); // Max(0, 7-7) = 0
    }
}
