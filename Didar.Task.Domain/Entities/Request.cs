using Didar.Task.Domain.Enums;

namespace Didar.Task.Domain.Entities;

public class Request
{
    public Guid Id { get; private set; }

    public string RequestKey { get; private set; } = null!;

    public Specialty Subject { get; private set; }

    public RequestPriority Priority { get; private set; }

    public RequestStatus Status { get; private set; }

    public Guid? AssignedExpertId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? AssignedAt { get; private set; }

    public DateTime? ClosedAt { get; private set; }

    private Request()
    {
        // For ORM
    }

    public Request(
        string requestKey,
        Specialty subject,
        RequestPriority priority)
    {
        if (string.IsNullOrWhiteSpace(requestKey))
            throw new ArgumentException(
                "Request key is required.",
                nameof(requestKey));

        Id = Guid.NewGuid();
        RequestKey = requestKey;
        Subject = subject;
        Priority = priority;
        Status = RequestStatus.Waiting;
        CreatedAt = DateTime.UtcNow;
    }

    public void AssignTo(Guid expertId)
    {
        if (Status != RequestStatus.Waiting)
            throw new InvalidOperationException(
                "Only waiting requests can be assigned.");

        if (expertId == Guid.Empty)
            throw new ArgumentException(
                "Expert ID is required.",
                nameof(expertId));

        AssignedExpertId = expertId;
        AssignedAt = DateTime.UtcNow;
        Status = RequestStatus.Assigned;
    }

    public void Close()
    {
        if (Status != RequestStatus.Assigned)
            throw new InvalidOperationException(
                "Only assigned requests can be closed.");

        ClosedAt = DateTime.UtcNow;
        Status = RequestStatus.Closed;
    }
}
