namespace Auth.Domain;

public class Role : Atributte
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Authorization> Authorizations { get; private set; }
    public bool Active  { get; private set; }

    private Role(string name, string description, List<Authorization> authorizations, bool active)
    {
        Name = name;
        Description = description;
        Authorizations = authorizations;
        Active = active;
        base.Added =  true;
    }
    
    public void Activate() => Active = true;
    
    public void Deactivate() => Active = false;
    
    public static Role Create(string name, string description, List<Authorization> authorizations, bool active) =>
    new Role(name, description, authorizations, active);
    
    public static Role CreateToAdd(string name, string description, List<Authorization> authorizations, bool active)
    {
        var role = new Role(name, description, authorizations, active);
        role.AddedNow();
        return role;
    }

    public void AddAuthorizations(List<Authorization> authorization)
    {
        authorization.ForEach(a => a.AddedNow());
        Authorizations.AddRange(authorization);
    }

    public void RemoveAuthorizations(List<Authorization> authorization)
    {
        List<Authorization> authAddedToRemove = new();
        Authorizations
            .Where(a => authorization.Contains(a))
            .ToList()
            .ForEach(a =>
            {
                if (a.Added)
                    authAddedToRemove.Add(a);
                else
                    a.Deactivate();
            });
        Authorizations.RemoveAll(a => authAddedToRemove.Contains(a));
    }
}