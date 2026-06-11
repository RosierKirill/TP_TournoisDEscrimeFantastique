# Rapport de tests — Tournoi d'Escrime Fantastique

Ce rapport synthétise l'exécution de la suite de tests et la couverture de code.
Le plan détaillé des scénarios est dans [PLAN_DE_TESTS.md](PLAN_DE_TESTS.md).

## Synthèse

| Indicateur | Valeur |
|------------|--------|
| Total de tests | **42** ✅ |
| Échecs | 0 |
| Cible (`net9.0`) | xUnit 2.9 / FluentAssertions 8.2 / Moq 4.20 |
| Couverture `ScoreCalculator` | **100 %** (lignes & branches) |
| Couverture `TournamentRanking` | **100 %** (lignes & branches) |
| Seuil CI | échec si `ScoreCalculator` < 95 % |

## Répartition par fichier

| Fichier de test | Méthodes | Portée |
|-----------------|:--------:|--------|
| `ScoreCalculatorBaseTests` | 4 | Calcul de base (Win/Draw/Loss) |
| `ScoreCalculatorSeriesBonusTests` | 5 | Bonus de série (+5 pour 3 victoires consécutives) |
| `ScoreCalculatorDisqualificationTests` | 2 | Disqualification → 0 |
| `ScoreCalculatorPenaltyTests` | 3 | Pénalités et plancher à 0 |
| `ScoreCalculatorEdgeCaseTests` | 4 | Liste vide, null, pénalité négative, tournoi long |
| `ScoreCalculatorParameterizedTests` | 2 | `[Theory]` InlineData (6 cas) + MemberData (5 cas) |
| `TournamentRankingTests` | 8 | Classement décroissant, égalités, champion, tous DQ, gardes |
| `TournamentRankingMoqTests` | 5 | Isolation de `IScoreCalculator` avec Moq |

> 33 méthodes de test ; les `[Theory]` étendent les cas paramétrés pour un total de **42 tests exécutés**.

## Détail par test

Durée par méthode de test ; les `[Theory]` cumulent leurs cas paramétrés.

