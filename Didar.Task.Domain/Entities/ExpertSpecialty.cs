using Didar.Task.Domain.Enums;

namespace Didar.Task.Domain.Entities;

public sealed class ExpertSpecialty
{
    public Guid ExpertId { get; private set; }
    public Specialty Specialty { get; private set; }

    private ExpertSpecialty() { }

    public ExpertSpecialty(Specialty specialty)
    {
        Specialty = specialty;
    }
}