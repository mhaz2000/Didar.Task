using Didar.Task.Domain.Enums;

namespace Didar.Task.Application.DTOs;

public sealed record ExpertDto(
    Guid Id,
    string Name,
    bool IsActive,
    int Capacity,
    int OpenRequests,
    IReadOnlyCollection<Specialty> Specialties);