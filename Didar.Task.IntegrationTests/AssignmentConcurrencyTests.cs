using Didar.Task.Application.DTOs;
using Didar.Task.Application.Services;
using Didar.Task.Domain.Entities;
using Didar.Task.Domain.Enums;
using Didar.Task.Infrastructure.Persistence;
using Didar.Task.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Didar.Task.IntegrationTests;

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(
                Environment.GetEnvironmentVariable("DIDAR_TASK_TEST_SQLSERVER")))
        {
            Skip = "Set DIDAR_TASK_TEST_SQLSERVER to a dedicated SQL Server test database connection string.";
        }
    }
}

public sealed class AssignmentConcurrencyTests
{
    private readonly string _connectionString =
        Environment.GetEnvironmentVariable("DIDAR_TASK_TEST_SQLSERVER")
        ?? throw new InvalidOperationException(
            "DIDAR_TASK_TEST_SQLSERVER is required to run SQL Server integration tests.");

    [SqlServerFact]
    public async System.Threading.Tasks.Task Twenty_concurrent_requests_never_exceed_expert_capacity()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_connectionString)
            .Options;
        var requestKeyPrefix = $"it-{Guid.NewGuid():N}-";
        Guid? expertId = null;

        await using (var setupContext = new AppDbContext(options))
        {
            await setupContext.Database.MigrateAsync();

            var expert = new Expert(
                name: $"Concurrency test {Guid.NewGuid():N}",
                capacity: 5,
                specialties: [Specialty.Technical]);

            setupContext.Experts.Add(expert);
            await setupContext.SaveChangesAsync();
            expertId = expert.Id;
        }

        try
        {
            var submissions = Enumerable.Range(0, 20)
                .Select(async index =>
                {
                    // Each caller gets its own DbContext, as separate HTTP
                    // requests and separate application instances would.
                    await using var context = new AppDbContext(options);
                    var service = new AssignmentService(
                        new RequestRepository(context),
                        new AssignmentRepository(context));

                    return await service.CreateRequestAsync(
                        new CreateRequestDto(
                            $"{requestKeyPrefix}{index}",
                            Specialty.Technical,
                            RequestPriority.Normal));
                });

            await System.Threading.Tasks.Task.WhenAll(submissions);

            await using var verifyContext = new AppDbContext(options);
            var requests = await verifyContext.Requests
                .Where(request => request.RequestKey.StartsWith(requestKeyPrefix))
                .ToListAsync();

            Assert.Equal(20, requests.Count);
            Assert.Equal(5, requests.Count(request =>
                request.Status == RequestStatus.Assigned));
            Assert.Equal(15, requests.Count(request =>
                request.Status == RequestStatus.Waiting));
            Assert.All(
                requests.Where(request => request.Status == RequestStatus.Assigned),
                request => Assert.Equal(expertId, request.AssignedExpertId));

            var openAssignments = await verifyContext.Requests.CountAsync(request =>
                request.AssignedExpertId == expertId &&
                request.Status == RequestStatus.Assigned);
            Assert.Equal(5, openAssignments);
        }
        finally
        {
            await using var cleanupContext = new AppDbContext(options);
            var testRequests = await cleanupContext.Requests
                .Where(request => request.RequestKey.StartsWith(requestKeyPrefix))
                .ToListAsync();
            cleanupContext.Requests.RemoveRange(testRequests);

            if (expertId.HasValue)
            {
                var expert = await cleanupContext.Experts
                    .FirstOrDefaultAsync(item => item.Id == expertId.Value);
                if (expert is not null)
                    cleanupContext.Experts.Remove(expert);
            }

            await cleanupContext.SaveChangesAsync();
        }
    }
}
