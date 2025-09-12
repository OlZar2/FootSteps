using FS.Contracts.Error;
using FS.Core.Enums;
using FS.Core.Exceptions;
using FS.Core.ValueObjects.Abstract;

namespace FS.Core.ValueObjects;

public class MissingPetInfo : ValueObject
{
    public PetType PetType { get; private set; }
    
    public Gender Gender { get; private set; }
    
    public string Breed { get; private set; }
    
    public string Color { get; private set; }
    
    public string PetName { get; private set; }
    
    private MissingPetInfo(PetType petType, Gender gender, string breed, string color, string petName)
    {
        PetType = petType;
        Gender = gender;
        Breed = breed;
        Color = color;
        PetName = petName;
    }
    
    public static MissingPetInfo Create(PetType petType, Gender gender, string breed, string color, string petName)
    {
        if (string.IsNullOrWhiteSpace(breed))
        {
            throw new DomainException(IssueCodes.Required, "Порода не может быть пустой", nameof(Breed));
        }
        if (string.IsNullOrWhiteSpace(color))
        {
            throw new DomainException(IssueCodes.Required, "Окрас не может быть пустым", nameof(Color));
        }
        if (string.IsNullOrWhiteSpace(petName))
        {
            throw new DomainException(IssueCodes.Required, "Кличка не может быть пустой", nameof(PetType));
        }

        return new MissingPetInfo(petType, gender, breed, color, petName);
    }
    
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return PetType;
        yield return Gender;
        yield return Breed;
        yield return Color;;
    }
}