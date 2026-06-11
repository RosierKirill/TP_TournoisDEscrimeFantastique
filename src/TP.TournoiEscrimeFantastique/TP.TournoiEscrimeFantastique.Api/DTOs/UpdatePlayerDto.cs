using System.ComponentModel.DataAnnotations;

namespace TP.TournoiEscrimeFantastique.Api.DTOs;

public record UpdatePlayerDto(
    [Required][MaxLength(100)] string Name,
    bool IsDisqualified,
    int PenaltyPoints
);
