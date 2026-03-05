namespace FS.Application.DTOs.GeoDTOs;

public class OverpassResponse
{
    public List<Element> elements { get; set; } = [];
    public sealed class Element
    {
        public string type { get; set; } = "";
        public long id { get; set; }
        public Dictionary<string, string>? tags { get; set; }
    }
}