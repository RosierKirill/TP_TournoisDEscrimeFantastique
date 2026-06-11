namespace TP.TournoiEscrimeFantastique;

public class ScoreCalculator
{
    public int CalculateScore(IList<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0)
    {
        if (matches == null) throw new ArgumentNullException(nameof(matches));
        if (penaltyPoints < 0) throw new ArgumentException("Les pénalités ne peuvent pas être négatives.", nameof(penaltyPoints));

        if (isDisqualified) return 0;

        int score = 0;
        int consecutiveWins = 0;

        foreach (var match in matches)
        {
            switch (match)
            {
                case MatchResult.Win:
                    score += 3;
                    consecutiveWins++;
                    if (consecutiveWins == 3) score += 5;
                    break;
                case MatchResult.Draw:
                    score += 1;
                    consecutiveWins = 0;
                    break;
                case MatchResult.Loss:
                    consecutiveWins = 0;
                    break;
            }
        }

        return Math.Max(0, score - penaltyPoints);
    }
}
