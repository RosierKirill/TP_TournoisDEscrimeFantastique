# Plan de tests : Tournoi d'Escrime Fantastique

## Vue d'ensemble

Ce document décrit l'ensemble des tests à implémenter pour le système de notation du tournoi d'escrime fantastique.  
Les tests sont organisés par catégorie, du plus simple au plus complexe, en suivant les bonnes pratiques xUnit + FluentAssertions + Moq.

---

## Architecture des tests

```
tests/
└── TP.TournoiEscrimeFantastique.Tests/
    ├── Unit/
    │   ├── ScoreCalculatorTests.cs        # Tests du calculateur de score (20 tests)
    │   └── TournamentServiceTests.cs      # Tests du service métier avec Moq
    ├── TestData/
    │   └── ScoreCalculatorTheoryData.cs   # Données paramétrées (MemberData)
    └── TP.TournoiEscrimeFantastique.Tests.csproj
```

**Packages NuGet requis :**
- `xunit` / `xunit.runner.visualstudio`
- `FluentAssertions`
- `Moq`
- `coverlet.collector` (couverture de code)

---

## Conventions de nommage

Tous les tests suivent la convention :

```
MéthodeTestée_Scénario_RésultatAttendu
```

Exemples :
- `CalculateScore_WithWinDrawLoss_Returns4Points`
- `CalculateScore_WhenDisqualified_Returns0`
- `CalculateScore_WithNullList_ThrowsArgumentNullException`

---

## Catégorie 1 — Tests de base (fonctionnement normal)

Ces tests vérifient le calcul des points bruts sans règles spéciales.

| # | Nom du test | Entrée | Résultat attendu | Règle vérifiée |
|---|-------------|--------|------------------|----------------|
| 1 | `CalculateScore_WithWinDrawLoss_Returns4Points` | Win, Draw, Loss | 4 pts | 3 + 1 + 0 = 4 |
| 2 | `CalculateScore_WithTwoWins_Returns6Points` | Win, Win | 6 pts | 3 + 3 = 6 |
| 3 | `CalculateScore_WithThreeDraws_Returns3Points` | Draw, Draw, Draw | 3 pts | 1 + 1 + 1 = 3 |
| 4 | `CalculateScore_WithTwoLosses_Returns0Points` | Loss, Loss | 0 pts | 0 + 0 = 0 |

**Structure type :**
```csharp
[Fact]
public void CalculateScore_WithWinDrawLoss_Returns4Points()
{
    // Arrange
    var results = new List<FightResult> { Win, Draw, Loss };

    // Act
    var score = _calculator.CalculateScore(results);

    // Assert
    score.Should().Be(4);
}
```

---

## Catégorie 2 — Tests du bonus de série

Ces tests vérifient l'attribution du bonus de +5 pts pour 3 victoires consécutives ou plus.

| # | Nom du test | Entrée | Résultat attendu | Détail |
|---|-------------|--------|------------------|--------|
| 5 | `CalculateScore_WithThreeConsecutiveWins_Returns14Points` | Win×3 | 14 pts | 9 pts + bonus 5 |
| 6 | `CalculateScore_WithFourConsecutiveWins_Returns17Points` | Win×4 | 17 pts | 12 pts + bonus 5 (une seule fois) |
| 7 | `CalculateScore_WithWinWinLossWin_Returns6Points` | Win, Win, Loss, Win | 6 pts | Série interrompue, pas de bonus |
| 8 | `CalculateScore_WithMultipleBonusSeries_Returns26Points` | Win×3, Loss, Win×4 | 26 pts | 21 pts + 5 + 5 (deux séries) |
| 9 | `CalculateScore_WithWinDrawWinWin_Returns10Points` | Win, Draw, Win, Win | 10 pts | Draw brise la série, pas de bonus |

**Cas #8 — détail du calcul :**
```
Win Win Win Loss Win Win Win Win
 3   3   3   0   3   3   3   3  = 21 pts
Série 1 (Win×3) → +5
Série 2 (Win×4) → +5
Total : 21 + 5 + 5 = 31 pts
```

> **Point d'attention :** Le bonus ne se cumule pas pour la même série (Win×4 = +5, pas +10). Une nouvelle série repart de zéro après une interruption.

---

## Catégorie 3 — Tests de disqualification

Ces tests vérifient que la disqualification annule **intégralement** le score, quelle que soit la performance.

| # | Nom du test | Entrée | Résultat attendu | Détail |
|---|-------------|--------|------------------|--------|
| 10 | `CalculateScore_WhenDisqualifiedWithPositiveScore_Returns0` | Win×3 + disqualifié | 0 pts | 14 pts annulés |
| 11 | `CalculateScore_WhenDisqualifiedWithNoFights_Returns0` | Aucun combat + disqualifié | 0 pts | Cas minimal |

