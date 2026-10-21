using Didar.Task.Application.DTOs;

namespace Didar.Task.Application.Interfaces;

public interface IExpertService
{
    Task<ExpertDto> CreateAsync(
        CreateExpertDto dto,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ExpertDto>> GetAllAsync(
        CancellationToken cancellationToken = default);

    Task<ExpertDto?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default);
}