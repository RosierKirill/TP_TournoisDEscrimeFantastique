using FluentAssertions;
using TP.TournoiEscrimeFantastique;
using TP.TournoiEscrimeFantastique.Tests.TestData;
using Xunit;
using static TP.TournoiEscrimeFantastique.MatchResult;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

public class ScoreCalculatorParameterizedTests
{
    private readonly ScoreCalculator _calculator = new();

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
        var score = _calculator.CalculateScore(results.ToList());

        score.Should().Be(expectedScore);
    }

    [Theory]
    [MemberData(nameof(ScoreCalculatorTheoryData.ComplexScoringScenarios),
                MemberType = typeof(ScoreCalculatorTheoryData))]
    public void CalculateScore_WithMemberData_ReturnsExpectedScore(
        MatchResult[] results, int penalties, bool isDisqualified, int expectedScore, string description)
    {
        var score = _calculator.CalculateScore(
            results.ToList(),
            isDisqualified: isDisqualified,
            penaltyPoints: penalties);

        score.Should().Be(expectedScore, because: description);
    }
}