**Structure type :**
```csharp
[Fact]
public void CalculateScore_WhenDisqualifiedWithPositiveScore_Returns0()
{
    // Arrange
    var results = new List<FightResult> { Win, Win, Win };
    var participant = new Participant(results, isDisqualified: true);

    // Act
    var score = _calculator.CalculateScore(participant);

    // Assert
    score.Should().Be(0, because: "la disqualification annule tout score");
}
```

---

## Catégorie 4 — Tests des pénalités

Ces tests vérifient que les pénalités soustraient des points sans jamais produire un score négatif.

| # | Nom du test | Score brut | Pénalités | Score final | Règle |
|---|-------------|------------|-----------|-------------|-------|
| 12 | `CalculateScore_WithNormalPenalties_Returns7Points` | 10 pts | 3 pts | 7 pts | Soustraction simple |
| 13 | `CalculateScore_WithPenaltiesGreaterThanScore_Returns0` | 5 pts | 8 pts | 0 pts | Plancher à 0 |
| 14 | `CalculateScore_WithEqualPenalties_Returns0` | 7 pts | 7 pts | 0 pts | Résultat nul = 0 |

> **Règle métier :** `score_final = Max(0, score_brut - pénalités)`

---

## Catégorie 5 — Tests des cas limites

Ces tests vérifient le comportement aux frontières et les cas d'erreur.

| # | Nom du test | Entrée | Résultat attendu | Type de vérification |
|---|-------------|--------|------------------|----------------------|
| 15 | `CalculateScore_WithEmptyList_Returns0` | Liste vide `[]` | 0 pts | Valeur par défaut |
| 16 | `CalculateScore_WithNullList_ThrowsArgumentNullException` | `null` | `ArgumentNullException` | Exception |
| 17 | `CalculateScore_WithNegativePenalties_ThrowsArgumentException` | Pénalités = -5 | `ArgumentException` | Exception |
| 18 | `CalculateScore_WithHundredFights_ReturnsCorrectScore` | 100 combats pattern complexe | Score calculé | Performance / fiabilité |

**Test #16 — vérification d'exception avec FluentAssertions :**
```csharp
[Fact]
public void CalculateScore_WithNullList_ThrowsArgumentNullException()
{
    // Arrange
    List<FightResult> results = null;

    // Act
    var act = () => _calculator.CalculateScore(results);

    // Assert
    act.Should().Throw<ArgumentNullException>()
       .WithParameterName("results");
}
```

**Test #17 — pénalités négatives :**
```csharp
[Fact]
public void CalculateScore_WithNegativePenalties_ThrowsArgumentException()
{
    // Arrange
    var results = new List<FightResult> { Win };

    // Act
    var act = () => _calculator.CalculateScore(results, penalties: -5);

    // Assert
    act.Should().Throw<ArgumentException>()
       .WithMessage("*négatif*");
}
```

**Test #18 — tournoi long (100 combats) :**
```
Pattern : (Win, Win, Win, Loss) × 25
→ 25 séries de 3 victoires consécutives
→ Score brut : 25 × (3+3+3+0) = 225 pts
→ Bonus : 25 × 5 = 125 pts
→ Score final : 350 pts
```

---

## Catégorie 6 — Tests paramétrés

### 6.1 — Theory avec InlineData (scénarios simples)

Regroupe plusieurs variantes d'un même scénario en un seul test paramétré.

```csharp
[Theory]
[InlineData(new[] { Win },               3)]
[InlineData(new[] { Draw },              1)]
[InlineData(new[] { Loss },              0)]
[InlineData(new[] { Win, Draw },         4)]
[InlineData(new[] { Win, Draw, Loss },   4)]
[InlineData(new[] { Win, Win, Win },    14)]
public void CalculateScore_WithInlineData_ReturnsExpectedScore(
    FightResult[] results,
    int expectedScore)
{
    // Act
    var score = _calculator.CalculateScore(results.ToList());

    // Assert
    score.Should().Be(expectedScore);
}
```

> **Test #19** — couvre au minimum 6 cas avec `[InlineData]`.

### 6.2 — Theory avec MemberData (cas complexes)

Pour les scénarios trop riches pour InlineData (pénalités, disqualification, séries multiples).

```csharp
// Dans ScoreCalculatorTheoryData.cs
public static IEnumerable<object[]> ComplexScoringScenarios =>
    new List<object[]>
    {
        // { résultats, pénalités, disqualifié, scoreAttendu, description }
        new object[] { new[] { Win,Win,Win,Loss,Win,Win,Win,Win }, 0, false, 31, "Double bonus série" },
        new object[] { new[] { Win,Win,Draw },                     3, false,  4, "Pénalités normales" },
        new object[] { new[] { Win,Draw },                        10, false,  0, "Pénalités > score" },
        new object[] { new[] { Win,Win,Win },                      0, true,   0, "Disqualification" },
        new object[] { new[] { Win,Win,Win,Win },                  2, false, 15, "Bonus + pénalités" },
    };

// Dans ScoreCalculatorTests.cs
[Theory]
[MemberData(nameof(ScoreCalculatorTheoryData.ComplexScoringScenarios),
            MemberType = typeof(ScoreCalculatorTheoryData))]
public void CalculateScore_WithMemberData_ReturnsExpectedScore(
    FightResult[] results,
    int penalties,
    bool isDisqualified,
    int expectedScore,
    string description)
{
    // Act
    var score = _calculator.CalculateScore(results.ToList(), penalties, isDisqualified);

    // Assert
    score.Should().Be(expectedScore, because: description);
}
```

