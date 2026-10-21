using Didar.Task.Application.DTOs;
using Didar.Task.Application.Interfaces;
using Didar.Task.Domain.Entities;
using Didar.Task.Domain.Enums;
using Didar.Task.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Didar.Task.Infrastructure.Repositories;


public sealed class ExpertRepository(
    AppDbContext dbContext) : IExpertRepository
{
    public async System.Threading.Tasks.Task AddAsync(
        Expert expert,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Experts.AddAsync(expert, cancellationToken);
    }

    public Task<Expert?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return dbContext.Experts
            .Include(x => x.Specialties)
            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }


    public Task<ExpertDto?> GetDtoByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return dbContext.Experts
            .AsNoTracking()
            .Where(expert => expert.Id == id)
            .Select(expert => new ExpertDto(
                expert.Id,
                expert.Name,
                expert.IsActive,
                expert.Capacity,
                dbContext.Requests.Count(request =>
                    request.AssignedExpertId == expert.Id &&
                    request.Status == RequestStatus.Assigned),
                expert.Specialties
                    .Select(s => s.Specialty)
                    .ToArray()))
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<IReadOnlyList<ExpertDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Experts
            .AsNoTracking()
            .OrderBy(expert => expert.Name)
            .Select(expert => new ExpertDto(
                expert.Id,
                expert.Name,
                expert.IsActive,
                expert.Capacity,
                dbContext.Requests.Count(request =>
                    request.AssignedExpertId == expert.Id &&
                    request.Status == RequestStatus.Assigned),
                expert.Specialties
                    .Select(s => s.Specialty)
                    .ToArray()))
            .ToListAsync(cancellationToken);
    }

    public async Task<ExpertCandidateDto?> SelectBestCandidateAsync(
        Specialty specialty,
        CancellationToken cancellationToken = default)
    {
        return await dbContext.Experts
            .AsNoTracking()
            .Where(expert =>
                expert.IsActive &&
                expert.Specialties.Any(
                    item => item.Specialty == specialty))
            .Select(expert => new
            {
                expert.Id,
                expert.Capacity,
                OpenRequests = dbContext.Requests.Count(request =>
                    request.AssignedExpertId == expert.Id &&
                    request.Status == RequestStatus.Assigned)
            })
            .Where(x => x.OpenRequests < x.Capacity)
            .OrderBy(x => (double)x.OpenRequests / x.Capacity)
            .ThenBy(x => x.OpenRequests)
            .ThenBy(x => x.Id)
            .Select(x => new ExpertCandidateDto(
                x.Id,
                x.Capacity,
                x.OpenRequests))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public System.Threading.Tasks.Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
