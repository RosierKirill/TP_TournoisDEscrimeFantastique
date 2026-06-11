using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using TP.TournoiEscrimeFantastique.Api.Data;
using DomainScoreCalculator = TP.TournoiEscrimeFantastique.ScoreCalculator;
using IFightScoreCalculator = TP.TournoiEscrimeFantastique.IScoreCalculator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((doc, _, _) =>
    {
        doc.Info.Title = "Tournoi d'Escrime Fantastique — API";
        doc.Info.Version = "v1";
        doc.Info.Description = """
            API REST pour gérer les joueurs et les combats du tournoi d'escrime fantastique.

            ### Valeurs acceptées pour `outcome`
            - `Win` — victoire (+3 pts)
            - `Draw` — match nul (+1 pt)
            - `Loss` — défaite (0 pt)
            """;
        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<TournamentDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tournament.db"));

builder.Services.AddScoped<IFightScoreCalculator, DomainScoreCalculator>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TournamentDbContext>();
    db.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference(opts =>
{
    opts.Title = "Tournoi d'Escrime Fantastique";
    opts.Theme = ScalarTheme.Saturn;
    opts.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
});

app.UseAuthorization();
app.MapControllers();
app.Run();