> **Test #20** — couvre au minimum 5 cas complexes avec `[MemberData]`.

---

## Récapitulatif des 20 tests

| # | Catégorie | Nom court | Score attendu |
|---|-----------|-----------|---------------|
| 1 | Base | Win, Draw, Loss | 4 |
| 2 | Base | Win, Win | 6 |
| 3 | Base | Draw×3 | 3 |
| 4 | Base | Loss×2 | 0 |
| 5 | Série | Win×3 | 14 |
| 6 | Série | Win×4 | 17 |
| 7 | Série | Win,Win,Loss,Win | 6 |
| 8 | Série | Win×3,Loss,Win×4 | 26 |
| 9 | Série | Win,Draw,Win,Win | 10 |
| 10 | Disqual. | Win×3 + DQ | 0 |
| 11 | Disqual. | Aucun + DQ | 0 |
| 12 | Pénalités | Score 10, Pén. 3 | 7 |
| 13 | Pénalités | Score 5, Pén. 8 | 0 |
| 14 | Pénalités | Score 7, Pén. 7 | 0 |
| 15 | Limite | Liste vide | 0 |
| 16 | Limite | Null | ArgumentNullException |
| 17 | Limite | Pén. négatives | ArgumentException |
| 18 | Limite | 100 combats | 350 |
| 19 | Paramétré | InlineData ×6 | Varié |
| 20 | Paramétré | MemberData ×5 | Varié |

---

## Tests du service avec Moq (TournamentServiceTests)

Ces tests isolent `TournamentService` de ses dépendances (repository, calculateur) via Moq.

### Dépendances à mocker

```csharp
private readonly Mock<IScoreCalculator> _calculatorMock;
private readonly Mock<IParticipantRepository> _repositoryMock;
private readonly TournamentService _service;

public TournamentServiceTests()
{
    _calculatorMock = new Mock<IScoreCalculator>();
    _repositoryMock = new Mock<IParticipantRepository>();
    _service = new TournamentService(_calculatorMock.Object, _repositoryMock.Object);
}
```

### Tests à implémenter

| # | Nom du test | Comportement mocké | Vérification |
|---|-------------|-------------------|--------------|
| S1 | `GetChampion_WithMultipleParticipants_ReturnsHighestScorer` | Calculator retourne des scores connus | Champion = joueur avec score max |
| S2 | `GetChampion_WhenRepositoryEmpty_ThrowsInvalidOperationException` | Repository retourne liste vide | Exception levée |
| S3 | `RegisterFightResult_CallsRepositorySave` | — | `Verify` que Save est appelé 1× |
| S4 | `GetChampion_WhenAllDisqualified_ThrowsOrReturnsNull` | Tous DQ → score 0 | Comportement défini |

**Exemple de vérification Moq :**
```csharp
[Fact]
public void RegisterFightResult_CallsRepositorySave()
{
    // Arrange
    var participantId = Guid.NewGuid();
    var result = FightResult.Win;

    // Act
    _service.RegisterFightResult(participantId, result);

    // Assert
    _repositoryMock.Verify(
        r => r.Save(It.Is<Participant>(p => p.Id == participantId)),
        Times.Once);
}
```

---

## Configuration GitHub Actions

```yaml
# .github/workflows/tests.yml
name: Tests & Coverage

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '9.0.x'

      - name: Restore
        run: dotnet restore

      - name: Build
        run: dotnet build --no-restore

      - name: Test with coverage
        run: |
          dotnet test --no-build \
            --collect:"XPlat Code Coverage" \
            --results-directory ./coverage

      - name: Upload coverage report
        uses: codecov/codecov-action@v4
        with:
          directory: ./coverage
          fail_ci_if_error: true
```

**Seuil de couverture recommandé :** ≥ 80% sur `ScoreCalculator` et `TournamentService`.

---

## Checklist finale

- [ ] Tous les 20 tests de `ScoreCalculatorTests` implémentés et verts
- [ ] Tests Moq pour `TournamentServiceTests`
- [ ] Données paramétrées dans `ScoreCalculatorTheoryData`
- [ ] Nommage cohérent `Méthode_Scénario_Résultat`
- [ ] Chaque test a Arrange / Act / Assert commentés
- [ ] FluentAssertions utilisé partout (pas de `Assert.Equal`)
- [ ] Pipeline GitHub Actions fonctionnel
- [ ] Couverture de code ≥ 80%
