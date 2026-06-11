using FluentAssertions;
using Xunit;
using TP.TournoiEscrimeFantastique;
using static TP.TournoiEscrimeFantastique.MatchResult.Result;

namespace TP.TournoiEscrimeFantastique.Tests.Unit;

/// <summary>
/// Tests unitaires de <see cref="TournamentRanking"/>.
/// Convention : Méthode_Scénario_RésultatAttendu. Structure : Arrange / Act / Assert.
/// </summary>
public class TournamentRankingTests
{
    private readonly TournamentRanking _ranking = new(new ScoreCalculator());

    [Fact]
    public void GetRanking_WithDistinctScores_OrdersPlayersByScoreDescending()
    {
        // Arrange
        var galahad = new Player("Galahad", new List<MatchResult> { new(Win), new(Win), new(Win) }); // 14
        var morgane = new Player("Morgane", new List<MatchResult> { new(Win), new(Draw) });      // 4
        var noir = new Player("Chevalier Noir", new List<MatchResult> { new(Draw) });       // 1
        var players = new List<Player> { morgane, noir, galahad };

        // Act
        var ranking = _ranking.GetRanking(players);

        // Assert
        ranking.Should().Equal(galahad, morgane, noir);
    }

    [Fact]
    public void GetRanking_WithTiedScores_PreservesInputOrder()
    {
        // Arrange
        var top = new Player("Top", new List<MatchResult> { new(Win), new(Win), new(Win) }); // 14
        var x = new Player("X", new List<MatchResult> { new(Win) });               // 3
        var y = new Player("Y", new List<MatchResult> { new(Draw), new(Draw), new(Draw) });  // 3
        var players = new List<Player> { x, top, y };

        // Act
        var ranking = _ranking.GetRanking(players);

        // Assert : top en tête, puis X avant Y (ordre d'entrée conservé pour les ex æquo)
        ranking.Should().Equal(top, x, y);
    }

    [Fact]
    public void GetRanking_WithNullPlayers_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _ranking.GetRanking(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("players");
    }

    [Fact]
    public void GetChampion_WithSeveralPlayers_ReturnsHighestScorer()
    {
        // Arrange
        var galahad = new Player("Galahad", new List<MatchResult> { new(Win), new(Win), new(Win) }); // 14
        var morgane = new Player("Morgane", new List<MatchResult> { new(Win), new(Draw) });      // 4
        var players = new List<Player> { morgane, galahad };

        // Act
        var champion = _ranking.GetChampion(players);

        // Assert
        champion.Should().Be(galahad);
    }

    [Fact]
    public void GetChampion_WhenAllPlayersDisqualified_ReturnsNull()
    {
        // Arrange
        var p1 = new Player("P1", new List<MatchResult> { new(Win), new(Win), new(Win) }, isDisqualified: true); // 0
        var p2 = new Player("P2", new List<MatchResult> { new(Win), new(Draw) }, isDisqualified: true);      // 0
        var players = new List<Player> { p1, p2 };

        // Act
        var champion = _ranking.GetChampion(players);

        // Assert
        champion.Should().BeNull("aucun champion possible si tous les joueurs sont disqualifiés");
    }

    [Fact]
    public void GetChampion_WithNullPlayers_ThrowsArgumentNullException()
    {
        // Act
        var act = () => _ranking.GetChampion(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("players");
    }

    [Fact]
    public void GetRanking_WithEmptyList_ReturnsEmpty()
    {
        // Arrange
        var players = new List<Player>();

        // Act
        var ranking = _ranking.GetRanking(players);

        // Assert
        ranking.Should().BeEmpty();
    }

    [Fact]
    public void GetChampion_WithEmptyList_ReturnsNull()
    {
        // Arrange
        var players = new List<Player>();

        // Act
        var champion = _ranking.GetChampion(players);

        // Assert
        champion.Should().BeNull();
    }
}
