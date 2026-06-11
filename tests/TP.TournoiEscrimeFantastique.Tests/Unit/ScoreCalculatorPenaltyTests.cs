using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorPenaltyTests
{
    private readonly ScoreCalculator _calculator = new();

    [Fact]
    public void CalculateScore_WithNormalPenalties_Returns7Points()
    {
        var results = new List<MatchResult> { Win, Draw, Win, Win }; // 10 pts brut

        var score = _calculator.CalculateScore(results, penaltyPoints: 3);

        score.Should().Be(7); // 10 - 3 = 7
    }

    [Fact]
    public void CalculateScore_WithPenaltiesGreaterThanScore_Returns0()
    {
        var results = new List<MatchResult> { Draw, Win, Draw }; // 5 pts brut

        var score = _calculator.CalculateScore(results, penaltyPoints: 8);

        score.Should().Be(0); // Max(0, 5-8) = 0
    }

    [Fact]
    public void CalculateScore_WithEqualPenalties_Returns0()
    {
        var results = new List<MatchResult> { Win, Draw, Win }; // 7 pts brut

        var score = _calculator.CalculateScore(results, penaltyPoints: 7);

        score.Should().Be(0); // Max(0, 7-7) = 0
    }
}
