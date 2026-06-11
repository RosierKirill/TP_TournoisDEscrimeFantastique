# Présentation CI/CD — Tournoi d'Escrime Fantastique

---

## 1. Contexte du projet

Nous développons le backend d'un jeu de rôle fantastique : des chevaliers s'affrontent en tournois d'escrime. Chaque duel produit un résultat (`Win`, `Draw`, `Loss`) et le système calcule automatiquement le score de chaque participant selon des règles précises, puis établit le classement.

Le dépôt est organisé en **trois couches** :

| Couche                      | Dossier                                     | Rôle                                                          |
| --------------------------- | ------------------------------------------- | ------------------------------------------------------------- |
| **Domaine** (règles métier) | `src/.../TP.TournoiEscrimeFantastique/`     | Calcul de score, classement, modèles `Player` / `MatchResult` |
| **API REST**                | `src/.../TP.TournoiEscrimeFantastique.Api/` | HTTP, persistance SQLite (EF Core), duels, notifications      |
| **Application web**         | `web/`                                      | Interface Next.js consommant l'API                            |

La **CI** (GitHub Actions) compile la solution .NET, exécute les tests unitaires et vérifie un seuil de couverture sur le cœur métier.

---

## 2. Pipeline CI/CD

Fichier : [`.github/workflows/tests.yml`](../.github/workflows/tests.yml)

### Déclencheurs

- Chaque `push` sur le dépôt
- Chaque `pull_request`

### Étapes du job `test`

