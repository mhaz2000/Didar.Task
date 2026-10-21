using Didar.Task.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Didar.Task.Application.DTOs;

public sealed record CreateRequestDto(
    [param: Required]
    [param: StringLength(100, MinimumLength = 1)]
    string RequestKey,

    [param: EnumDataType(typeof(Specialty))]
    Specialty Subject,

    [param: EnumDataType(typeof(RequestPriority))]
    RequestPriority Priority);
