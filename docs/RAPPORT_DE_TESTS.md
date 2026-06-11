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
