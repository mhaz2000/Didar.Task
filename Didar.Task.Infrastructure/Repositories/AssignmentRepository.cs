using Didar.Task.Application.Interfaces;
using Didar.Task.Domain.Enums;
using Didar.Task.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;

namespace Didar.Task.Infrastructure.Repositories;


public sealed class AssignmentRepository(
    AppDbContext dbContext) : IAssignmentRepository
{
    public async Task<bool> TryAssignAsync(
        Guid requestId,
        CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(cancellationToken);

        await AcquireAssignmentLockAsync(transaction, cancellationToken);

        var request = await dbContext.Requests
            .FirstOrDefaultAsync(
                x => x.Id == requestId,
                cancellationToken);

        if (request is null || request.Status != RequestStatus.Waiting)
            return false;

        var candidate = await dbContext.Experts
            .AsNoTracking()
            .Where(expert =>
                expert.IsActive &&
                expert.Specialties.Any(
                    specialty => specialty.Specialty == request.Subject))
            .Select(expert => new
            {
                expert.Id,
                expert.Capacity,
                OpenRequests = dbContext.Requests.Count(r =>
                    r.AssignedExpertId == expert.Id &&
                    r.Status == RequestStatus.Assigned)
            })
            .Where(x => x.OpenRequests < x.Capacity)
            .OrderBy(x => (double)x.OpenRequests / x.Capacity)
            .ThenBy(x => x.OpenRequests)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (candidate is null)
            return false;

        request.AssignTo(candidate.Id);

        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return true;
    }

    private async System.Threading.Tasks.Task AcquireAssignmentLockAsync(
        IDbContextTransaction transaction,
        CancellationToken cancellationToken)
    {
        var connection = dbContext.Database.GetDbConnection();

        await using var command = connection.CreateCommand();

        command.Transaction = transaction.GetDbTransaction();
        command.CommandType = CommandType.Text;
        command.CommandText = """
            DECLARE @result int;

            EXEC @result = sys.sp_getapplock
                @Resource = @resource,
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = 10000;

            SELECT @result;
            """;

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@resource";
        parameter.Value = "ExpertAssignment:Assign";
        command.Parameters.Add(parameter);

        var result = Convert.ToInt32(
            await command.ExecuteScalarAsync(cancellationToken));

        if (result < 0)
        {
            throw new TimeoutException(
                "Could not acquire the assignment database lock.");
        }
    }
}