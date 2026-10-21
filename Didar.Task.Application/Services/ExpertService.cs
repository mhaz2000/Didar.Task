using Didar.Task.Application.DTOs;
using Didar.Task.Application.Interfaces;
using Didar.Task.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Didar.Task.Application.Services;

public sealed class ExpertService(
    IExpertRepository expertRepository) : IExpertService
{
    public async Task<ExpertDto> CreateAsync(
        CreateExpertDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Expert name is required.");

        if (dto.Capacity <= 0)
            throw new ArgumentException("Capacity must be greater than zero.");

        if (dto.Specialties is null || dto.Specialties.Count == 0)
            throw new ArgumentException(
                "At least one specialty is required.");

        if (dto.Specialties.Any(x => !Enum.IsDefined(x)))
            throw new ArgumentException("Invalid specialty.");

        var expert = new Expert(
            dto.Name.Trim(),
            dto.Capacity,
            dto.Specialties);

        await expertRepository.AddAsync(expert, cancellationToken);
        await expertRepository.SaveChangesAsync(cancellationToken);

        return new ExpertDto(
            expert.Id,
            expert.Name,
            expert.IsActive,
            expert.Capacity,
            0,
            expert.Specialties
                .Select(x => x.Specialty)
                .Distinct()
                .ToArray());
    }

    public Task<IReadOnlyList<ExpertDto>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return expertRepository.GetAllAsync(cancellationToken);
    }

    public async Task<ExpertDto?> GetByIdAsync(
    Guid id,
    CancellationToken cancellationToken = default)
    {
        return await expertRepository.GetDtoByIdAsync(id);
    }
}