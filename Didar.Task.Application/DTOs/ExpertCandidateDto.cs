namespace Didar.Task.Application.DTOs;

public sealed record ExpertCandidateDto(
    Guid ExpertId,
    int Capacity,
    int OpenRequests);