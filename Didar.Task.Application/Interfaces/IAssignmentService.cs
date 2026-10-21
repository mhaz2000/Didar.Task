using Didar.Task.Application.DTOs;

namespace Didar.Task.Application.Interfaces;

public interface IAssignmentService
{
    Task<RequestDto> CreateRequestAsync(
        CreateRequestDto request,
        CancellationToken cancellationToken = default);

    Task<RequestDto> CloseRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task ProcessWaitingRequestsAsync(
        CancellationToken cancellationToken = default);

    Task<RequestDto?> GetRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RequestDto>> GetWaitingRequestsAsync(
        CancellationToken cancellationToken = default);
}
