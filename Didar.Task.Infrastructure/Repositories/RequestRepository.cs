using Didar.Task.Application.Interfaces;
using Didar.Task.Domain.Entities;
using Didar.Task.Domain.Enums;
using Didar.Task.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Didar.Task.Infrastructure.Repositories;

public sealed class RequestRepository(
    AppDbContext dbContext) : IRequestRepository
{
    public Task<Request?> GetByRequestKeyAsync(
        string requestKey,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Requests
            .FirstOrDefaultAsync(
                x => x.RequestKey == requestKey,
                cancellationToken);
    }

    public Task<Request?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Requests
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task<IReadOnlyList<Request>> GetWaitingRequestsAsync(
        CancellationToken cancellationToken = default)
    {
        var agingCutoff = DateTime.UtcNow.AddMinutes(-15);

        return await dbContext.Requests
            .Where(x => x.Status == RequestStatus.Waiting)
            .OrderByDescending(x =>
                x.Priority == RequestPriority.Urgent
                    ? 2
                    : x.CreatedAt <= agingCutoff
                        ? 3
                        : 1)
            .ThenBy(x => x.CreatedAt)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async System.Threading.Tasks.Task AddAsync(
        Request request,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Requests.AddAsync(
            request,
            cancellationToken);
    }

    public System.Threading.Tasks.Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Request> CreateOrGetByKeyAsync(
    Request request,
    CancellationToken cancellationToken = default)
    {
        var existing = await GetByRequestKeyAsync(
            request.RequestKey,
            cancellationToken);

        if (existing is not null)
            return existing;

        await dbContext.Requests.AddAsync(request, cancellationToken);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return request;
        }
        catch (DbUpdateException ex)
            when (IsUniqueConstraintViolation(ex))
        {
            dbContext.Entry(request).State = EntityState.Detached;

            var concurrentRequest = await GetByRequestKeyAsync(
                request.RequestKey,
                cancellationToken);

            if (concurrentRequest is not null)
                return concurrentRequest;

            throw;
        }
    }

    private static bool IsUniqueConstraintViolation(
        DbUpdateException exception)
    {
        return exception.InnerException is SqlException sqlException
            && sqlException.Number is 2601 or 2627;
    }

}