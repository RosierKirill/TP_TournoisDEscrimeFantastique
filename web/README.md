# Front — Tournoi d'Escrime Fantastique (Next.js)

Application web pour gérer les joueurs, saisir les duels, consulter le classement et
notifier les participants. Elle consomme l'API REST .NET du projet.

## Stack

- **Next.js 14** (App Router) + **TypeScript**
- **Tailwind CSS** + composants maison façon **shadcn/ui**
- **lucide-react** (icônes), thème *fantasy médiéval* (arène sombre, or & améthyste)

## Prérequis

- Node.js 18+ (testé sur Node 20)
- L'API .NET démarrée sur `http://localhost:5211`

## Démarrage

```bash
# 1. Démarrer l'API (depuis la racine du dépôt, dans un autre terminal)
dotnet run --project src/TP.TournoiEscrimeFantastique/TP.TournoiEscrimeFantastique.Api

# 2. Démarrer le front
cd web
npm install
npm run dev
```

Le front est accessible sur <http://localhost:3000>.

### Configurer l'URL de l'API

Par défaut le front pointe sur `http://localhost:5211`. Pour surcharger, créer un fichier
`web/.env.local` :

```env
NEXT_PUBLIC_API_URL=http://localhost:5211
```

> Le CORS de l'API autorise `http://localhost:3000` (voir `appsettings.json`,
> section `Cors:AllowedOrigins`).

## Fonctionnalités

- **Joueurs (`/`)** : enrôler, modifier (nom / pénalités), disqualifier / réintégrer,
  supprimer. Bouton « Peupler une démo » pour créer un tournoi d'exemple.
- **Combats** : ajouter des résultats *Victoire / Nul / Défaite* ; le score est recalculé
  par l'API (bonus de série, pénalités, disqualification). Cliquer un combat le retire.
- **Classement (`/ranking`)** : podium, classement décroissant, mise en avant du champion.
- **Notifications** : notifier un joueur, ou diffuser le classement final à tous les
  participants (bouton « Notifier tous »).

## Scripts

| Commande        | Effet                          |
| --------------- | ------------------------------ |
| `npm run dev`   | Serveur de développement       |
| `npm run build` | Build de production            |
| `npm run start` | Sert le build de production     |
| `npm run lint`  | Lint Next.js                   |

## Notes

- Version Next.js : `14.2.35` (dernier correctif de la branche 14). `npm audit` peut
  signaler des avis liés au **serveur Next.js auto-hébergé en production** (DoS, etc.) ;
  ils ne concernent pas une utilisation locale et ne sont corrigés qu'en migrant vers
  Next 16 (changement majeur).
- Sous Windows, le cache webpack sur disque est désactivé en dev (`next.config.mjs`)
  pour éviter les avertissements `ENOENT ...pack.gz`.