**Méthodologie de mesure.** Chaque durée est le **minimum observé sur 8 exécutions
séquentielles** (`-c Release`, parallélisme désactivé via runsettings). Prendre le minimum
sur plusieurs exécutions élimine le bruit de démarrage (JIT, chargement d'assembly), qui
ne fait pas partie du temps réel d'un test. Les deux valeurs marquées **‡** restent
élevées car elles paient l'**initialisation unique d'une librairie** au premier appel
(génération du proxy Castle pour Moq, première assertion d'exception FluentAssertions) :
c'est un coût de premier usage du processus, pas le coût du test.

| Fichier | Test | Résultat | Durée (ms) |
|---------|------|:--------:|-----------:|
| `ScoreCalculatorBaseTests` | `CalculateScore_WithWinDrawLoss_Returns4Points` | ✅ | 0,16 |
| `ScoreCalculatorBaseTests` | `CalculateScore_WithTwoWins_Returns6Points` | ✅ | 0,15 |
| `ScoreCalculatorBaseTests` | `CalculateScore_WithThreeDraws_Returns3Points` | ✅ | 0,24 |
| `ScoreCalculatorBaseTests` | `CalculateScore_WithTwoLosses_Returns0Points` | ✅ | 0,14 |
| `ScoreCalculatorSeriesBonusTests` | `CalculateScore_WithThreeConsecutiveWins_Returns14Points` | ✅ | 0,16 |
| `ScoreCalculatorSeriesBonusTests` | `CalculateScore_WithFourConsecutiveWins_Returns17Points` | ✅ | 0,15 |
| `ScoreCalculatorSeriesBonusTests` | `CalculateScore_WithWinWinLossWin_Returns9Points` | ✅ | 0,15 |
| `ScoreCalculatorSeriesBonusTests` | `CalculateScore_WithMultipleBonusSeries_Returns31Points` | ✅ | 0,37 |
| `ScoreCalculatorSeriesBonusTests` | `CalculateScore_WithWinDrawWinWin_Returns10Points` | ✅ | 0,21 |
| `ScoreCalculatorDisqualificationTests` | `CalculateScore_WhenDisqualifiedWithPositiveScore_Returns0` | ✅ | 0,25 |
| `ScoreCalculatorDisqualificationTests` | `CalculateScore_WhenDisqualifiedWithNoFights_Returns0` | ✅ | 0,12 |
| `ScoreCalculatorPenaltyTests` | `CalculateScore_WithNormalPenalties_Returns7Points` | ✅ | 0,18 |
| `ScoreCalculatorPenaltyTests` | `CalculateScore_WithPenaltiesGreaterThanScore_Returns0` | ✅ | 0,17 |
| `ScoreCalculatorPenaltyTests` | `CalculateScore_WithEqualPenalties_Returns0` | ✅ | 0,25 |
| `ScoreCalculatorEdgeCaseTests` | `CalculateScore_WithEmptyList_Returns0` | ✅ | 0,15 |
| `ScoreCalculatorEdgeCaseTests` | `CalculateScore_WithNullList_ThrowsArgumentNullException` | ✅ | 15,82 ‡ |
| `ScoreCalculatorEdgeCaseTests` | `CalculateScore_WithNegativePenalties_ThrowsArgumentException` | ✅ | 1,06 |
| `ScoreCalculatorEdgeCaseTests` | `CalculateScore_WithHundredFights_ReturnsCorrectScore` | ✅ | 0,98 |
| `ScoreCalculatorParameterizedTests` | `CalculateScore_WithInlineData_ReturnsExpectedScore` (6 cas) | ✅ | 0,01 |
| `ScoreCalculatorParameterizedTests` | `CalculateScore_WithMemberData_ReturnsExpectedScore` (5 cas) | ✅ | 0,01 |
| `TournamentRankingTests` | `GetRanking_WithDistinctScores_OrdersPlayersByScoreDescending` | ✅ | 0,27 |
| `TournamentRankingTests` | `GetRanking_WithTiedScores_PreservesInputOrder` | ✅ | 0,39 |
| `TournamentRankingTests` | `GetRanking_WithNullPlayers_ThrowsArgumentNullException` | ✅ | 0,98 |
| `TournamentRankingTests` | `GetChampion_WithSeveralPlayers_ReturnsHighestScorer` | ✅ | 0,23 |
| `TournamentRankingTests` | `GetChampion_WhenAllPlayersDisqualified_ReturnsNull` | ✅ | 0,20 |
| `TournamentRankingTests` | `GetChampion_WithNullPlayers_ThrowsArgumentNullException` | ✅ | 0,48 |
| `TournamentRankingTests` | `GetRanking_WithEmptyList_ReturnsEmpty` | ✅ | 1,51 |
| `TournamentRankingTests` | `GetChampion_WithEmptyList_ReturnsNull` | ✅ | 0,11 |
| `TournamentRankingMoqTests` | `GetRanking_OrdersPlayersByScoresReturnedByCalculator` | ✅ | 4,14 |
| `TournamentRankingMoqTests` | `GetRanking_CallsCalculatorOncePerPlayer` | ✅ | 2,53 |
| `TournamentRankingMoqTests` | `GetRanking_ForwardsPlayerDisqualificationAndPenaltyToCalculator` | ✅ | 113,09 ‡ |
| `TournamentRankingMoqTests` | `GetChampion_ReturnsPlayerWithHighestMockedScore` | ✅ | 5,77 |
| `TournamentRankingMoqTests` | `GetChampion_WhenCalculatorReturnsAllZero_ReturnsNull` | ✅ | 0,90 |

‡ Coût d'initialisation unique de la librairie au premier usage (proxy Moq / FluentAssertions),
indépendant de la logique testée. Hors ces deux entrées, chaque test s'exécute en moins de 6 ms.

## Scénarios clés couverts

- **Calcul** : combinaisons Win/Draw/Loss, totaux corrects.
- **Bonus de série** : +5 une seule fois par série ; deux séries distinctes = deux bonus ;
  série brisée par Draw/Loss = pas de bonus ; `Win×3, Loss, Win×4` = 31.
- **Disqualification** : annule tout score (même sans combat).
- **Pénalités** : soustraction simple, pénalités ≥ score → 0, jamais négatif.
- **Cas limites** : liste vide → 0 ; `null` → `ArgumentNullException` ;
  pénalité négative → `ArgumentException` ; tournoi de 100 combats → 350.
- **Paramétrés** : `[InlineData]` (cas simples) et `[MemberData]` (cas complexes).
- **Classement** : tri décroissant, ordre d'entrée conservé pour les ex æquo,
  champion = meilleur score, `null` si liste vide ou tous disqualifiés.
- **Isolation** : `TournamentRanking` testé avec un mock de `IScoreCalculator`
  (`Setup`/`Returns`, `Verify`/`Times`).

## Reproduire localement

```bash
# Tous les tests
dotnet test TP.TournoiEscrimeFantastique.slnx

# Avec couverture (rapport Cobertura)
dotnet test TP.TournoiEscrimeFantastique.slnx \
  --collect:"XPlat Code Coverage" --results-directory ./coverage
```

Le rapport `coverage/**/coverage.cobertura.xml` contient le détail par classe.
La CI publie ce rapport en artefact à chaque exécution.
