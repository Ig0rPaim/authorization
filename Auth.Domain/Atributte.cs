namespace Auth.Domain;

public abstract class Atributte : Identifier
{
    public bool Added {get; protected set;}
    public void AddedNow()  => Added = true;
}