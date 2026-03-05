namespace FS.Core.Shared.Geo;

public class City
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsCalculated { get; private set; }
    public List<District> Districts { get; private set; }

    public City(string name, List<District> districts)
    {
        Name = name;
        Districts = districts;
    }
}