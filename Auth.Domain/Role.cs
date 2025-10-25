namespace Auth.Domain;

public class Role : Atributte
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public List<Authorization> Authorizations { get; private set; }
    public bool Active  { get; private set; }

    private Role(){}
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
    public static List<string> RolesThatWillRemoved(Role[] roles)
    {
        if(!roles.Any())
            return null;
        List<string> message = new List<string>() { "There are no roles that will removed:" };
        foreach (var role in roles)
        {
            message.Add($"- {role.Name}");
        }
        return message;
    }
    public Role AnyAuthorizationAreInRole(IEnumerable<Authorization> authorizations)
    {
        if(!Authorizations.Select(authorizations.Contains).Contains(true))
            return new Role().SetInvalide<Role>();
        return this;
    }
    public Role AuthorizationsInRole(List<Authorization> authorizationsToAdded)
    {
        if (Authorizations.Count >= authorizationsToAdded.Count)
            return AuthorizationsAreContainedInTheRole(ref authorizationsToAdded);
        return AuthorizationsAreLeakingOfTheRole(authorizationsToAdded);
    }
    private Role AuthorizationsAreContainedInTheRole(ref List<Authorization> authorizationsToAdded)
    {
        bool newAuthInsideRole = !authorizationsToAdded.Select(a => Authorizations.Contains(a)).Contains(false);
        if (!newAuthInsideRole) 
            return new Role().SetInvalide<Role>();
        
        authorizationsToAdded = new List<Authorization>();
        return this;
        
    }
    private Role AuthorizationsAreLeakingOfTheRole(List<Authorization> authorizationsToAdded)
    {
        // if(Authorizations.Count >= authorizationsToAdded.Count)
        //     return new Role().SetInvalide<Role>();
        
        bool newAuthLeakRole = !Authorizations.Select(authorizationsToAdded.Contains).Contains(false);
        
        if (!newAuthLeakRole)
            return new Role().SetInvalide<Role>();
        
        authorizationsToAdded.RemoveAll(Authorizations.Contains);

        return this;
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
    public override bool Equals(object? obj)
    {
        if (obj is Role role)
        {
            return 
                Name.Equals(role.Name) &&
                Description.Equals(role.Description) &&
                Authorizations.SequenceEqual(role.Authorizations);
        }
        return base.Equals(obj);
    }
}