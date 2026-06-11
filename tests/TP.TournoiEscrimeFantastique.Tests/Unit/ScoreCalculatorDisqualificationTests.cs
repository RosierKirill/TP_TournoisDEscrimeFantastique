using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorDisqualificationTests
{
    private readonly ScoreCalculator _calculator = new();

    [Fact]
    public void CalculateScore_WhenDisqualifiedWithPositiveScore_Returns0()
    {
        var results = new List<MatchResult> { Win, Win, Win };

        var score = _calculator.CalculateScore(results, isDisqualified: true);

        score.Should().Be(0, because: "la disqualification annule tout score");
    }

    [Fact]
    public void CalculateScore_WhenDisqualifiedWithNoFights_Returns0()
    {
        var results = new List<MatchResult>();

        var score = _calculator.CalculateScore(results, isDisqualified: true);

        score.Should().Be(0, because: "la disqualification annule tout score");
    }
}
