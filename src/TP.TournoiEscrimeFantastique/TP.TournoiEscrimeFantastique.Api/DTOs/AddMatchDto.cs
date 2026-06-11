using System.ComponentModel.DataAnnotations;

namespace TP.TournoiEscrimeFantastique.Api.DTOs;

public record AddMatchDto(
    [Required] string Outcome
);
