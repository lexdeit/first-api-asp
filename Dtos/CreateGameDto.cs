﻿using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class CreateGameDto(
    int Id,
    [Required][StringLength(50)] string Name,
    int GenreId,
    [Range(1, 5000)] decimal Price,
    DateOnly ReleaseDate
);
