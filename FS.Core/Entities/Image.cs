namespace FS.Core.Entities;

//TODO: переработать всю систему картинок. Надо сделать VO, а не агрегат
public class Image
{
    public Guid Id { get; private set; }
    public string Path { get; private set; }

    private Image(Guid id, string path)
    {
        Id = id;
        Path = path;
    }

    public static Image Create(string ext)
    {
        //TODO: подумать какое исключение бросать
        if (string.IsNullOrWhiteSpace(ext))
            throw new ArgumentException("Расширение не может быть пустым");
        
        var id = Guid.NewGuid();
        var path = id + ext;

        return new Image(id, path);
    }
}