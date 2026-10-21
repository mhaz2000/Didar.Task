using Didar.Task.Domain.Enums;

namespace Didar.Task.Application.DTOs;

public sealed record RequestDto(
    Guid Id,
    string RequestKey,
    Specialty Subject,
    RequestPriority Priority,
    RequestStatus Status,
    Guid? AssignedExpertId,
    DateTime CreatedAt);
