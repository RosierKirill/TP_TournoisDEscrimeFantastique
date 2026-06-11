namespace TP.TournoiEscrimeFantastique.Api.Notifications;

/// <summary>Notifie les participants du tournoi (ici : journalisation + historique en mémoire).</summary>
public interface INotificationService
{
    /// <summary>Notifie un participant et conserve le message dans l'historique.</summary>
    Notification Notify(string playerName, string message);

    /// <summary>Retourne l'historique des notifications (les plus récentes d'abord).</summary>
    IReadOnlyList<Notification> GetHistory();
}

/// <summary>Une notification envoyée à un participant.</summary>
public record Notification(string PlayerName, string Message, DateTimeOffset SentAt);
