using System.ComponentModel.DataAnnotations;

namespace TP.TournoiEscrimeFantastique.Api.DTOs;

public record CreatePlayerDto(
    [Required][MaxLength(100)] string Name,
    bool IsDisqualified = false,
    int PenaltyPoints = 0
);
