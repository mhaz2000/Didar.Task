using Didar.Task.Domain.Enums;

namespace Didar.Task.Domain.Entities;

public class Expert
{
    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public bool IsActive { get; private set; }

    public int Capacity { get; private set; }

    public ICollection<ExpertSpecialty> Specialties { get; private set; }
    = new List<ExpertSpecialty>();

    private Expert()
    {
        // For ORM
    }

    public Expert(
        string name,
        int capacity,
        IEnumerable<Specialty> specialties)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Expert name is required.",
                nameof(name));

        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Capacity must be greater than zero.");

        ArgumentNullException.ThrowIfNull(specialties);

        var specialtySet = specialties.ToHashSet();

        if (specialtySet.Count == 0)
            throw new ArgumentException(
                "At least one specialty is required.",
                nameof(specialties));

        Id = Guid.NewGuid();
        Name = name;
        Capacity = capacity;
        Specialties = specialties
            .Distinct()
            .Select(s => new ExpertSpecialty(s))
            .ToList();
        IsActive = true;
    }

    public bool CanHandle(Specialty specialty) =>
        IsActive && Specialties.Any(x => x.Specialty == specialty);

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void ChangeCapacity(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(capacity),
                "Capacity must be greater than zero.");

        Capacity = capacity;
    }

    public void SetSpecialties(IEnumerable<Specialty> specialties)
    {
        ArgumentNullException.ThrowIfNull(specialties);

        var specialtySet = specialties.ToHashSet();

        if (specialtySet.Count == 0)
            throw new ArgumentException(
                "At least one specialty is required.",
                nameof(specialties));

        Specialties = specialties
            .Distinct()
            .Select(s => new ExpertSpecialty(s))
            .ToList();
    }
}