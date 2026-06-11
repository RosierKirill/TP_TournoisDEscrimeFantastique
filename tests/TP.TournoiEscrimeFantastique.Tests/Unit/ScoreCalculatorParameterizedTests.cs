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
    [InlineData(new[] { "Win" },                       3)]
    [InlineData(new[] { "Draw" },                      1)]
    [InlineData(new[] { "Loss" },                      0)]
    [InlineData(new[] { "Win", "Draw" },               4)]
    [InlineData(new[] { "Win", "Draw", "Loss" },       4)]
    [InlineData(new[] { "Win", "Win", "Win" },        14)]
    public void CalculateScore_WithInlineData_ReturnsExpectedScore(
        string[] results, int expectedScore)
    {
        var matches = results
            .Select(r => new MatchResult(Enum.Parse<MatchResult.Result>(r)))
            .ToList();

        var score = _calculator.CalculateScore(matches);

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
