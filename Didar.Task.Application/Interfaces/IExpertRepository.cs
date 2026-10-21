using Didar.Task.Application.DTOs;
using Didar.Task.Domain.Entities;
using Didar.Task.Domain.Enums;

namespace Didar.Task.Application.Interfaces;

public interface IExpertRepository
{
    System.Threading.Tasks.Task AddAsync(
        Expert expert,
        CancellationToken cancellationToken = default);

    Task<Expert?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExpertDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ExpertCandidateDto?> SelectBestCandidateAsync(
        Domain.Enums.Specialty specialty,
        CancellationToken cancellationToken = default);

    System.Threading.Tasks.Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
    Task<ExpertDto?> GetDtoByIdAsync(Guid id, CancellationToken cancellationToken = default);
}