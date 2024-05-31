﻿using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{

    const string GetGameEndpointName = "GetGame";

    private static readonly List<GameSummaryDto> games = [
    new (
    1,
    "Street Fighter 2",
    "Fighting",
    19.99M,
    new DateOnly(1992, 7, 15)
),
new (
    2,
    "Minecraft",
    "Fighting",
    19.99M,
    new DateOnly(1992, 7, 15)
),
new (
    3,
    "Fortnite",
    "Shooter",
    19.99M,
    new DateOnly(1992, 7, 15)
),
];

    public static RouteGroupBuilder MapGamesEndpoints(this WebApplication app)
    {

        var group = app.MapGroup("games")
                        .WithParameterValidation();

        group.MapGet("/", (GameStoreContext dbContext) =>
        {
            return dbContext
            .Games
            .Include(game => game.Genre)
            .Select(game => game.ToGameSummaryDto())
            .AsNoTracking();
        });

        group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
        {

            Game? game = dbContext.Games.Find(id);

            return game is null ? Results.NotFound() : Results.Ok(game.ToGameDetailsDto());

        }).WithName(GetGameEndpointName);

        group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = newGame.ToEntity();

            dbContext.Games.Add(game);
            dbContext.SaveChanges();



            return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game.ToGameDetailsDto());
        });

        group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = dbContext.Games.Find(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            dbContext.Entry(existingGame)
            .CurrentValues
            .SetValues(updatedGame.ToEntity(id));
            dbContext.SaveChanges();

            return Results.NoContent();
        });

        group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
        {

            dbContext.Games
            .Where(game => game.Id == id)
            .ExecuteDelete();

            return Results.NoContent();

        });

        return group;
    }


}
