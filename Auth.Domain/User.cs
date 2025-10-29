namespace Auth.Domain;

public class User
{
    public string Email { get; set; }
    public List<Role> Roles { get; private set; }
    public List<Authorization> Authorizations { get; private set; }
    public List<Authorization> AllAuthorizations
    {
        get
        {
            return Roles
                .Select(r => r.Authorizations)
                .Aggregate((auth, next) => auth.Concat(next).ToList())
                .Concat(Authorizations).ToList();
        }
    }
    public void AddAuthorizations(List<Authorization> authorization, Func<List<Role>> getAllRoles)
    {
        var allRoles = getAllRoles();
        var rolesToAdd = new List<Role>();
        allRoles.RemoveAll(r => Roles.Contains(r));
        rolesToAdd.AddRange(
            allRoles
                .Select(r => r.AuthorizationsInRole(ref authorization))
                .Where(r => r.IsValide()));
        
        if (rolesToAdd.Any())
        {
            rolesToAdd.ForEach(r => r.AddedNow());
            Roles.AddRange(rolesToAdd);
        }

        if (authorization.Any())
        {
            authorization.ForEach(a => a.AddedNow());
            Authorizations.AddRange(authorization);
        }
    }
    public void RemoveAuthorizations(List<Authorization> authorization, bool force = false)
    {
        var authorizationsToRemove = Authorizations.Where(authorization.Contains);
        
        Authorizations.RemoveAll(a => authorizationsToRemove.Contains(a));
        
        authorization.RemoveAll(authorizationsToRemove.Contains);
        
        if(!authorization.Any())
            return;
        
        var rolesToRemove = Roles.Select(r => r.AnyAuthorizationAreInRole(authorization)).Where(r => r.IsValide());
        
        var toRemove = rolesToRemove as Role[] ?? rolesToRemove.ToArray();
        
        if (!toRemove.Any())
            return;
        
        if (!force)
        {
            string message = string.Join("\n", Role.RolesThatWillRemoved(toRemove));
            throw new InvalidOperationException(message);
        }
        
        Roles.RemoveAll(r => toRemove.Contains(r));
    }
    public void AddRole(Role role)
    {
        role.AddedNow();
        Roles.Add(role);
    }
    public void RemoveRole(Role role)
    {
        List<Role> rolesToRemove = new();
        Roles.Where(r => r.Equals(role)).ToList().ForEach(r =>
        {
            if(r.Added)
                rolesToRemove.Add(r);
            else
                r.Deactivate();
        });
    }
}