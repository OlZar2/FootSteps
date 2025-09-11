using FS.Contracts.Error;
using FS.Core.Exceptions;

namespace FS.Core.Entities;

public class PetType
{
    public Guid Id { get; private set; }
    
    public string Name { get; private set; }

    private PetType(
        Guid id,
        string name)
    {
        Id = id;
        Name = name;
    }

    public static PetType Create(Guid id, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new DomainException(
            IssueCodes.Required, "Pet type name cannot be null or whitespace.", nameof(name));

        return new PetType(id, name);
    }
}