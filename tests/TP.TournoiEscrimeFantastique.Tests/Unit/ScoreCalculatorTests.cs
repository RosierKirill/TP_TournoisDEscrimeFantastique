using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using Xunit;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorTests
{
    private readonly ScoreCalculator _calculator;

    public ScoreCalculatorTests()
    {
        _calculator = new ScoreCalculator();
    }

    [Fact]
    public void CalculateScore_WithWinDrawLoss_Returns4Points()
    {
        // Arrange
        var results = new List<FightResult>
        {
            FightResult.Win,
            FightResult.Draw,
            FightResult.Loss
        };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(4);
    }

    [Fact]
    public void CalculateScore_WithTwoWins_Returns6Points()
    {
        // Arrange
        var results = new List<FightResult>
        {
            FightResult.Win,
            FightResult.Win
        };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(6);
    }

    [Fact]
    public void CalculateScore_WithThreeDraws_Returns3Points()
    {
        // Arrange
        var results = new List<FightResult>
        {
            FightResult.Draw,
            FightResult.Draw,
            FightResult.Draw
        };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(3);
    }

    [Fact]
    public void CalculateScore_WithTwoLosses_Returns0Points()
    {
        // Arrange
        var results = new List<FightResult>
        {
            FightResult.Loss,
            FightResult.Loss
        };

        // Act
        var score = _calculator.CalculateScore(results);

        // Assert
        score.Should().Be(0);
    }
}
