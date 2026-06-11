# Bonnes pratiques de test — Tournoi d'Escrime Fantastique

Conventions appliquées dans ce projet pour des tests clairs, fiables et maintenables.

## 1. Structure AAA (Arrange / Act / Assert)

Chaque test est découpé en trois blocs commentés : préparation des données, appel de
la méthode testée, vérification du résultat.

```csharp
[Fact]
public void CalculateScore_WithWinDrawLoss_Returns4Points()
{
    // Arrange
    var matches = new List<MatchResult> { Win, Draw, Loss };

    // Act
    var score = _calculator.CalculateScore(matches);

    // Assert
    score.Should().Be(4);
}
```

## 2. Nommage : `Méthode_Scénario_RésultatAttendu`

Le nom décrit ce qui est testé et l'attendu, sans ambiguïté.

- ✅ `CalculateScore_WithThreeConsecutiveWins_Returns14Points`
- ✅ `GetChampion_WhenAllPlayersDisqualified_ReturnsNull`
- ❌ `Test1()`, `CalculateScoreTest()`

## 3. Un seul concept par test

Un test vérifie **une** règle métier. Les variantes d'un même comportement sont
regroupées via des tests paramétrés plutôt que dupliquées.

## 4. FluentAssertions partout

On privilégie les assertions expressives (`Should()`) plutôt que `Assert.Equal`, avec
un message explicite quand il clarifie l'intention.

```csharp
score.Should().Be(0, because: "la disqualification annule tout score");
result.Should().Equal(galahad, morgane, noir);
act.Should().Throw<ArgumentNullException>().WithParameterName("matches");
```

## 5. Tests paramétrés

- `[Theory] + [InlineData]` pour les cas simples (entrée/sortie directe).
- `[Theory] + [MemberData]` (dans `TestData/`) pour les scénarios riches
  (pénalités, disqualification, séries multiples).

## 6. Isolation des dépendances avec Moq

Une classe qui dépend d'une autre est testée contre une **abstraction** mockée, pour
valider sa logique propre indépendamment de la dépendance.

```csharp
var calculatorMock = new Mock<IScoreCalculator>();
calculatorMock
    .Setup(c => c.CalculateScore(player.Matches, It.IsAny<bool>(), It.IsAny<int>()))
    .Returns(30);

var ranking = new TournamentRanking(calculatorMock.Object);
// ...
calculatorMock.Verify(
    c => c.CalculateScore(It.IsAny<IList<MatchResult>>(), It.IsAny<bool>(), It.IsAny<int>()),
    Times.Exactly(2));
```

> ⚠️ Moq compare les arguments `IEnumerable` par **contenu** (séquence), pas par
> référence : donner des contenus distincts aux jeux de données pour cibler les `Setup`.

## 7. TDD — Red / Green / Refactor

Le code métier est piloté par les tests :
1. **Red** : écrire un test qui échoue (`feat`/`test` rouge).
2. **Green** : implémenter le minimum pour le faire passer.
3. **Refactor** : nettoyer sans changer le comportement, tests toujours verts.

L'historique git reflète ces étapes (`test(red):` / `feat(green):`).

## 8. Couverture et garde-fou CI

- Cible : **≥ 95 %** sur `ScoreCalculator` (atteinte : 100 %).
- La CI (`.github/workflows/tests.yml`) bloque toute baisse sous le seuil.

## 9. Style et lint

Le fichier [`.editorconfig`](../.editorconfig) fixe l'indentation, les `using` hors
namespace, les namespaces *file-scoped* et les conventions de nommage
(interfaces `I*`, champs privés `_camelCase`). Vérification :

```bash
dotnet format TP.TournoiEscrimeFantastique.slnx --verify-no-changes
```
