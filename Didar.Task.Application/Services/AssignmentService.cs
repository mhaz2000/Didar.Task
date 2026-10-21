using Didar.Task.Application.DTOs;
using Didar.Task.Application.Interfaces;
using Didar.Task.Domain.Entities;

namespace Didar.Task.Application.Services;

public sealed class AssignmentService(
    IRequestRepository requestRepository,
    IAssignmentRepository assignmentRepository)
    : IAssignmentService
{
    public async Task<RequestDto> CreateRequestAsync(
        CreateRequestDto dto,
        CancellationToken cancellationToken = default)
    {
        var request = new Request(
            dto.RequestKey,
            dto.Subject,
            dto.Priority);

        var savedRequest =
            await requestRepository.CreateOrGetByKeyAsync(
                request,
                cancellationToken);

        if (savedRequest.Subject != dto.Subject ||
            savedRequest.Priority != dto.Priority)
        {
            throw new InvalidOperationException(
                "RequestKey already exists with different request data.");
        }

        if (savedRequest.Status ==
            Domain.Enums.RequestStatus.Waiting)
        {
            await assignmentRepository.TryAssignAsync(
                savedRequest.Id,
                cancellationToken);
        }

        var result = await requestRepository.GetByIdAsync(
            savedRequest.Id,
            cancellationToken);

        return Map(result ?? savedRequest);
    }

    public async Task<RequestDto> CloseRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var request = await requestRepository.GetByIdAsync(
            requestId,
            cancellationToken);

        if (request is null)
            throw new KeyNotFoundException("Request not found.");

        request.Close();

        await requestRepository.SaveChangesAsync(cancellationToken);

        await ProcessWaitingRequestsAsync(cancellationToken);

        return Map(request);
    }

    public async System.Threading.Tasks.Task ProcessWaitingRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var waitingRequests =
            await requestRepository.GetWaitingRequestsAsync(
                cancellationToken);

        foreach (var request in waitingRequests)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await assignmentRepository.TryAssignAsync(
                request.Id,
                cancellationToken);
        }
    }


    public async Task<RequestDto?> GetRequestAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        var request = await requestRepository.GetByIdAsync(
            requestId,
            cancellationToken);

        return request is null ? null : Map(request);
    }

    public async Task<IReadOnlyList<RequestDto>> GetWaitingRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var requests = await requestRepository.GetWaitingRequestsAsync(
            cancellationToken);

        return requests.Select(Map).ToList();
    }

    private static RequestDto Map(Request request) =>
        new(
            request.Id,
            request.RequestKey,
            request.Subject,
            request.Priority,
            request.Status,
            request.AssignedExpertId,
            request.CreatedAt);
}