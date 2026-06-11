using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult.Result;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorBaseTests
{
    private readonly ScoreCalculator _calculator = new();

    [Fact]
    public void CalculateScore_WithWinDrawLoss_Returns4Points()
    {
        var results = new List<MatchResult> { new(Win), new(Draw), new(Loss) };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(4);
    }

    [Fact]
    public void CalculateScore_WithTwoWins_Returns6Points()
    {
        var results = new List<MatchResult> { new(Win), new(Win) };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(6);
    }

    [Fact]
    public void CalculateScore_WithThreeDraws_Returns3Points()
    {
        var results = new List<MatchResult> { new(Draw), new(Draw), new(Draw) };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(3);
    }

    [Fact]
    public void CalculateScore_WithTwoLosses_Returns0Points()
    {
        var results = new List<MatchResult> { new(Loss), new(Loss) };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(0);
    }
}
