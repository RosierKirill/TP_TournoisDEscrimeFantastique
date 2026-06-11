using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorTests
{
    private readonly ScoreCalculator _calculator;

    public ScoreCalculatorTests()
    {
        _calculator = new ScoreCalculator();
    }

    // ── Catégorie 1 : Tests de base ──────────────────────────────────────────

    [Fact]
    public void CalculateScore_WithWinDrawLoss_Returns4Points()
    {
        // Arrange
        var results = new List<MatchResult> { Win, Draw, Loss };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(4);
    }

    [Fact]
    public void CalculateScore_WithTwoWins_Returns6Points()
    {
        // Arrange
        var results = new List<MatchResult> { Win, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(6);
    }

    [Fact]
    public void CalculateScore_WithThreeDraws_Returns3Points()
    {
        // Arrange
        var results = new List<MatchResult> { Draw, Draw, Draw };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(3);
    }

    [Fact]
    public void CalculateScore_WithTwoLosses_Returns0Points()
    {
        // Arrange
        var results = new List<MatchResult> { Loss, Loss };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(0);
    }

    // ── Catégorie 2 : Tests du bonus de série ────────────────────────────────

    [Fact]
    public void CalculateScore_WithThreeConsecutiveWins_Returns14Points()
    {
        // Arrange
        var results = new List<MatchResult> { Win, Win, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(14); // 9 pts + bonus 5
    }

    [Fact]
    public void CalculateScore_WithFourConsecutiveWins_Returns17Points()
    {
        // Arrange
        var results = new List<MatchResult> { Win, Win, Win, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(17); // 12 pts + bonus 5 (une seule fois)
    }

    [Fact]
    public void CalculateScore_WithWinWinLossWin_Returns9Points()
    {
        // Arrange
        // Série de 2 victoires interrompue par une défaite : aucun bonus
        var results = new List<MatchResult> { Win, Win, Loss, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(9); // 3+3+0+3 = 9, série jamais ≥ 3 → pas de bonus
    }

    [Fact]
    public void CalculateScore_WithMultipleBonusSeries_Returns31Points()
    {
        // Arrange
        // Win×3 → série 1 (+5), Loss brise, Win×4 → série 2 (+5)
        var results = new List<MatchResult> { Win, Win, Win, Loss, Win, Win, Win, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(31); // 21 pts base + 5 + 5 = 31
    }

    [Fact]
    public void CalculateScore_WithWinDrawWinWin_Returns10Points()
    {
        // Arrange
        // Draw brise toute continuité de série
        var results = new List<MatchResult> { Win, Draw, Win, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(10); // 3+1+3+3 = 10, pas de série ≥ 3
    }

    // ── Catégorie 3 : Tests de disqualification ──────────────────────────────

    [Fact]
    public void CalculateScore_WhenDisqualifiedWithPositiveScore_Returns0()
    {
        // Arrange
        var results = new List<MatchResult> { Win, Win, Win };

        // Act
        var score = _calculator.CalculateScore(results, isDisqualified: true);

        // Assert
        score.Should().Be(0, because: "la disqualification annule tout score");
    }

    [Fact]
    public void CalculateScore_WhenDisqualifiedWithNoFights_Returns0()
    {
        // Arrange
        var results = new List<MatchResult>();

        // Act
        var score = _calculator.CalculateScore(results, isDisqualified: true);

        // Assert
        score.Should().Be(0, because: "la disqualification annule tout score");
    }
}