| Étape             | Action                                                                               |
| ----------------- | ------------------------------------------------------------------------------------ |
| Checkout          | Récupération du code source                                                          |
| Setup .NET        | Installation des SDK 9.0 et 10.0                                                     |
| Restore           | `dotnet restore TP.TournoiEscrimeFantastique.slnx`                                   |
| Build             | Compilation en configuration **Release**                                             |
| Test + couverture | `dotnet test` avec collecteur **XPlat Code Coverage** (coverlet → rapport Cobertura) |
| **Quality gate**  | Échec de la CI si la couverture de `ScoreCalculator` passe **sous 95 %**             |
| Artefacts         | Publication du rapport `coverage.cobertura.xml`                                      |
| Codecov           | Envoi optionnel du rapport (ne bloque pas la CI en cas d'erreur)                     |

### Ce que la CI garantit aujourd'hui

- Le projet **compile** (domaine + API + tests)
- Les **42 tests unitaires** passent
- La logique de **`ScoreCalculator`** reste couverte à au moins **95 %**

### Ce que la CI ne fait pas encore

- Pas de tests d'intégration HTTP sur l'API
- Pas de tests sur l'application web Next.js
- Pas de déploiement automatique (build + test uniquement)

---

## 3. Structure des tests

```
tests/
└── TP.TournoiEscrimeFantastique.Tests/
    ├── Unit/                              # Tests unitaires xUnit
    │   ├── ScoreCalculatorBaseTests.cs           # Calcul de base
    │   ├── ScoreCalculatorSeriesBonusTests.cs    # Bonus de série
    │   ├── ScoreCalculatorDisqualificationTests.cs
    │   ├── ScoreCalculatorPenaltyTests.cs
    │   ├── ScoreCalculatorEdgeCaseTests.cs       # Cas limites & exceptions
    │   ├── ScoreCalculatorParameterizedTests.cs  # [Theory] paramétrés
    │   ├── TournamentRankingTests.cs             # Classement & champion
    │   └── TournamentRankingMoqTests.cs          # Isolation avec Moq
    ├── TestData/
    │   └── ScoreCalculatorTheoryData.cs     # Jeux de données [MemberData]
    └── TP.TournoiEscrimeFantastique.Tests.csproj
```

**Outiling :** xUnit 2.9, FluentAssertions 8.2, Moq 4.20, coverlet.collector.

Le projet de tests référence **uniquement la bibliothèque domaine** (`TP.TournoiEscrimeFantastique`), pas l'API. Les tests valident les règles métier en isolation, sans serveur HTTP ni base de données.

---

## 4. Fonctionnalités testées (couche domaine)

### 4.1 `ScoreCalculator` — calcul du score

Classe testée : `src/.../TP.TournoiEscrimeFantastique/ScoreCalculator.cs`  
Interface : `IScoreCalculator.CalculateScore(matches, isDisqualified, penaltyPoints)`

| Fichier de test                        | Scénarios couverts                                                              | Nb tests |
| -------------------------------------- | ------------------------------------------------------------------------------- | :------: |
| `ScoreCalculatorBaseTests`             | Win/Draw/Loss, totaux simples (4, 6, 3, 0 pts)                                  |    4     |
| `ScoreCalculatorSeriesBonusTests`      | Bonus +5 pour 3 victoires consécutives ; série brisée ; doubles séries (31 pts) |    5     |
| `ScoreCalculatorDisqualificationTests` | Disqualification → score 0 (avec ou sans combats)                               |    2     |
| `ScoreCalculatorPenaltyTests`          | Soustraction de pénalités ; plancher à 0                                        |    3     |
| `ScoreCalculatorEdgeCaseTests`         | Liste vide ; `null` → exception ; pénalité négative ; 100 combats               |    4     |
| `ScoreCalculatorParameterizedTests`    | 6 cas `[InlineData]` + 5 cas `[MemberData]` complexes                           |    11    |

**Règles métier vérifiées :**

| Règle                                       | Points                      |
| ------------------------------------------- | --------------------------- |
| Victoire                                    | +3                          |
| Match nul                                   | +1                          |
| Défaite                                     | 0                           |
| Bonus de série (≥ 3 victoires consécutives) | +5 (une fois par série)     |
| Disqualification                            | score = 0                   |
| Pénalités                                   | `Max(0, score − pénalités)` |

**Exceptions testées :**

- `ArgumentNullException` si la liste de matchs est `null`
- `ArgumentException` si les pénalités sont négatives

### 4.2 `TournamentRanking` — classement et champion

Classe testée : `src/.../TP.TournoiEscrimeFantastique/TournamentRanking.cs`

| Fichier de test             | Scénarios couverts                                                                                           | Nb tests |
| --------------------------- | ------------------------------------------------------------------------------------------------------------ | :------: |
| `TournamentRankingTests`    | Tri décroissant ; ex æquo (ordre d'entrée) ; champion ; liste vide ; tous DQ ; `null` → exception            |    8     |
| `TournamentRankingMoqTests` | Tri avec scores mockés ; appels à `IScoreCalculator` ; transmission DQ/pénalités ; champion score 0 → `null` |    5     |

**Isolation Moq :** dans `TournamentRankingMoqTests`, `IScoreCalculator` est remplacé par un mock pour tester **uniquement** la logique de classement, sans dépendre des règles de calcul.

### 4.3 Synthèse des tests

| Indicateur                               | Valeur                           |
| ---------------------------------------- | -------------------------------- |
| Total de tests exécutés                  | **42**                           |
| Méthodes de test (`[Fact]` / `[Theory]`) | 33                               |
| Couverture `ScoreCalculator`             | **100 %** (lignes & branches)    |
| Couverture `TournamentRanking`           | **100 %** (lignes & branches)    |
| Seuil CI                                 | ≥ **95 %** sur `ScoreCalculator` |

---

## 5. Fonctionnalités non couvertes par les tests automatisés

| Composant                                             | Raison                                                         |
| ----------------------------------------------------- | -------------------------------------------------------------- |
| `TP.TournoiEscrimeFantastique.Api` (contrôleurs HTTP) | Pas de projet de tests d'intégration (`WebApplicationFactory`) |
| `DomainPlayerService`, `OutcomeMapper`                | Pas de tests unitaires dédiés                                  |
| `web/` (Next.js)                                      | Pas de Jest / Playwright configuré                             |
| Duels aléatoires, notifications                       | Logique API non testée automatiquement                         |

La couche API **réutilise** le domaine testé ; les règles de score sont donc validées indirectement, mais **pas** le câblage HTTP ↔ base de données ↔ JSON.

---

## 6. Comment l'API utilise la couche domaine

### 6.1 Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  Application web (Next.js)                                  │
│  web/src/lib/api.ts  →  appels HTTP                         │
└──────────────────────────┬──────────────────────────────────┘
                           │ REST (JSON)
┌──────────────────────────▼──────────────────────────────────┐
│  API (TP.TournoiEscrimeFantastique.Api)                     │
│  Controllers → DomainPlayerService → Domaine                │
│  SQLite (PlayerEntity, MatchEntity)                         │
└──────────────────────────┬──────────────────────────────────┘
                           │ ProjectReference
┌──────────────────────────▼──────────────────────────────────┐
│  Domaine (TP.TournoiEscrimeFantastique)  ← TESTÉ PAR LA CI  │
│  IScoreCalculator / ScoreCalculator                         │
│  TournamentRanking                                          │
│  Player / MatchResult                                       │
└─────────────────────────────────────────────────────────────┘
```

### 6.2 Injection des services (Program.cs)

```csharp
builder.Services.AddScoped<IScoreCalculator, ScoreCalculator>();
builder.Services.AddScoped<TournamentRanking>();
builder.Services.AddScoped<DomainPlayerService>();
```

### 6.3 Pont API ↔ domaine : `DomainPlayerService`

Fichier : `src/.../Api/Services/DomainPlayerService.cs`

Ce service évite de dupliquer la logique métier dans les contrôleurs :

1. **`PlayerEntityMapper`** convertit les entités SQLite (`PlayerEntity`, `MatchEntity`) en objets domaine `Player` + `MatchResult`.
2. **`IScoreCalculator.CalculateScore()`** calcule le score d'un joueur.
3. **`TournamentRanking.GetRanking()`** / **`GetChampion()`** établissent le classement.

| Méthode API                | Utilisation du domaine                    |
| -------------------------- | ----------------------------------------- |
| `GetScore(entity)`         | `PlayerEntityMapper` → `IScoreCalculator` |
| `ToDto(entity)`            | Score via domaine → `PlayerDto` JSON      |
| `GetRankingDtos(entities)` | `TournamentRanking.GetRanking()`          |
| `GetChampionDto(entities)` | `TournamentRanking.GetChampion()`         |

### 6.4 Contrôleurs et endpoints

| Contrôleur                | Endpoints principaux                            | Domaine utilisé                               |
| ------------------------- | ----------------------------------------------- | --------------------------------------------- |
| `PlayersController`       | CRUD joueurs, ajout/suppression de matchs       | `DomainPlayerService.ToDto()` → score calculé |
| `RankingController`       | `GET /api/ranking`, `GET /api/ranking/champion` | `GetRankingDtos()`, `GetChampionDto()`        |
| `DuelsController`         | `POST /api/duels`                               | `GetScore()` après ajout des matchs           |
| `NotificationsController` | `POST /api/notifications/broadcast-ranking`     | `GetRankingDtos()` pour les messages          |

**Exemple de flux** — ajout d'un match :

```
Client  POST /api/players/1/matches  { "outcome": "Win" }
   →  PlayersController enregistre "Win" en base (MatchEntity)
   →  DomainPlayerService.ToDto()
        →  PlayerEntityMapper.ToDomain()  →  Player + List<MatchResult>
        →  ScoreCalculator.CalculateScore()  →  score (ex. 7)
   →  Réponse JSON  { "score": 7, "matches": [...] }
```

### 6.5 Séparation des responsabilités

| Élément                                             | Couche  | Testé en CI ? |
| --------------------------------------------------- | ------- | :-----------: |
| Règles de calcul (`ScoreCalculator`)                | Domaine |      ✅       |
| Classement (`TournamentRanking`)                    | Domaine |      ✅       |
| Persistance (`PlayerEntity`, EF Core)               | API     |      ❌       |
| Conversion string → `MatchResult` (`OutcomeMapper`) | API     |      ❌       |
| Exposition HTTP (contrôleurs, DTOs)                 | API     |      ❌       |
| Interface utilisateur                               | Web     |      ❌       |

---

## 7. Conventions de test (bonnes pratiques)

Document détaillé : [`BONNES_PRATIQUES.md`](BONNES_PRATIQUES.md)

| Pratique                                   | Application                                                   |
| ------------------------------------------ | ------------------------------------------------------------- |
| Structure **AAA** (Arrange / Act / Assert) | Commentaires dans la majorité des tests                       |
| Nommage `Méthode_Scénario_RésultatAttendu` | Ex. `CalculateScore_WithThreeConsecutiveWins_Returns14Points` |
| **FluentAssertions**                       | `Should().Be()`, `Should().Throw()`, `because:`               |
| **Un concept par test**                    | Fichiers séparés par catégorie métier                         |
| **Tests paramétrés**                       | `[Theory]` + `[InlineData]` / `[MemberData]`                  |
| **Isolation**                              | Moq sur `IScoreCalculator` pour `TournamentRanking`           |

---

## 8. Commandes utiles (démonstration locale)

```bash
# Compiler toute la solution (domaine + API + tests)
dotnet build TP.TournoiEscrimeFantastique.slnx --configuration Release

# Lancer les 42 tests
dotnet test TP.TournoiEscrimeFantastique.slnx --configuration Release

# Tests avec rapport de couverture (comme en CI)
dotnet test TP.TournoiEscrimeFantastique.slnx \
  --configuration Release \
  --collect:"XPlat Code Coverage" \
  --results-directory ./coverage
```

---

## 9. Documents complémentaires

| Document                                     | Contenu                                    |
| -------------------------------------------- | ------------------------------------------ |
| [`PLAN_DE_TESTS.md`](PLAN_DE_TESTS.md)       | Plan détaillé des scénarios à couvrir      |
| [`RAPPORT_DE_TESTS.md`](RAPPORT_DE_TESTS.md) | Synthèse d'exécution et couverture         |
| [`BONNES_PRATIQUES.md`](BONNES_PRATIQUES.md) | Conventions xUnit / FluentAssertions / Moq |
| [`README.md`](../README.md)                  | Vue d'ensemble du projet et démarrage      |
