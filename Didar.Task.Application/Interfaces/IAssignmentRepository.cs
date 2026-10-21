namespace Didar.Task.Application.Interfaces;

public interface IAssignmentRepository
{
    Task<bool> TryAssignAsync(
        Guid requestId,
        CancellationToken cancellationToken = default);
}