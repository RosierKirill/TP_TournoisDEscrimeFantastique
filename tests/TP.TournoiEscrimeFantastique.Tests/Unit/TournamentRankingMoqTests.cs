using FluentAssertions;
using Moq;
using Xunit;
using TP.TournoiEscrimeFantastique;
using static TP.TournoiEscrimeFantastique.MatchResult.Result;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

/// <summary>
/// Tests de <see cref="TournamentRanking"/> en isolation : la dépendance
/// <see cref="IScoreCalculator"/> est remplacée par un mock Moq, afin de tester
/// la logique de classement indépendamment des règles de calcul.
/// </summary>
/// <remarks>
/// Moq compare les arguments <c>IEnumerable</c> par contenu (séquence) ; on donne donc
/// à chaque joueur une liste de matchs au contenu distinct pour cibler les setups.
/// </remarks>
public class TournamentRankingMoqTests
{
    private readonly Mock<IScoreCalculator> _calculatorMock = new();

    [Fact]
    public void GetRanking_OrdersPlayersByScoresReturnedByCalculator()
    {
        // Arrange : on impose des scores arbitraires via le mock
        var alice = new Player("Alice", new List<MatchResult> { new(Win) });
        var bob = new Player("Bob", new List<MatchResult> { new(Win), new(Draw) });
        var carol = new Player("Carol", new List<MatchResult> { new(Draw) });
        _calculatorMock.Setup(c => c.CalculateScore(alice.Matches, It.IsAny<bool>(), It.IsAny<int>())).Returns(10);
        _calculatorMock.Setup(c => c.CalculateScore(bob.Matches, It.IsAny<bool>(), It.IsAny<int>())).Returns(30);
        _calculatorMock.Setup(c => c.CalculateScore(carol.Matches, It.IsAny<bool>(), It.IsAny<int>())).Returns(20);
        var ranking = new TournamentRanking(_calculatorMock.Object);

        // Act
        var result = ranking.GetRanking(new List<Player> { alice, bob, carol });

        // Assert : 30 > 20 > 10
        result.Should().Equal(bob, carol, alice);
    }

    [Fact]
    public void GetRanking_CallsCalculatorOncePerPlayer()
    {
        // Arrange
        var p1 = new Player("P1", new List<MatchResult> { new(Win) });
        var p2 = new Player("P2", new List<MatchResult> { new(Draw) });
        _calculatorMock
            .Setup(c => c.CalculateScore(It.IsAny<List<MatchResult>>(), It.IsAny<bool>(), It.IsAny<int>()))
            .Returns(0);
        var ranking = new TournamentRanking(_calculatorMock.Object);

        // Act
        ranking.GetRanking(new List<Player> { p1, p2 });

        // Assert
        _calculatorMock.Verify(
            c => c.CalculateScore(It.IsAny<List<MatchResult>>(), It.IsAny<bool>(), It.IsAny<int>()),
            Times.Exactly(2));
    }

    [Fact]
    public void GetRanking_ForwardsPlayerDisqualificationAndPenaltyToCalculator()
    {
        // Arrange : le joueur disqualifié + un second pour déclencher le tri
        var cheaterMatches = new List<MatchResult> { new(Win) };
        var cheater = new Player("Tricheur", cheaterMatches, isDisqualified: true, penaltyPoints: 5);
        var other = new Player("Honnête", new List<MatchResult> { new(Draw) });
        _calculatorMock
            .Setup(c => c.CalculateScore(It.IsAny<List<MatchResult>>(), It.IsAny<bool>(), It.IsAny<int>()))
            .Returns(0);
        var ranking = new TournamentRanking(_calculatorMock.Object);

        // Act
        ranking.GetRanking(new List<Player> { cheater, other });

        // Assert : les drapeaux du joueur sont bien transmis au calculateur
        _calculatorMock.Verify(c => c.CalculateScore(cheaterMatches, true, 5), Times.Once);
    }

    [Fact]
    public void GetChampion_ReturnsPlayerWithHighestMockedScore()
    {
        // Arrange
        var weak = new Player("Faible", new List<MatchResult> { new(Draw) });
        var strong = new Player("Fort", new List<MatchResult> { new(Win) });
        _calculatorMock.Setup(c => c.CalculateScore(weak.Matches, It.IsAny<bool>(), It.IsAny<int>())).Returns(5);
        _calculatorMock.Setup(c => c.CalculateScore(strong.Matches, It.IsAny<bool>(), It.IsAny<int>())).Returns(99);
        var ranking = new TournamentRanking(_calculatorMock.Object);

        // Act
        var champion = ranking.GetChampion(new List<Player> { weak, strong });

        // Assert
        champion.Should().Be(strong);
    }

    [Fact]
    public void GetChampion_WhenCalculatorReturnsAllZero_ReturnsNull()
    {
        // Arrange
        var p1 = new Player("P1", new List<MatchResult> { new(Win) });
        var p2 = new Player("P2", new List<MatchResult> { new(Draw) });
        _calculatorMock
            .Setup(c => c.CalculateScore(It.IsAny<List<MatchResult>>(), It.IsAny<bool>(), It.IsAny<int>()))
            .Returns(0);
        var ranking = new TournamentRanking(_calculatorMock.Object);

        // Act
        var champion = ranking.GetChampion(new List<Player> { p1, p2 });

        // Assert
        champion.Should().BeNull();
    }
}
