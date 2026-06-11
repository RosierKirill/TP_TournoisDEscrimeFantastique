using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using TP.TournoiEscrimeFantastique.Tests.TestData;
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
        // Série de 2 victoires interrompue : jamais ≥ 3 consécutives → pas de bonus
        var results = new List<MatchResult> { Win, Win, Loss, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(9); // 3+3+0+3 = 9
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
        score.Should().Be(31); // 21 pts base + 5 + 5
    }

    [Fact]
    public void CalculateScore_WithWinDrawWinWin_Returns10Points()
    {
        // Arrange
        // Draw brise la série : jamais 3 victoires consécutives
        var results = new List<MatchResult> { Win, Draw, Win, Win };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(10); // 3+1+3+3 = 10
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

    // ── Catégorie 4 : Tests des pénalités ────────────────────────────────────

    [Fact]
    public void CalculateScore_WithNormalPenalties_Returns7Points()
    {
        // Arrange
        var results = new List<MatchResult> { Win, Draw, Win, Win }; // 10 pts brut

        // Act
        var score = _calculator.CalculateScore(results, penaltyPoints: 3);

        // Assert
        score.Should().Be(7); // 10 - 3 = 7
    }

    [Fact]
    public void CalculateScore_WithPenaltiesGreaterThanScore_Returns0()
    {
        // Arrange
        var results = new List<MatchResult> { Draw, Win, Draw }; // 5 pts brut

        // Act
        var score = _calculator.CalculateScore(results, penaltyPoints: 8);

        // Assert
        score.Should().Be(0); // Max(0, 5-8) = 0
    }

    [Fact]
    public void CalculateScore_WithEqualPenalties_Returns0()
    {
        // Arrange
        var results = new List<MatchResult> { Win, Draw, Win }; // 7 pts brut

        // Act
        var score = _calculator.CalculateScore(results, penaltyPoints: 7);

        // Assert
        score.Should().Be(0); // Max(0, 7-7) = 0
    }

    // ── Catégorie 5 : Tests des cas limites ──────────────────────────────────

    [Fact]
    public void CalculateScore_WithEmptyList_Returns0()
    {
        // Arrange
        var results = new List<MatchResult>();

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(0);
    }

    [Fact]
    public void CalculateScore_WithNullList_ThrowsArgumentNullException()
    {
        // Arrange
        List<MatchResult> results = null!;

        // Act
        var act = () => _calculator.CalculateScore(results);

        // Assert
        act.Should().Throw<ArgumentNullException>()
           .WithParameterName("matches");
    }

    [Fact]
    public void CalculateScore_WithNegativePenalties_ThrowsArgumentException()
    {
        // Arrange
        var results = new List<MatchResult> { Win };

        // Act
        var act = () => _calculator.CalculateScore(results, penaltyPoints: -5);

        // Assert
        act.Should().Throw<ArgumentException>()
           .WithMessage("*négatives*");
    }

    [Fact]
    public void CalculateScore_WithHundredFights_ReturnsCorrectScore()
    {
        // Arrange
        // Pattern (Win×3, Loss) × 25 : 25 séries de 3 victoires consécutives
        var results = Enumerable.Repeat(
            new[] { Win, Win, Win, Loss }, 25)
            .SelectMany(x => x)
            .ToList();

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        // 25 × (9 pts base + 5 bonus) = 25 × 14 = 350
        score.Should().Be(350);
    }

    // ── Catégorie 6 : Tests paramétrés ───────────────────────────────────────

    [Theory]
    [InlineData(new MatchResult[] { Win },                         3)]
    [InlineData(new MatchResult[] { Draw },                        1)]
    [InlineData(new MatchResult[] { Loss },                        0)]
    [InlineData(new MatchResult[] { Win, Draw },                   4)]
    [InlineData(new MatchResult[] { Win, Draw, Loss },             4)]
    [InlineData(new MatchResult[] { Win, Win, Win },              14)]
    public void CalculateScore_WithInlineData_ReturnsExpectedScore(
        MatchResult[] results, int expectedScore)
    {
        // Act
        var score = _calculator.CalculateScore(results.ToList());

        // Assert
        score.Should().Be(expectedScore);
    }

    [Theory]
    [MemberData(nameof(ScoreCalculatorTheoryData.ComplexScoringScenarios),
                MemberType = typeof(ScoreCalculatorTheoryData))]
    public void CalculateScore_WithMemberData_ReturnsExpectedScore(
        MatchResult[] results, int penalties, bool isDisqualified, int expectedScore, string description)
    {
        // Act
        var score = _calculator.CalculateScore(
            results.ToList(),
            isDisqualified: isDisqualified,
            penaltyPoints: penalties);

        // Assert
        score.Should().Be(expectedScore, because: description);
    }
}
