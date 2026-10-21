using Didar.Task.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Didar.Task.Application.DTOs;

public sealed record CreateExpertDto(
    [param: Required]
    [param: StringLength(200, MinimumLength = 1)]
    string Name,

    [param: Range(1, int.MaxValue)]
    int Capacity,

    [param: Required]
    [param: MinLength(1)]
    IReadOnlyCollection<Specialty> Specialties);
