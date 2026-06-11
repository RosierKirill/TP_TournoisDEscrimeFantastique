# Tournoi d'Escrime Fantastique — Système de notation

Backend de notation pour un jeu de rôle fantastique : des héros s'affrontent en duels
(victoire / nul / défaite) et le système calcule automatiquement leur score selon des
règles précises, puis établit le classement du tournoi.

[![Tests & Coverage](https://github.com/RosierKirill/TP_TournoisDEscrimeFantastique/actions/workflows/tests.yml/badge.svg)](https://github.com/RosierKirill/TP_TournoisDEscrimeFantastique/actions/workflows/tests.yml)

[![codecov](https://codecov.io/gh/RosierKirill/TP_TournoisDEscrimeFantastique/graph/
badge.svg)](https://codecov.io/gh/RosierKirill/TP_TournoisDEscrimeFantastique)

## Règles de notation

| Règle            | Effet                                                                            |
| ---------------- | -------------------------------------------------------------------------------- |
| Victoire         | +3 points                                                                        |
| Match nul        | +1 point                                                                         |
| Défaite          | 0 point                                                                          |
| Bonus de série   | +5 points par série d'au moins 3 victoires consécutives (une fois par série)     |
| Disqualification | score total = 0, quelles que soient les performances                             |
| Pénalités        | soustraites du score, sans jamais descendre sous 0 (`Max(0, score - pénalités)`) |

**Exemples :**

- `Win, Draw, Loss, Win` → 3+1+0+3 = **7**
- `Win, Win, Win, Draw` → 9+1 +5 bonus = **15**
- `Win×3, Loss, Win×4` → 21 +5 +5 = **31** (deux séries)
- Score 8, pénalités 10 → **0** (jamais négatif)

## Pile technique

- **.NET 9** (C#, nullable activé)
- **xUnit** 2.9 — framework de tests
- **FluentAssertions** 8.2 — assertions lisibles
- **Moq** 4.20 — isolation des dépendances
- **coverlet** — couverture de code
- **GitHub Actions** — CI (build, tests, gate de couverture)

## Structure du dépôt

```
src/TP.TournoiEscrimeFantastique/.../
├── MatchResult.cs          # enum Win / Draw / Loss
├── ScoreCalculator.cs      # calcul du score (règles métier)
├── IScoreCalculator.cs     # abstraction (injectable / mockable)
├── Player.cs               # joueur : nom, matchs, disqualification, pénalités
└── TournamentRanking.cs    # classement (GetRanking) et champion (GetChampion)

tests/TP.TournoiEscrimeFantastique.Tests/
├── Unit/                   # tests unitaires (par catégorie)
└── TestData/               # données paramétrées (MemberData)

docs/
├── PLAN_DE_TESTS.md        # plan de tests
├── RAPPORT_DE_TESTS.md     # rapport d'exécution & couverture
└── BONNES_PRATIQUES.md     # conventions de test du projet
```

## Démarrage

Prérequis : [SDK .NET 9](https://dotnet.microsoft.com/download) (ou supérieur).

```bash
# Restaurer et compiler
dotnet build TP.TournoiEscrimeFantastique.slnx

# Lancer les tests
dotnet test TP.TournoiEscrimeFantastique.slnx

# Tests + couverture de code
dotnet test TP.TournoiEscrimeFantastique.slnx \
  --collect:"XPlat Code Coverage" --results-directory ./coverage
```

## Tests & couverture

- **42 tests** verts, organisés par catégorie (calcul de base, bonus de série,
  disqualification, pénalités, cas limites, tests paramétrés) + classement et
  isolation Moq.
- Couverture **100 %** sur `ScoreCalculator` et `TournamentRanking`.
- La CI échoue si la couverture de `ScoreCalculator` passe sous **95 %**.

Voir [docs/RAPPORT_DE_TESTS.md](docs/RAPPORT_DE_TESTS.md) pour le détail.

## Intégration continue

Le workflow [`.github/workflows/tests.yml`](.github/workflows/tests.yml) s'exécute à
chaque `push` et `pull_request` : restauration → build Release → tests avec couverture
→ contrôle du seuil ≥ 95 % → publication du rapport en artefact.

