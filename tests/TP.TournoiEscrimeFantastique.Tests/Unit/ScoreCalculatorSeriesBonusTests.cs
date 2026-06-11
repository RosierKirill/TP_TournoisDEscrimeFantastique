using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorSeriesBonusTests
{
    private readonly ScoreCalculator _calculator = new();

    [Fact]
    public void CalculateScore_WithThreeConsecutiveWins_Returns14Points()
    {
        var results = new List<MatchResult> { Win, Win, Win };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(14); // 9 pts + bonus 5
    }

    [Fact]
    public void CalculateScore_WithFourConsecutiveWins_Returns17Points()
    {
        var results = new List<MatchResult> { Win, Win, Win, Win };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(17); // 12 pts + bonus 5 (une seule fois)
    }

    [Fact]
    public void CalculateScore_WithWinWinLossWin_Returns9Points()
    {
        // Série de 2 victoires interrompue : jamais ≥ 3 consécutives → pas de bonus
        var results = new List<MatchResult> { Win, Win, Loss, Win };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(9); // 3+3+0+3 = 9
    }

    [Fact]
    public void CalculateScore_WithMultipleBonusSeries_Returns31Points()
    {
        // Win×3 → série 1 (+5), Loss brise, Win×4 → série 2 (+5)
        var results = new List<MatchResult> { Win, Win, Win, Loss, Win, Win, Win, Win };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(31); // 21 pts base + 5 + 5
    }

    [Fact]
    public void CalculateScore_WithWinDrawWinWin_Returns10Points()
    {
        // Draw brise la série : jamais 3 victoires consécutives
        var results = new List<MatchResult> { Win, Draw, Win, Win };

        var score = _calculator.CalculateScore(results);

        score.Should().Be(10); // 3+1+3+3 = 10
    }
}
