using FS.Contracts.Error;
using FS.Core.Enums;
using FS.Core.Exceptions;
using FS.Core.ValueObjects.Abstract;

namespace FS.Core.ValueObjects;

public class FoundPetInfo : ValueObject
{
    public PetType PetType { get; private set; }
    
    public Gender? Gender { get; private set; }
    
    public string? Breed { get; private set; }
    
    public string Color { get; private set; }
    
    private FoundPetInfo(PetType petType, Gender? gender, string? breed, string color)
    {
        PetType = petType;
        Gender = gender;
        Breed = breed;
        Color = color;
    }
    
    public static FoundPetInfo Create(PetType petType, Gender? gender, string? breed, string color)
    {
        if (string.IsNullOrWhiteSpace(color))
        {
            throw new DomainException(IssueCodes.Required, "Окрас не может быть пустым", nameof(Color));
        }

        return new FoundPetInfo(petType, gender, breed, color);
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PetType;
        yield return Gender;
        yield return Breed;
        yield return Color;
    }
}