namespace Auth.Domain;

public class Page
{
    public Guid Guid { get; private set; }
    public string Name { get; private set; }

    public Page(Guid guid, string name)
    {
        Guid = guid;
        Name = name;
    }
}