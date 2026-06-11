namespace TP.TournoiEscrimeFantastique;

public class ScoreCalculator : IScoreCalculator
{
    private const int WinPoints      = 3;
    private const int DrawPoints     = 1;
    private const int SeriesBonus    = 5;
    private const int SeriesThreshold = 3;

    public int CalculateScore(IList<MatchResult> matches, bool isDisqualified = false, int penaltyPoints = 0)
    {
        if (matches == null)    throw new ArgumentNullException(nameof(matches));
        if (penaltyPoints < 0) throw new ArgumentException("Les pénalités ne peuvent pas être négatives.", nameof(penaltyPoints));

        if (isDisqualified) return 0;

        int score = 0;
        int consecutiveWins = 0;

        foreach (var match in matches)
        {
            switch (match)
            {
                case MatchResult.Win:
                    score += WinPoints;
                    consecutiveWins++;
                    if (consecutiveWins == SeriesThreshold) score += SeriesBonus;
                    break;
                case MatchResult.Draw:
                    score += DrawPoints;
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
