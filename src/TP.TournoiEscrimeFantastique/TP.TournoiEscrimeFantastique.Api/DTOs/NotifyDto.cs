using System.ComponentModel.DataAnnotations;
using TP.TournoiEscrimeFantastique.Api.Notifications;

namespace TP.TournoiEscrimeFantastique.Api.DTOs;

/// <summary>Notification ciblée d'un participant.</summary>
public record NotifyDto(
    [Required] string Message
);

/// <summary>Une notification renvoyée au client.</summary>
public record NotificationDto(string PlayerName, string Message, DateTimeOffset SentAt)
{
    public static NotificationDto From(Notification n) => new(n.PlayerName, n.Message, n.SentAt);
}
