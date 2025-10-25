namespace Auth.Domain;

public class User
{
    public string Email { get; set; }
    public List<Role> Roles { get; private set; }
    public List<Authorization> Authorizations { get; private set; }

    public void AddAuthorizations(List<Authorization> authorization, Func<List<Role>> getAllRoles)
    {
        var allRoles = getAllRoles();
        var rolesToAdd = new List<Role>();
        allRoles.RemoveAll(r => Roles.Contains(r));

        allRoles.ForEach(r =>
        {
            AuthorizationMaybeIncludedInRole(r, ref authorization, rolesToAdd);
            AuthorizationMaybeLeakRole(r, authorization, rolesToAdd);
                        
        });
        
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

    void AuthorizationMaybeIncludedInRole(Role roleNotAdded, ref List<Authorization> authorizationsToAdded, List<Role> rolesToAdd)
    {
        if (roleNotAdded.Authorizations.Count <= authorizationsToAdded.Count)
            return;
        bool newAuthInsideRole = !authorizationsToAdded.Select(a => roleNotAdded.Authorizations.Contains(a)).Contains(false);
        if (newAuthInsideRole)
        {
            rolesToAdd.Add(roleNotAdded);
            authorizationsToAdded = new List<Authorization>();
        }
    }
    void AuthorizationMaybeLeakRole(Role roleNotAdded, List<Authorization> authorizationsToAdded, List<Role> rolesToAdd)
    {
        if(roleNotAdded.Authorizations.Count > authorizationsToAdded.Count)
            return;
        bool newAuthLeakRole =  !roleNotAdded.Authorizations.Select(a => authorizationsToAdded.Contains(a)).Contains(false);
        if (newAuthLeakRole)
        {
            rolesToAdd.Add(roleNotAdded);
            authorizationsToAdded.RemoveAll(a => 
                rolesToAdd
                    .Select(r => r.Authorizations)
                    .Aggregate((auth, next) => auth.Concat(next).ToList())
                    .Contains(a)
            );
        }
    }
    
    
    #region 
    // if (r.Authorizations.Count > authorization.Count)
    // {        
    //     bool newAuthInsideRole = !authorization.Select(a => r.Authorizations.Contains(a)).Contains(false);
    //     if (newAuthInsideRole)
    //     {
    //         rolesToAdd.Add(r);
    //         authorization = new List<Authorization>();
    //     }
    // }
    // else
    // {
    //     bool newAuthLeakRole =  !r.Authorizations.Select(a => authorization.Contains(a)).Contains(false);
    //     if (newAuthLeakRole)
    //     {
    //         rolesToAdd.Add(r);
    //         authorization.RemoveAll(a => 
    //             rolesToAdd
    //             .Select(r => r.Authorizations)
    //             .Aggregate((auth, next) => auth.Concat(next).ToList())
    //             .Contains(a)
    //         );
    //     }
    // }
    #endregion
    
}