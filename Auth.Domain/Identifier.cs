namespace Auth.Domain;

public class Identifier
{
    public int Id { get; protected set; }

    protected T SetInvalide<T>() where T : Identifier
    {
        Id = -1;
        return this as T ?? throw new InvalidCastException();
    }
    
    public bool IsValide() => Id > 0;
}