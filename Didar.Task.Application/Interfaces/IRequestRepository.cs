using Didar.Task.Domain.Entities;

namespace Didar.Task.Application.Interfaces;

public interface IRequestRepository
{
    Task<Request?> GetByRequestKeyAsync(
        string requestKey,
        CancellationToken cancellationToken = default);

    Task<Request> CreateOrGetByKeyAsync(
        Request request,
        CancellationToken cancellationToken = default);

    Task<Request?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Request>> GetWaitingRequestsAsync(
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task AddAsync(
        Request request,
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}
