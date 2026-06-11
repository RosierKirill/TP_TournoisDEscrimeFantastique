using System.Collections.Concurrent;

namespace TP.TournoiEscrimeFantastique.Api.Notifications;

/// <summary>
/// Implémentation simple : journalise la notification et la conserve en mémoire.
/// Dans un vrai système, on enverrait un e-mail / push / SignalR ici.
/// </summary>
public class InMemoryNotificationService(ILogger<InMemoryNotificationService> logger) : INotificationService
{
    private const int MaxHistory = 100;
    private readonly ConcurrentQueue<Notification> _history = new();

    public Notification Notify(string playerName, string message)
    {
        var notification = new Notification(playerName, message, DateTimeOffset.UtcNow);

        logger.LogInformation("Notification → {Player} : {Message}", playerName, message);

        _history.Enqueue(notification);
        while (_history.Count > MaxHistory && _history.TryDequeue(out _)) { }

        return notification;
    }

    public IReadOnlyList<Notification> GetHistory() =>
        _history.Reverse().ToList();
}
