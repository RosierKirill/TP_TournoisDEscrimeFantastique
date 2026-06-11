using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorEdgeCaseTests
{
    private readonly ScoreCalculator _calculator = new();

    [Fact]
    public void CalculateScore_WithEmptyList_Returns0()
    {
        var results = new List<MatchResult>();

        var score = _calculator.CalculateScore(results);

        score.Should().Be(0);
    }

    [Fact]
    public void CalculateScore_WithNullList_ThrowsArgumentNullException()
    {
        List<MatchResult> results = null!;

        var act = () => _calculator.CalculateScore(results);

        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("matches");
    }

    [Fact]
    public void CalculateScore_WithNegativePenalties_ThrowsArgumentException()
    {
        var results = new List<MatchResult> { Win };

        var act = () => _calculator.CalculateScore(results, penaltyPoints: -5);

        act.Should().Throw<ArgumentException>()
           .WithMessage("*négatives*");
    }

    [Fact]
    public void CalculateScore_WithHundredFights_ReturnsCorrectScore()
    {
        // Pattern (Win×3, Loss) × 25 : 25 séries de 3 victoires consécutives
        var results = Enumerable.Repeat(
            new[] { Win, Win, Win, Loss }, 25)
            .SelectMany(x => x)
            .ToList();

        var score = _calculator.CalculateScore(results);

        // 25 × (9 pts base + 5 bonus) = 25 × 14 = 350
        score.Should().Be(350);
    }
}
