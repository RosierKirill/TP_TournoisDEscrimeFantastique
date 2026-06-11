using System.ComponentModel.DataAnnotations;

namespace TP.TournoiEscrimeFantastique.Api.Data.Entities;

public class PlayerEntity
{
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = "";

    public bool IsDisqualified { get; set; }

    public int PenaltyPoints { get; set; }

    public List<MatchEntity> Matches { get; set; } = [];
}
